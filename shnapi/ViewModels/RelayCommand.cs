// =============================================================
// ВСПОМОГАТЕЛЬНЫЙ КЛАСС для команд (используется во ViewModel)
// RelayCommand реализует интерфейс ICommand, который позволяет
// привязывать кнопки и другие элементы UI к методам ViewModel
// без code-behind в View.
// =============================================================

using System.Windows.Input;

namespace shnapi.ViewModels
{
    /// <summary>
    /// Универсальная реализация ICommand.
    /// Принимает делегаты execute и canExecute в конструкторе,
    /// что позволяет описывать логику команды прямо во ViewModel.
    /// </summary>
    public class RelayCommand : ICommand
    {
        // Действие, которое выполняется при вызове команды
        private readonly Action<object?> _execute;

        // Условие, при котором команда доступна (можно null — всегда доступна)
        private readonly Func<object?, bool>? _canExecute;

        /// <param name="execute">Логика выполнения команды</param>
        /// <param name="canExecute">Условие активности команды (необязательно)</param>
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Вызывается WPF для проверки, активна ли команда.
        /// Если вернёт false — кнопка будет заблокирована автоматически.
        /// </summary>
        public bool CanExecute(object? parameter)
            => _canExecute == null || _canExecute(parameter);

        /// <summary>
        /// Выполняет команду.
        /// </summary>
        public void Execute(object? parameter)
            => _execute(parameter);

        /// <summary>
        /// Событие, которое сообщает WPF о необходимости
        /// перепроверить CanExecute (например, после изменения данных).
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add    => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
