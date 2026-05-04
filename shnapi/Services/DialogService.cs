// =============================================================
// РЕАЛИЗАЦИЯ СЕРВИСА ДИАЛОГОВ
// Конкретная реализация IDialogService на основе WPF MessageBox.
// ViewModel не импортирует этот класс напрямую —
// она получает его через конструктор как IDialogService.
// Благодаря этому в тестах можно подменить реализацию
// на фиктивную (mock) без изменения кода ViewModel.
// =============================================================

using System.Windows;

namespace shnapi.Services
{
    /// <summary>
    /// Реализация IDialogService через стандартный WPF MessageBox.
    /// Регистрируется в IoC-контейнере как Transient или Singleton —
    /// см. комментарий в App.xaml.cs.
    /// </summary>
    public class DialogService : IDialogService
    {
        /// <inheritdoc/>
        public void ShowInfo(string message, string title = "Информация")
            => MessageBox.Show(message, title,
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);

        /// <inheritdoc/>
        public void ShowWarning(string message, string title = "Предупреждение")
            => MessageBox.Show(message, title,
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);

        /// <inheritdoc/>
        public void ShowError(string message, string title = "Ошибка")
            => MessageBox.Show(message, title,
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);

        /// <inheritdoc/>
        public bool ShowConfirmation(string message, string title = "Подтверждение")
            => MessageBox.Show(message, title,
                               MessageBoxButton.YesNo,
                               MessageBoxImage.Question) == MessageBoxResult.Yes;
    }
}
