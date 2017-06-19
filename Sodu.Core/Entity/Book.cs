using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using SQLite.Net.Attributes;

namespace Sodu.Core.Entity
{
    public class Book : ViewModelBase
    {
        [PrimaryKey]// 主键。
        [AutoIncrement]// 自动增长。
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        ///书ID
        /// </summary>
        [Unique]
        public string BookId { get; set; }

        /// <summary>
        ///书名
        /// </summary>
        public string BookName { get; set; }


        private string _newestChapterName;
        /// <summary>
        ///最新章节名称
        /// </summary>
        public string NewestChapterName
        {
            get
            {
                return _newestChapterName;
            }
            set
            {
                Set(ref _newestChapterName, value);
            }

        }
        /// <summary>
        ///当前章节正文地址
        /// </summary>
        public string NewestChapterUrl { get; set; }

        private string _cover;
        /// <summary>
        ///封面
        /// </summary>
        public string Cover
        {
            get
            {
                return _cover;
            }
            set
            {
                Set(ref _cover, value);
            }
        }

        private string _description;
        /// <summary>
        ///简介
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                Set(ref _description, value);
            }
        }



        private string _lastReadChapterName;
        /// <summary>
        ///最后阅读的章节名称
        /// </summary>
        public string LastReadChapterName
        {

            get
            {
                return _lastReadChapterName;
            }
            set
            {
                Set(ref _lastReadChapterName, value);
            }
        }

        private string _lastReadChapterUrl;
        /// <summary>
        ///最后阅读的章节地址
        /// </summary>
        public string LastReadChapterUrl
        {

            get
            {
                return _lastReadChapterUrl;
            }
            set
            {
                Set(ref _lastReadChapterUrl, value);
            }
        }

        private string _unReadCountData;
        /// <summary>
        ///未读提示
        /// </summary>
        [Ignore]
        public string UnReadCountData
        {
            get
            {
                return _unReadCountData;
            }
            set
            {
                Set(ref _unReadCountData, value);
            }
        }

        private bool _isNew;
        /// <summary>
        ///是否有更新
        /// </summary>
        [Ignore]
        public bool IsNew
        {
            get
            {
                return _isNew;
            }
            set
            {
                Set(ref _isNew, value);
            }
        }


        /// <summary>   
        ///更新时间
        /// </summary>
        private string _updateTime;

        public string UpdateTime
        {

            get
            {
                return _updateTime;
            }

            set
            {
                try
                {
                    _updateTime = !string.IsNullOrEmpty(value)
                   ? DateTime.Parse(value).ToString("yyyy-MM-dd HH:mm")
                   : DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                }
                catch (Exception)
                {
                    _updateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                }
            }
        }

        /// <summary>
        ///更新目录地址
        /// </summary>
        public string UpdateCatalogUrl { get; set; }

        /// <summary>
        /// 正文章节列表地址
        /// </summary>
        public string CatalogListUrl
        {
            get; set;
        }

        /// <summary>
        ///作者名称
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        ///来源网站
        /// </summary>
        public string LyWeb { get; set; }

        /// 排行榜数据
        public string RankChangeValue { get; set; }

        /// <summary>
        /// 是否是本地图书
        /// </summary>
        private bool _isLocal;
        public bool IsLocal
        {
            get
            {
                return _isLocal;
            }
            set
            {
                Set(ref _isLocal, value);
            }
        }

        /// <summary>
        /// 是否为历史记录
        /// </summary>
        private bool _isHistory;
        public bool IsHistory
        {
            get
            {
                return _isHistory;
            }
            set
            {
                Set(ref _isHistory, value);
            }
        }

        [Ignore]
        public ObservableCollection<BookCatalog> CatalogList { get; set; }

        [Ignore]
        public ObservableCollection<BookCatalog> UnDownloadCatalogList { get; set; }

        /// <summary>
        /// 是否为编辑状态
        /// </summary>
        private bool _isInEdit;
        [Ignore]
        public bool IsInEdit
        {
            get
            {
                return _isInEdit;
            }
            set
            {
                Set(ref _isInEdit, value);
            }
        }

        /// <summary>
        /// 是否勾选
        /// </summary>
        private bool _isSelected;
        [Ignore]
        [XmlIgnore]
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                Set(ref _isSelected, value);
            }
        }


        public Book Clone()
        {
            var str = JsonConvert.SerializeObject(this);
            var entity = JsonConvert.DeserializeObject<Book>(str);
            return entity;
        }

    }
}
