using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Sodu.Core.Extend
{
    public class AutoLoadCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        private Action _loadAction = null;
        private bool IsBusy { get; set; } = false;

        public AutoLoadCollection(Action action)
        {
            _loadAction = action;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (IsBusy)
            {
                return null;
            }

            return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
        }

        public bool HasMoreItems => true;


        protected async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken cancle, uint count)
        {
            try
            {
                IsBusy = true;

                await Task.Delay(1, cancle);

                _loadAction?.Invoke();

                return new LoadMoreItemsResult();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
