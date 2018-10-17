using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto
{
    public class PageListDto<T>
    {
        public PageListDto()
        { }
        public PageListDto(List<T> data, int pageIndex, int pageSize) : this(data, pageIndex, pageSize, -1)
        { }

        public PageListDto(List<T> data, int pageIndex, int pageSize, int totalCount)
        {
            if (data == null)
            {
                this.TotalCount = 0;
                this.TotalPages = 0;
                this.PageSize = pageSize;
                this.PageIndex = pageIndex;
                return;
            }
            TotalCount = totalCount < 1 ? data.Count() : totalCount;


            if (pageSize == 0)//pageSize == 0时，默认获取全部
            {
                TotalPages = 1;
            }
            if (pageSize > 0)
            {
                TotalPages = TotalCount / pageSize;

                if (TotalCount % pageSize > 0)
                    TotalPages++;
            }

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.PageData = data;
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
