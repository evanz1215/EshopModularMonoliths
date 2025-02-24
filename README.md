# EshopModularMonoliths
Modular Monolithic Architecture with .NET

Add-Migration InitialCreate -OutputDir Data/Migrations -Project Ordering -StartupProject Api -Context OrderingDbContext
Update-Database -Context OrderingDbContext