namespace KTRegistration.API;
public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.ConfigureCORS(configuration);
        services.ConfigureIdentity();
        services.ConfigureDatabase(configuration);
        services.ConfigureSwagger();
        services.ConfigureMapster();
        services.ConfigureFluentValidation();
        services.ConfigureServiceImplementations();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddHttpContextAccessor();
        return services;
    }

    private static IServiceCollection ConfigureCORS(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials()
                       .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()
                       ??throw new InvalidOperationException("AllowedOrigins in appsettings not found."));
            });
        });

        return services;
    }

    private static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection String 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        return services;
    }

    private static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

        return services;
    }

    private static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Koperasi Tentera API",
                Version = "v1",
                Description = "API documentation with last update date",
                Contact = new OpenApiContact
                {
                    Name = "Deena Hammouri",
                    Email = "Deenahamouri@gmail.com"
                },
                Extensions = new Dictionary<string, IOpenApiExtension>
            {
                { "LastUpdateDate", new OpenApiString("28-10-2024") },
                { "Environment", new OpenApiString("Development") },
                { "APIStatus", new OpenApiString("Active") }
            }
            });
        });

        return services;
    }

    private static IServiceCollection ConfigureMapster(this IServiceCollection services)
    {
        services.AddMapster();
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan((AppDomain.CurrentDomain.GetAssemblies()));
        services.AddSingleton<IMapper>(new Mapper(mappingConfig));
        return services;
    }

    private static IServiceCollection ConfigureFluentValidation(this IServiceCollection services)
    {
        services
               .AddFluentValidationAutoValidation()
               .AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

        return services;
    }

    private static IServiceCollection ConfigureServiceImplementations(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
