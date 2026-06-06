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

        // Search
        public string? Search { get; set; }

        // Sorting
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; } = "asc";
    }

    public interface IQueryService
    {
        Task<Pagination<TDto>> QueryAsync<TEntity, TDto>(
            IQueryable<TEntity> query,
            QueryParameters parameters,
            Func<TEntity, TDto> map,
            Func<IQueryable<TEntity>, string, IQueryable<TEntity>>? search = null
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
            Func<IQueryable<TEntity>, string, IQueryable<TEntity>>? search = null
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

        private IQueryable<TEntity> ApplySorting<TEntity>(IQueryable<TEntity> query, string sortBy, string? direction)
        {
            var prop = typeof(TEntity).GetProperty(sortBy);

            if (prop == null) return query;

            return direction?.ToLower() == "desc"
                ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                : query.OrderBy(e => EF.Property<object>(e, sortBy));
        }

        private string _GenerateCacheKey<TEntity>(QueryParameters parameters)
        {
            return $"{typeof(TEntity).Name}_" +
                      $"{parameters.Page}_{parameters.PerPage}_" +
                      $"{parameters.Search}_" +
                      $"{parameters.SortBy}_{parameters.SortDirection}_";
        }
    }
}