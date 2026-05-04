// =============================================================
// CODE-BEHIND главного окна — View в паттерне MVVM
//
// ИЗМЕНЕНИЕ: конструктор теперь принимает MainViewModel
// как параметр (Constructor Injection).
// DI-контейнер передаёт уже готовый экземпляр с внедрёнными
// зависимостями. Здесь просто устанавливается DataContext.
// =============================================================

using System.Windows;
using shnapi.ViewModels;

namespace shnapi
{
    public partial class MainWindow : Window
    {
        /// <param name="viewModel">
        /// Внедряется IoC-контейнером автоматически.
        /// MainWindow не создаёт ViewModel сам — он его получает.
        /// </param>
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();

            // Устанавливаем DataContext программно.
            // Все привязки {Binding ...} в XAML будут работать
            // с этим экземпляром ViewModel.
            DataContext = viewModel;
        }
    }
}
