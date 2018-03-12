using System;
using System.Collections.Generic;

namespace Gravity.Data
{
    public class DataPage<T>
    {
        public DataPage(List<T> data, int count, int pageSize, int pageIndex)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Count = count;
            PageSize = pageSize;
            PageIndex = pageIndex;
        }
        
        public List<T> Data { get; }

        public int Count { get; }

        public int PageSize { get; }

        public int PageIndex { get; }
        
        public int TotalPages => Count / PageSize + 1;
        
        public bool HasPrevious => PageIndex > 0;

        public bool HasNext => PageIndex < TotalPages - 1;
    }
}