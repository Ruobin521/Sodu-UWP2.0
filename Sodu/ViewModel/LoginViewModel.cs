using System;
using System.Collections.Generic;
using System.Linq;
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
    public class LoginViewModel : ViewModelBase
    {

        private HttpHelper Http { get; set; }

        private bool _isLoading;
        /// <summary>
        ///正在加载
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }



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



        private void OnLoginCommand()
        {
            if (!CheckInput())
            {
                ToastHelper.ShowMessage("请输入用户名密码");
                return;
            }

            LoginAction();
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
                var html = await Http.HttpClientPostRequest(WebPageUrl.LoginPostPage, postdata);
                if (html != null && html.Contains("{\"success\":true}"))
                {
                    ToastHelper.ShowMessage("登陆成功");

                    ViewModelInstance.Instance.Main.SetLoginAction(true);
                    CookieHelper.SetCookie(WebPageUrl.LoginPostPage, true);
                    NavigationService.GoBack();
                }
                else
                {
                    ToastHelper.ShowMessage("账号或密码错误，请重新输入。");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }


        private bool CheckInput()
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserName.Trim()) || string.IsNullOrEmpty(PassWd) || string.IsNullOrEmpty(PassWd.Trim()))
            {
                return false;
            }


            return true;
        }


    }
}
