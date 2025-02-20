namespace Basket;

public static class BasketModule
{
    public static IServiceCollection AddBasketModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 2. Application Use Case services
        services.AddScoped<IBasketRepository, BasketRepository>();

        //services.AddScoped<IBasketRepository, CachedBasketRepository>();

        //services.AddScoped<IBasketRepository>(provider =>
        //{
        //    var basketRepository = new BasketRepository(provider.GetRequiredService<BasketDbContext>());
        //    return new CachedBasketRepository(basketRepository, provider.GetRequiredService<IDistributedCache>());
        //});

        services.Decorate<IBasketRepository, CachedBasketRepository>();

        // 3. Data - Infrastructure services

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<BasketDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        return services;
    }

    public static IApplicationBuilder UseBasketModule(this IApplicationBuilder app)
    {
        // Configure the HTTP request pipeline.

        app.UseMigration<BasketDbContext>();

        return app;
    }
}