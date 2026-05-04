// =============================================================
// CODE-BEHIND окна (часть View в паттерне MVVM)
// В правильном MVVM code-behind должен быть максимально пустым.
// Вся логика вынесена в MainViewModel.
// Здесь остаётся только инициализация окна.
// =============================================================

using System.Windows;

namespace shnapi
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml.
    /// DataContext (ViewModel) задаётся прямо в XAML — см. MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Инициализирует компоненты, сгенерированные из XAML
            InitializeComponent();
        }
    }
}
