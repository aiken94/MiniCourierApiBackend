using Microsoft.EntityFrameworkCore;

namespace CourierBackend.Services
{
    public class Pagination<T>
    {
        public string? Status { get; set; }
        public List<T>? Data { get; set; }

        public PaginationMeta? Meta { get; set; }

        public class PaginationMeta
        {
            public int CurrentPage { get; set; }
            public int PerPage { get; set; }
            public int Total { get; set; }
            public int LastPage { get; set; }
        }

        public static Pagination<T> Create(List<T> data, int total, int currentPage, int perPage)
        {
            return new Pagination<T>
            {
                Status = "success",
                Data = data,
                Meta = new PaginationMeta
                {
                    CurrentPage = currentPage,
                    PerPage = perPage,
                    Total = total,
                    LastPage = (int)Math.Ceiling(total / (double)perPage)
                }
            };
        }
    }

    public class PaginationRequest
    {
        private const int MaxPageSize = 100;

        public int Page { get; set; } = 1;

        private int _perPage = 10;
        public int PerPage
        {
            get => _perPage;
            set => _perPage = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }

    public interface IPaginationService
    {
        Task<Pagination<TDto>> PaginateAsync<TEntity, TDto>(
            IQueryable<TEntity> query,
            PaginationRequest request,
            Func<TEntity, TDto> map
        );
    }

    public class PaginationService : IPaginationService
    {
        public async Task<Pagination<TDto>> PaginateAsync<TEntity, TDto>(IQueryable<TEntity> query, PaginationRequest request, Func<TEntity, TDto> map)
        {
            var total = await query.CountAsync();

            var items = await query
                .Skip((request.Page - 1) * request.PerPage)
                .Take(request.PerPage)
                .ToListAsync();

            var data = items.Select(map).ToList();

            return Pagination<TDto>.Create(
                data,
                total,
                request.Page,
                request.PerPage
            );
        }
    }
}