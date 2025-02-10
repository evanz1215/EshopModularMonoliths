namespace Catalog.Products.Features.GetProducts;

public record GetProductsQueyr() : IQuery<GetProductsResult>;

public record GetProductsResult(IEnumerable<ProductDto> Products);

public class GetProductsHandler(CatalogDbContext dbContext) : IQueryHandler<GetProductsQueyr, GetProductsResult>
{
    public async Task<GetProductsResult> Handle(GetProductsQueyr query, CancellationToken cancellationToken)
    {
        var products = await dbContext.Products
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        // mapping product entity to productdto
        var productDtos = products.Adapt<List<ProductDto>>();

        return new GetProductsResult(productDtos);
    }
}