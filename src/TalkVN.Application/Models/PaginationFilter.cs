namespace TalkVN.Application.Models
{
    public class PaginationFilter
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public PaginationFilter()
        {
            this.PageIndex = 1;
            this.PageSize = 10;
        }
        public PaginationFilter(int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex < 0 ? 1 : pageIndex;
            this.PageSize = pageSize < 1 ? 10 : pageSize;
            this.PageSize = pageSize > 100 ? 100 : pageSize;
        }
    }
}
