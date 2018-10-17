using System.Collections.Generic;

namespace ShanDian.Machine.Dto
{
    public class VMPageList<T>
    {
        public VMPageList()
        { }
        public VMPageList(List<T> data, int pageIndex, int pageSize) : this(data, pageIndex, pageSize, -1)
        { }

        public VMPageList(List<T> data, int pageIndex, int pageSize, int totalCount)
        {
            if (data == null)
            {
                TotalCount = 0;
                TotalPages = 0;
                PageSize = pageSize;
                PageIndex = pageIndex;
                return;
            }
            TotalCount = totalCount < 1 ? data.Count : totalCount;


            if (pageSize == 0 )//pageSize == 0时，默认获取全部
            {
                TotalPages = 1;
            }
            if (pageSize > 0)
            {
                TotalPages = TotalCount / pageSize;

                if (TotalCount % pageSize > 0)
                    TotalPages++;
            }

            PageSize = pageSize;
            PageIndex = pageIndex;
            PageData = data;
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        public List<T> PageData { set; get; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 显示页码数量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总的数量
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总的页数
        /// </summary>
        public int TotalPages { get; set; }
    }
}
