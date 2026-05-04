// =============================================================
// МОДЕЛЬ (Model) в паттерне MVVM
// Отвечает только за данные и бизнес-логику.
// Не знает ничего о UI (View) и ViewModel.
// =============================================================

namespace shnapi.Models
{
    /// <summary>
    /// Модель контакта телефонной книги.
    /// Хранит данные: имя и номер телефона.
    /// </summary>
    public class Contact
    {
        /// <summary>Имя контакта</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Номер телефона</summary>
        public string Phone { get; set; } = string.Empty;
    }
}
