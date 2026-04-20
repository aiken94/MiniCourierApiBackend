namespace CourierBackend.Services
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;

    public class QueryParameters
    {
        private const int MaxPageSize = 100;

        public int Page { get; set; } = 1;

        private int _perPage = 10;
        public int PerPage
        {
            get => _perPage;
            set => _perPage = value > MaxPageSize ? MaxPageSize : value;
        }

        public string? Fields { get; set; }

        // Search
        public string? Search { get; set; }

        // Sorting
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; } = "asc";

        // Filtering (key-value pairs)
        public Dictionary<string, Dictionary<string, string>>? Filters { get; set; }
    }

    public interface IQueryService
    {
        Task<Pagination<TDto>> QueryAsync<TEntity, TDto>(
            IQueryable<TEntity> query,
            QueryParameters parameters,
            Func<TEntity, TDto> map,
            Func<IQueryable<TEntity>, string, IQueryable<TEntity>>? search = null,
            Func<IQueryable<TEntity>, Dictionary<string, string>, IQueryable<TEntity>>? filter = null
        );
    }

    public class QueryService : IQueryService
    {
        private readonly IMemoryCache _cache;

        public QueryService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<Pagination<TDto>> QueryAsync<TEntity, TDto>(
            IQueryable<TEntity> query,
            QueryParameters parameters,
            Func<TEntity, TDto> map,
            Func<IQueryable<TEntity>, string, IQueryable<TEntity>>? search = null,
            Func<IQueryable<TEntity>, Dictionary<string, string>, IQueryable<TEntity>>? filter = null
        )
        {
            // for previous page caching, we can generate a cache key based on the query parameters
            var cacheKey = _GenerateCacheKey<TEntity>(parameters);

            if (_cache.TryGetValue(cacheKey, out Pagination<TDto>? cached))
            {
                return cached;
            }

            // SEARCH
            if (!string.IsNullOrWhiteSpace(parameters.Search) && search != null)
            {
                query = search(query, parameters.Search);
            }

            // FILTER
            if (parameters.Filters != null)
            {
                query = FilterService.Apply(query, parameters.Filters);
            }

            // SORT
            if (!string.IsNullOrWhiteSpace(parameters.SortBy))
            {
                query = ApplySorting(query, parameters.SortBy, parameters.SortDirection);
            }

            var total = query.Count();

            // PAGINATION
            var items = query
                .Skip((parameters.Page - 1) * parameters.PerPage)
                .Take(parameters.PerPage)
                .ToList();

            var data = items.Select(map).ToList();

            // Apply field selection
            //var fields = FieldSelectionService.Apply(data, parameters.Fields);

            var response = Pagination<TDto>.Create(
                data,
                total,
                parameters.Page,
                parameters.PerPage
            );

            // Cache it(e.g., 60 seconds)
            _cache.Set(cacheKey, response, TimeSpan.FromSeconds(60));

            return response;
        }

        private IQueryable<TEntity> ApplySorting<TEntity>(
            IQueryable<TEntity> query,
            string sortBy,
            string? direction
        )
        {
            var prop = typeof(TEntity).GetProperty(sortBy);

            if (prop == null) return query;

            return direction?.ToLower() == "desc"
                ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                : query.OrderBy(e => EF.Property<object>(e, sortBy));
        }

        private string _GenerateCacheKey<TEntity>(QueryParameters parameters)
        {
            var key = $"{typeof(TEntity).Name}_" +
                      $"{parameters.Page}_{parameters.PerPage}_" +
                      $"{parameters.Search}_" +
                      $"{parameters.SortBy}_{parameters.SortDirection}_" +
                      $"{parameters.Fields}";

            if (parameters.Filters != null)
            {
                foreach (var f in parameters.Filters)
                {
                    foreach (var op in f.Value)
                    {
                        key += $"_{f.Key}-{op.Key}-{op.Value}";
                    }
                }
            }

            return key;
        }
    }
}