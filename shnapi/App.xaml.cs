// =============================================================
// APP.XAML.CS — точка входа приложения
// Здесь настраивается IoC-контейнер (DI-контейнер).
// Именно здесь определяется, какая реализация будет подставлена
// под каждый интерфейс, и какое время жизни у каждого сервиса.
//
// Использован пакет Microsoft.Extensions.DependencyInjection.
// Установить: ПКМ на проект → Управление пакетами NuGet →
//             найти «Microsoft.Extensions.DependencyInjection» → Установить
// =============================================================

using Microsoft.Extensions.DependencyInjection;
using shnapi.Services;
using shnapi.ViewModels;
using System.Windows;

namespace shnapi
{
    public partial class App : Application
    {
        // IServiceProvider — корневой объект IoC-контейнера.
        // Через него разрешаются все зарегистрированные зависимости.
        private IServiceProvider _serviceProvider = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // ── 1. Создаём коллекцию дескрипторов сервисов ──────────────
            var services = new ServiceCollection();

            // ── 2. Регистрируем сервисы ──────────────────────────────────

            // DialogService → Transient (новый экземпляр при каждом запросе).
            // Подходит для лёгких stateless-сервисов, таких как диалоги:
            // каждый вызов независим, хранить состояние не нужно.
            services.AddTransient<IDialogService, DialogService>();

            // MainViewModel → Transient.
            // Создаётся один раз при старте (для одного окна), но
            // Transient корректен: ViewModel не должна быть Singleton,
            // чтобы не накапливать состояние между сессиями.
            services.AddTransient<MainViewModel>();

            // MainWindow → Transient.
            // Окно регистрируем в контейнере, чтобы DI мог внедрить
            // MainViewModel в его конструктор автоматически.
            services.AddTransient<MainWindow>();

            // ── 3. Строим контейнер ───────────────────────────────────────
            _serviceProvider = services.BuildServiceProvider();

            // ── 4. Разрешаем главное окно и показываем его ───────────────
            // Контейнер сам создаст MainWindow, передав ему MainViewModel,
            // а тот — IDialogService. Весь граф зависимостей строится
            // автоматически.
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
