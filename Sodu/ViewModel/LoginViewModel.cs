using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Sodu.Core.HtmlService;
using Sodu.Core.Util;
using Sodu.Service;
using Sodu.View;

namespace Sodu.ViewModel
{
    public class LoginViewModel : CommonPageViewModel
    {


        #region 属性

        private string _userName;

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            set { Set(ref _userName, value); }
        }

        private string _passWd;
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWd
        {
            get { return _passWd; }
            set { Set(ref _passWd, value); }
        }

        private string _passWd2;
        /// <summary>
        /// 密码2
        /// </summary>
        public string PassWd2
        {
            get { return _passWd2; }
            set { Set(ref _passWd2, value); }
        }



        #endregion

        #region 命令

        private ICommand _loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (
                           _loginCommand = new RelayCommand<object>(
                               (obj) =>
                               {
                                   OnLoginCommand();
                               }));
            }
        }


        private ICommand _registerCommand;
        public ICommand RegisterCommand
        {
            get
            {
                return _registerCommand ?? (
                           _registerCommand = new RelayCommand<object>(
                               (obj) =>
                               {
                                   OnRegisterCommand();
                               }));
            }
        }

        private ICommand _toRegisterCommand;
        public ICommand ToRegisterCommand
        {
            get
            {
                return _toRegisterCommand ?? (
                           _toRegisterCommand = new RelayCommand<object>(
                               (obj) =>
                               {
                                   NavigationService.NavigateTo(typeof(RegisterPage));
                               }));
            }
        }



        private ICommand _logoutCommand;
        public ICommand LogoutCommand
        {
            get
            {
                return _logoutCommand ?? (
                           _logoutCommand = new RelayCommand<object>(
                               (obj) =>
                               {
                                   OnLogoutCommand();

                               }));
            }
        }


        #endregion

        #region 构造函数

        public LoginViewModel()
        {
            if (CookieHelper.CheckLogin())
            {
                UserName = AppSettingService.GetKeyValue(SettingKey.UserName) as string;
            }
        }


        #endregion


        #region 方法

        private void OnLoginCommand()
        {
            if (!CheckLoginInput())
            {
                return;
            }

            LoginAction();
        }
        private bool CheckLoginInput()
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserName.Trim()) || string.IsNullOrEmpty(PassWd) || string.IsNullOrEmpty(PassWd.Trim()))
            {
                ToastHelper.ShowMessage("请输入用户名密码");
                return false;
            }


            return true;
        }
        private async void LoginAction()
        {
            if (IsLoading)
            {
                return;
            }

            IsLoading = true;
            try
            {
                var postdata = "username=" + this.UserName + "&userpass=" + this.PassWd;

                Http = new HttpHelper();
                var html = await Http.HttpClientPostRequest(SoduPageValue.LoginPostPage, postdata);
                if (html != null && html.Contains("{\"success\":true}"))
                {
                    AppSettingService.SetKeyValue(SettingKey.UserName, UserName);
                    CookieHelper.SetCookie(SoduPageValue.LoginPostPage, true);
                    ViewModelInstance.Instance.Main.SetLoginAction(true);
                    NavigationService.GoBack();
                }
                else
                {
                    ToastHelper.ShowMessage("账号或密码错误，请重新输入。");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OnRegisterCommand()
        {
            if (!CheckRegisterInput())
            {
                return;
            }

            RegisterAction();
        }
        private bool CheckRegisterInput()
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserName.Trim()) || string.IsNullOrEmpty(PassWd) || string.IsNullOrEmpty(PassWd.Trim()) || string.IsNullOrEmpty(PassWd2) || string.IsNullOrEmpty(PassWd2.Trim()))
            {
                ToastHelper.ShowMessage("请输入用户名密码");
                return false;
            }

            if (!PassWd.Equals(PassWd2))
            {
                ToastHelper.ShowMessage("两次密码输入不一致，请确认");
                return false;
            }
            return true;
        }
        private async void RegisterAction()
        {
            if (IsLoading)
            {
                return;
            }

            IsLoading = true;
            try
            {
                var postdata = "username=" + WebUtility.UrlEncode(UserName) + "&userpass=" + this.PassWd;

                Http = new HttpHelper();
                var html = await Http.HttpClientPostRequest(SoduPageValue.RegisterPostPage, postdata);
                if (html != null && html.Contains("{\"success\":true}"))
                {
                    ToastHelper.ShowMessage("注册成功");

                    AppSettingService.SetKeyValue(SettingKey.UserName, UserName);
                    ViewModelInstance.Instance.Main.SetLoginAction(true);
                    CookieHelper.SetCookie(SoduPageValue.LoginPostPage, true);
                    NavigationService.NavigateTo(typeof(MainPage));
                    NavigationService.ClearHistory();
                }
                else
                {
                    ToastHelper.ShowMessage("注册失败，该用户名可能已经被注册");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                IsLoading = false;
            }
        }


        private void OnLogoutCommand()
        {
            CookieHelper.SetCookie(SoduPageValue.HomePage, false);
            AppSettingService.SetKeyValue(SettingKey.UserName, null);
            ViewModelInstance.Instance.Main.SetLoginAction(false);
            NavigationService.ClearHistory();
            NavigationService.NavigateTo(typeof(MainPage));
            NavigationService.NavigateTo(typeof(LoginPage));
        }

        #endregion
    }
}
