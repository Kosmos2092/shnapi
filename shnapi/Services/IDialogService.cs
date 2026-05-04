// =============================================================
// ИНТЕРФЕЙС СЕРВИСА ДИАЛОГОВ (часть слоя Services)
// Абстракция, которую ViewModel использует для показа диалогов.
// ViewModel знает только об интерфейсе — не о MessageBox или
// о любой другой конкретной реализации UI.
// Это и есть суть DI: зависимость от абстракции, не от класса.
// =============================================================

namespace shnapi.Services
{
    /// <summary>
    /// Контракт сервиса пользовательских диалогов.
    /// Реализацию предоставляет IoC-контейнер — ViewModel
    /// об этом ничего не знает.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>Информационное сообщение (ОК)</summary>
        void ShowInfo(string message, string title = "Информация");

        /// <summary>Предупреждение (ОК)</summary>
        void ShowWarning(string message, string title = "Предупреждение");

        /// <summary>Сообщение об ошибке (ОК)</summary>
        void ShowError(string message, string title = "Ошибка");

        /// <summary>
        /// Запрос подтверждения (Да / Нет).
        /// Возвращает true если пользователь нажал «Да».
        /// </summary>
        bool ShowConfirmation(string message, string title = "Подтверждение");
    }
}
