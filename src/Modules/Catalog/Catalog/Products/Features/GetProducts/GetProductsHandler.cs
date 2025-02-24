namespace Catalog.Products.Features.GetProducts;

public record GetProductsQueyr(PaginationRequest PaginatedRequest) : IQuery<GetProductsResult>;

public record GetProductsResult(PaginatedResult<ProductDto> Products);

public class GetProductsHandler(CatalogDbContext dbContext) : IQueryHandler<GetProductsQueyr, GetProductsResult>
{
    public async Task<GetProductsResult> Handle(GetProductsQueyr query, CancellationToken cancellationToken)
    {
        int pageIndex = query.PaginatedRequest.PageIndex;
        int pageSize = query.PaginatedRequest.PageSize;

        long totalCount = await dbContext.Products.LongCountAsync(cancellationToken);

        var products = await dbContext.Products
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        // mapping product entity to productdto
        var productDtos = products.Adapt<List<ProductDto>>();

        //return new GetProductsResult(productDtos);

        return new GetProductsResult(new PaginatedResult<ProductDto>(pageIndex, pageSize, totalCount, productDtos));
    }
}