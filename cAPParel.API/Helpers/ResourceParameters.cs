using cAPParel.API.Filters;

namespace cAPParel.API.Helpers
{
    public class ResourceParameters
    {
        const int maxPageSize = 200;
        public string? SearchQuery { get; set; }
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 100;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
        public string OrderBy { get; set; } = "Id";
        public string? Fields { get; set; }
    }
}
