namespace WebAppMvc.Models {
    public class PaginatedViewModel<TModel> {
        public IEnumerable<TModel> Items { get; }

        public PaginatedViewModel(IEnumerable<TModel> items, int page, int pageSize, int count, string? nameFilter, string? descriptionFilter, string orderBy, string orderDirection) {
            Items = items;

            Page = page;

            PageSize = pageSize;
            NameFilter = nameFilter;
            DescriptionFilter = descriptionFilter;
            OrderBy = orderBy;
            OrderDirection = orderDirection;
            TotalPages = Convert.ToInt32(Math.Round(1.0 * count / pageSize));
        }

        public int Page { get; }
        public int PageSize { get; }
        public string? NameFilter { get; }
        public string? DescriptionFilter { get; }
        public string OrderBy { get; }
        public string OrderDirection { get; }
        public int TotalPages { get; }

    }
}
