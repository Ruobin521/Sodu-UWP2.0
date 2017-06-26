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
    public abstract class IncrementalLoadingBase<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        public bool HasMoreItems
        {
            get { return HasMoreItemsOverride(); }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (_busy)
            {
                throw new InvalidOperationException("Only one operation in flight at a time");
            }

            _busy = true;

            return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
        }

        protected async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {
                // 加载开始事件
                if (this.OnLoadMoreStarted != null)
                {
                    this.OnLoadMoreStarted(count);
                }

                var items = await LoadMoreItemsOverrideAsync(c, count);

                AddItems(items);

                // 加载完成事件
                if (this.OnLoadMoreCompleted != null)
                {
                    this.OnLoadMoreCompleted(items == null ? 0 : items.Count);
                }

                return new LoadMoreItemsResult { Count = items == null ? 0 : (uint)items.Count };
            }
            finally
            {
                _busy = false;
            }
        }


        public delegate void LoadMoreStarted(uint count);
        public delegate void LoadMoreCompleted(int count);

        public event LoadMoreStarted OnLoadMoreStarted;
        public event LoadMoreCompleted OnLoadMoreCompleted;

        /// <summary>
        /// 将新项目添加进来，之所以是virtual的是为了方便特殊要求，比如不重复之类的
        /// </summary>
        protected virtual void AddItems(IList<T> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    this.Add(item);
                }
            }
        }

        protected abstract Task<IList<T>> LoadMoreItemsOverrideAsync(CancellationToken c, uint count);

        protected abstract bool HasMoreItemsOverride();


        protected bool _busy = false;
    }
}
