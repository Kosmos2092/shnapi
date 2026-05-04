// =============================================================
// VIEWMODEL — рефакторинг с применением Dependency Injection
// Изменения относительно предыдущей версии:
//   — IDialogService внедряется через конструктор (Constructor Injection).
//   — ViewModel больше не создаёт диалоги самостоятельно и
//     не зависит от конкретного класса DialogService.
//   — Конкретную реализацию подставит IoC-контейнер при
//     разрешении зависимости (см. App.xaml.cs).
// =============================================================

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using shnapi.Models;
using shnapi.Services;

namespace shnapi.ViewModels
{
    /// <summary>
    /// ViewModel главного окна.
    /// Зависит от IDialogService — не от DialogService напрямую.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        // -------------------------------------------------------
        // ЗАВИСИМОСТЬ, внедрённая через конструктор (DI)
        // -------------------------------------------------------

        // Сервис диалогов. Тип — интерфейс, не конкретный класс.
        // IoC-контейнер подставит реализацию автоматически.
        private readonly IDialogService _dialog;

        // -------------------------------------------------------
        // ПОЛЯ
        // -------------------------------------------------------

        private string _newName         = string.Empty;
        private string _newPhone        = string.Empty;
        private string _validationError = string.Empty;

        // -------------------------------------------------------
        // СВОЙСТВА
        // -------------------------------------------------------

        public string NewName
        {
            get => _newName;
            set { _newName = value; OnPropertyChanged(); }
        }

        public string NewPhone
        {
            get => _newPhone;
            set { _newPhone = value; OnPropertyChanged(); }
        }

        /// <summary>Текст ошибки валидации. Пустая строка = нет ошибки.</summary>
        public string ValidationError
        {
            get => _validationError;
            set { _validationError = value; OnPropertyChanged(); }
        }

        // -------------------------------------------------------
        // КОЛЛЕКЦИЯ КОНТАКТОВ
        // ObservableCollection уведомляет DataGrid автоматически.
        // -------------------------------------------------------

        public ObservableCollection<Contact> Contacts { get; }
            = new ObservableCollection<Contact>();

        // -------------------------------------------------------
        // КОМАНДЫ
        // -------------------------------------------------------

        public ICommand AddCommand    { get; }
        public ICommand DeleteCommand { get; }

        // -------------------------------------------------------
        // КОНСТРУКТОР
        // IDialogService приходит снаружи — из IoC-контейнера.
        // ViewModel не знает, какая именно реализация будет передана.
        // -------------------------------------------------------

        public MainViewModel(IDialogService dialogService)
        {
            _dialog = dialogService;

            AddCommand    = new RelayCommand(_ => ExecuteAdd(),  _ => CanAdd());
            DeleteCommand = new RelayCommand(ExecuteDelete,      p  => p is Contact);
        }

        // -------------------------------------------------------
        // ЛОГИКА КОМАНД
        // -------------------------------------------------------

        private bool CanAdd()
            => !string.IsNullOrWhiteSpace(NewName) &&
               !string.IsNullOrWhiteSpace(NewPhone);

        /// <summary>
        /// Добавление контакта:
        ///  1. Валидация формата.
        ///  2. Проверка дубликата по номеру — ShowWarning через сервис.
        ///  3. Успешное добавление   — ShowInfo через сервис.
        /// </summary>
        private void ExecuteAdd()
        {
            // --- Валидация имени ---
            if (string.IsNullOrWhiteSpace(NewName))
            {
                ValidationError = "Имя не может быть пустым.";
                return;
            }

            // --- Валидация формата телефона ---
            var phoneRegex = new Regex(@"^(\+7|8)?\d{10}$");
            if (!phoneRegex.IsMatch(NewPhone.Trim()))
            {
                ValidationError = "Неверный формат. Примеры: +79001234567, 89001234567";
                return;
            }

            ValidationError = string.Empty;

            // --- Проверка дубликата ---
            // Если номер уже есть в списке — предупреждаем через сервис
            // и отменяем добавление.
            bool duplicate = false;
            foreach (var c in Contacts)
            {
                if (c.Phone == NewPhone.Trim())
                {
                    duplicate = true;
                    break;
                }
            }

            if (duplicate)
            {
                // ShowWarning вызывается через интерфейс —
                // ViewModel не знает, что за ним стоит MessageBox.
                _dialog.ShowWarning(
                    $"Контакт с номером {NewPhone.Trim()} уже существует.",
                    "Дубликат номера");
                return;
            }

            // --- Добавление ---
            var newContact = new Contact
            {
                Name  = NewName.Trim(),
                Phone = NewPhone.Trim()
            };
            Contacts.Add(newContact);

            // Информируем пользователя об успешном добавлении.
            _dialog.ShowInfo(
                $"Контакт «{newContact.Name}» успешно добавлен.",
                "Контакт добавлен");

            NewName  = string.Empty;
            NewPhone = string.Empty;
        }

        /// <summary>
        /// Удаление контакта:
        ///  Запрашиваем подтверждение через ShowConfirmation.
        ///  Если пользователь нажал «Нет» — не удаляем.
        /// </summary>
        private void ExecuteDelete(object? parameter)
        {
            if (parameter is not Contact contact) return;

            // ShowConfirmation возвращает true при нажатии «Да».
            bool confirmed = _dialog.ShowConfirmation(
                $"Удалить контакт «{contact.Name}» ({contact.Phone})?",
                "Подтверждение удаления");

            if (confirmed)
                Contacts.Remove(contact);
        }

        // -------------------------------------------------------
        // INotifyPropertyChanged
        // -------------------------------------------------------

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
