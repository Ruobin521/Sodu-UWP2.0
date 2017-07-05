using System;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Threading;
using Newtonsoft.Json;
using Sodu.Contants;
using Sodu.Core.Config;
using Sodu.Core.DataBase;
using Sodu.Core.Entity;
using Sodu.Core.HtmlService;
using Sodu.Core.Util;
using Sodu.Service;
using Sodu.View;
using static Windows.Phone.UI.Input.HardwareButtons;

namespace Sodu
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {

        public static bool IsPro;

        public static Frame RootFrame;
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this.UnhandledException += App_UnhandledException;

            var package = Windows.ApplicationModel.Package.Current;

#if !DEBUG
            IsPro = !package.DisplayName.Equals("小说搜索阅读 UWP");
#endif

#if DEBUG
            IsPro = false;
#endif
            if (!IsPro)
            {
                CookieHelper.SetCookie(SoduPageValue.HomePage, false);
            }
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (PlatformHelper.CurrentPlatform == PlatformHelper.Platform.IsMobile)
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                statusBar.ForegroundColor = Colors.White;
                statusBar.BackgroundOpacity = 1;
                statusBar.BackgroundColor = ConstantValue.AppMainColor;
            }

            RootFrame = Window.Current.Content as Frame;

         

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (RootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                RootFrame = new Frame();
                DispatcherHelper.Initialize();

                if (PlatformHelper.CurrentPlatform == PlatformHelper.Platform.IsPc)
                {
                    //RootFrame.Navigated += RootFrame_Navigated;
                    //SystemNavigationManager.GetForCurrentView().BackRequested += (sender, args) =>
                    //{
                    //    args.Handled = true;
                    //    OnAppBack();
                    //};
                    //rootFrame.KeyUp -= RootFrame_KeyUp;
                    //rootFrame.KeyUp += RootFrame_KeyUp;

                    //rootFrame.KeyDown -= RootFrame_KeyDown;
                    //rootFrame.KeyDown += RootFrame_KeyDown;
                }
                if (PlatformHelper.CurrentPlatform == PlatformHelper.Platform.IsMobile)
                {
                    BackPressed += (sender, args) =>
                    {
                        args.Handled = true;
                        OnAppBack();
                    };
                }

                RootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = RootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (RootFrame.Content == null)
                {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置
                    // 参数
                    RootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();

                ApplicationView.PreferredLaunchViewSize = new Size(450, 600);
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

                if (!string.IsNullOrEmpty(e.Arguments))
                {
                    OnTitleClick(e.Arguments);
                }
            }
        }


        private static void OnTitleClick(string args)
        {
            var titleEntity = JsonConvert.DeserializeObject<TitleEntity>(args);
            var book = JsonConvert.DeserializeObject<Book>(titleEntity.BookJosn);

            if (titleEntity.BookType == "0")
            {
                NavigationService.NavigateTo(typeof(UpdateCatalogPage));
                ViewModel.ViewModelInstance.Instance.UpdateCatalog.LoadData(book);
            }
            else
            {
                var temp = DbLocalBook.GetBookById(AppDataPath.GetLocalBookDbPath(), book.BookId);
                if (temp != null)
                {
                    NavigationService.NavigateTo(typeof(BookContentPage));
                    ViewModel.ViewModelInstance.Instance.BookContent.LoadData(temp);
                }
            }
        }

        public static async void HideStatusBar(bool isContent = false)
        {
            if (PlatformHelper.IsMobile)
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                statusBar.ForegroundColor = Colors.White;
                statusBar.BackgroundOpacity = 1;
                statusBar.BackgroundColor = isContent ? Color.FromArgb(255, 25, 25, 25) : Contants.ConstantValue.AppMainColor;
                await statusBar.HideAsync();
            }
        }

        public static async void ShowStatusBar(bool isContent = false)
        {
            if (PlatformHelper.IsMobile)
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                statusBar.ForegroundColor = Colors.White;
                statusBar.BackgroundOpacity = 1;
                statusBar.BackgroundColor = isContent ? Color.FromArgb(255, 25, 25, 25) : Contants.ConstantValue.AppMainColor;
                await statusBar.ShowAsync();
            }
        }

        public void OnAppBack()
        {
            NavigationService.GoBack();
        }

        private void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (PlatformHelper.CurrentPlatform == PlatformHelper.Platform.IsPc)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = RootFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            }
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }
    }
}
