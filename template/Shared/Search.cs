using System;
using System.Collections.Generic;
using System.Text;

namespace template.Shared
{
    public class Search
    {
        public string FilterText { get; set; }
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 25;
    }

    public class SearchResponse<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int Total { get; set; }
    }
}
