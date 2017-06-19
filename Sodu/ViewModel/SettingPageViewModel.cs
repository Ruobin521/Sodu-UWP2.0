using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Sodu.ViewModel
{
    public class SettingPageViewModel : ViewModelBase
    {
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

        #endregion
    }
}
