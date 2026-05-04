// =============================================================
// VIEWMODEL (ViewModel) в паттерне MVVM
// Связующее звено между Model и View.
// Содержит: логику UI, команды, свойства для привязки.
// НЕ знает о конкретных элементах UI (кнопках, полях ввода).
// View привязывается к свойствам ViewModel через Data Binding.
// =============================================================

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using shnapi.Models;

namespace shnapi.ViewModels
{
    /// <summary>
    /// ViewModel главного окна.
    /// Реализует INotifyPropertyChanged — это позволяет View
    /// автоматически обновляться при изменении свойств.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        // -------------------------------------------------------
        // ПОЛЯ
        // -------------------------------------------------------

        // Хранит введённое имя нового контакта
        private string _newName = string.Empty;

        // Хранит введённый телефон нового контакта
        private string _newPhone = string.Empty;

        // Хранит текст ошибки валидации
        private string _validationError = string.Empty;

        // -------------------------------------------------------
        // СВОЙСТВА, привязанные к полям ввода во View
        // -------------------------------------------------------

        /// <summary>
        /// Имя нового контакта (привязано к TextBox в XAML).
        /// При изменении уведомляет View через OnPropertyChanged.
        /// </summary>
        public string NewName
        {
            get => _newName;
            set { _newName = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Телефон нового контакта (привязан к TextBox в XAML).
        /// </summary>
        public string NewPhone
        {
            get => _newPhone;
            set { _newPhone = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Текст ошибки валидации. Пустая строка = нет ошибки.
        /// Привязан к TextBlock с сообщением об ошибке.
        /// </summary>
        public string ValidationError
        {
            get => _validationError;
            set { _validationError = value; OnPropertyChanged(); }
        }

        // -------------------------------------------------------
        // КОЛЛЕКЦИЯ КОНТАКТОВ
        // ObservableCollection автоматически уведомляет View
        // при добавлении/удалении элементов — не нужно вручную
        // обновлять список на экране.
        // -------------------------------------------------------

        /// <summary>
        /// Список контактов, отображаемый в DataGrid.
        /// </summary>
        public ObservableCollection<Contact> Contacts { get; }
            = new ObservableCollection<Contact>();

        // -------------------------------------------------------
        // КОМАНДЫ
        // ICommand — интерфейс WPF для привязки действий к кнопкам.
        // -------------------------------------------------------

        /// <summary>
        /// Команда добавления нового контакта.
        /// Без параметра — берёт данные из NewName и NewPhone.
        /// </summary>
        public ICommand AddCommand { get; }

        /// <summary>
        /// Команда удаления контакта.
        /// С параметром — получает объект Contact из DataGrid.
        /// </summary>
        public ICommand DeleteCommand { get; }

        // -------------------------------------------------------
        // КОНСТРУКТОР
        // -------------------------------------------------------

        public MainViewModel()
        {
            // Инициализируем команды, передавая методы-обработчики
            AddCommand    = new RelayCommand(_ => ExecuteAdd(),    _ => CanAdd());
            DeleteCommand = new RelayCommand(ExecuteDelete,        param => param is Contact);
        }

        // -------------------------------------------------------
        // ЛОГИКА КОМАНД
        // -------------------------------------------------------

        /// <summary>
        /// Условие активности кнопки "Добавить":
        /// поля не должны быть пустыми.
        /// </summary>
        private bool CanAdd()
            => !string.IsNullOrWhiteSpace(NewName) &&
               !string.IsNullOrWhiteSpace(NewPhone);

        /// <summary>
        /// Добавляет новый контакт после прохождения валидации.
        /// </summary>
        private void ExecuteAdd()
        {
            // --- Валидация имени ---
            if (string.IsNullOrWhiteSpace(NewName))
            {
                ValidationError = "Имя не может быть пустым.";
                return;
            }

            // --- Валидация телефона ---
            // Допустимые форматы: +7XXXXXXXXXX  или  8XXXXXXXXXX  или  XXXXXXXXXX (10 цифр)
            var phoneRegex = new Regex(@"^(\+7|8)?\d{10}$");
            if (!phoneRegex.IsMatch(NewPhone.Trim()))
            {
                ValidationError = "Неверный формат. Примеры: +79001234567, 89001234567";
                return;
            }

            // Валидация пройдена — добавляем контакт в коллекцию
            ValidationError = string.Empty;
            Contacts.Add(new Contact { Name = NewName.Trim(), Phone = NewPhone.Trim() });

            // Очищаем поля ввода
            NewName  = string.Empty;
            NewPhone = string.Empty;
        }

        /// <summary>
        /// Удаляет переданный контакт из коллекции.
        /// Параметр приходит из DataGrid через CommandParameter="{Binding}".
        /// </summary>
        private void ExecuteDelete(object? parameter)
        {
            if (parameter is Contact contact)
                Contacts.Remove(contact);
        }

        // -------------------------------------------------------
        // РЕАЛИЗАЦИЯ INotifyPropertyChanged
        // Уведомляет View об изменении конкретного свойства.
        // -------------------------------------------------------

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Вызывается внутри setter'а свойства.
        /// [CallerMemberName] автоматически подставляет имя свойства.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
