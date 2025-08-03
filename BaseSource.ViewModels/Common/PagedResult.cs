using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BaseSource.ViewModels.Common
{
    public class PagedResult<T> : PagedResultBase
    {
        public List<T> Items { set; get; }
    }

    public class PagedResultBase
    {
        public int PageNumber { get; set; }

        public int TotalItemCount { get; set; }

        public int PageCount
        {
            get
            {
                var pageCount = (double)TotalItemCount / PageSize;
                return (int)Math.Ceiling(pageCount);
            }
        }

        public string PageUrl { get; set; }
        public int PageSize { get; set; }
    }

    public class PageQuery
    {
        private int _page;
        public int Page
        {
            get
            {
                return _page >= 1 ? _page : 1;
            }
            set
            {
                _page = value >= 1 ? value : 1;
            }
        }

        private int _pageSize;
        public int PageSize
        {
            get
            {
                return _pageSize >= 1 ? _pageSize : 10;
            }
            set
            {
                _pageSize = value <= 50 ? value : 10;
            }
        }
    }

    public class PageQueryAll : PageQuery
    {
        public bool IsAll { get; set; }
    }

    public class PageQueryFixed
    {
        private int _page;
        public int Page
        {
            get
            {
                return _page >= 1 ? _page : 1;
            }
            set
            {
                _page = value >= 1 ? value : 1;
            }
        }
    }
}