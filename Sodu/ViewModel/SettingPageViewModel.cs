using System;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Sodu.Core.AppService;
using Sodu.Service;
using Sodu.View;

namespace Sodu.ViewModel
{


    public class SettingPageViewModel : ViewModelBase
    {

        #region 属性
        public bool IsFree => !App.IsPro;
        public bool IsPro => App.IsPro;

        private string _appVersion;
        /// <summary>
        /// 程序版本号
        /// </summary>
        public string AppVersion
        {
            get
            {
                return _appVersion;
            }
            set
            {
                Set(ref _appVersion, value);
            }
        }

        private bool _isAutoAddtoOnlineShelf;
        public bool IsAutoAddtoOnlineShelf
        {
            get
            {
                _isAutoAddtoOnlineShelf = AppSettingService.GetBoolKeyValue(SettingKey.IsAutoAddToOnlineShelf);
                return _isAutoAddtoOnlineShelf;
            }
            set
            {
                Set(ref _isAutoAddtoOnlineShelf, value);
                AppSettingService.SetKeyValue(SettingKey.IsAutoAddToOnlineShelf,value);
            }
        }


        #endregion

        #region 命令
        private ICommand _personnalCenterCommand;
        public ICommand PersonnalCenterCommand
        {
            get
            {
                return _personnalCenterCommand ?? (
                 _personnalCenterCommand = new RelayCommand<object>(
                      (obj) =>
                      {
                          NavigationService.NavigateTo(ViewModelInstance.Instance.Main.IsLogin
                              ? typeof(PersonalCenterPage)
                              : typeof(LoginPage));
                      }));
            }
        }



        private ICommand _buyProCommand;
        public ICommand BuyProCommand
        {
            get
            {
                return _buyProCommand ?? (
                 _buyProCommand = new RelayCommand<object>(
                      (obj) =>
                      {
                          NavigationService.NavigateTo(typeof(ProVersionPage));
                      }));
            }
        }

        private ICommand _historyCommand;
        public ICommand HistoryCommand
        {
            get
            {
                return _historyCommand ?? (
                 _historyCommand = new RelayCommand<object>(
                      (obj) =>
                      {
                          NavigationService.NavigateTo(typeof(HistoryPage));

                          ViewModelInstance.Instance.History.LoadData();
                      }));
            }
        }

        private ICommand _downloadCenterCommand;
        public ICommand DownloadCenterCommand
        {
            get
            {
                return _downloadCenterCommand ?? (
                 _downloadCenterCommand = new RelayCommand<object>(
                      (obj) =>
                      {
                          NavigationService.NavigateTo(typeof(DownloadCenterPage));
                      }));
            }
        }





        private ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                return _exitCommand ?? (
                 _exitCommand = new RelayCommand<object>(
                      (obj) =>
                      {
                          Application.Current.Exit();
                      }));
            }
        }


        private ICommand _commentCommand;
        public ICommand CommentCommand
        {
            get
            {
                return _commentCommand ?? (
                 _commentCommand = new RelayCommand<object>(
                     async (obj) =>
                      {
                          //免费版本

                          if (IsFree)
                          {
                              await Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9nblggh4sk4v"));
                          }

                          if (IsPro)
                          {
                              //捐赠版本
                              await Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9nblggh51vh6"));
                          }
                      }));
            }
        }



        private ICommand _mzsmCommand;
        public ICommand MzsmCommand
        {
            get
            {
                return _mzsmCommand ?? (
                 _mzsmCommand = new RelayCommand<object>(
                     async (obj) =>
                     {

                         var dialog = new ContentDialog()
                         {
                             Title = "免责声明",
                             Content = "\n　　小说搜索阅读是一款提供网络小说及时更新的工具，为广大小说爱好者提供一种方便，快捷，舒适的适度体验。\n\n　　小说搜索阅读中的所有数据均来自第三方，对所有返回的数据概不负责，亦不承担任何责任。\n\n　　小说搜索阅读鼓励用户支持正版阅读，为网络小说的发展做一份贡献。",
                             PrimaryButtonText = "确定",
                             FullSizeDesired = false,
                         };

                         dialog.PrimaryButtonClick += (s, e) => { };
                         await dialog.ShowAsync();

                     }));
            }
        }

        #endregion


        #region 方法

        public SettingPageViewModel()
        {
            InitData();
        }

        public void InitData()
        {
            var str = Package.Current.Id;
            string version = $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}";
            _appVersion = version + "  " + (IsPro ? "专业版" : "免费版");
        }




        #endregion

    }
}
