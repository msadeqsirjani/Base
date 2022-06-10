namespace Base.Infra.IoC;

public static class DependencyContainer
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var result = context.ModelState.Values.SelectMany(x => x.Errors).First().ErrorMessage;

                    return new UnprocessableEntityObjectResult(Result.WithException(result));
                };
            })
            .AddFluentValidation(options => options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()))
            .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

        services.AddOptions();
        services.AddMemoryCache();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
        services.AddTransient(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IUnitOfWorkAsync, UnitOfWorkAsync>();
        services.AddTransient(typeof(IService<>), typeof(Service<>));
        services.AddTransient(typeof(IServiceAsync<>), typeof(ServiceAsync<>));
        services.AddTransient<ILogService, LogService>();
        services.AddTransient<IJwtService, JwtService>();
        services.AddTransient<IFileService, FileService>();
        services.AddTransient<IMailService, MailService>();

        services.Configure<JwtSetting>(configuration.GetSection("JwtSetting"));

        services.AddHttpClient();

        services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().Build()));

        services.AddEndpointsApiExplorer();

        var setting = new JwtSetting();

        configuration.Bind("JwtSetting", setting);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.IssuerSigningKey)),
                    ValidateIssuer = setting.ValidateIssuer,
                    ValidIssuer = setting.ValidIssuer,
                    ValidateAudience = setting.ValidateAudience,
                    ValidAudience = setting.ValidAudience,
                    RequireExpirationTime = setting.RequireExpirationTime,
                    ValidateLifetime = setting.ValidateLifetime,
                    ClockSkew = TimeSpan.FromDays(1)
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        try
                        {
                            var token = context.HttpContext.GetAuthenticationToken();

                            var handler = new JwtSecurityTokenHandler();

                            handler.ValidateToken(token, options.TokenValidationParameters, out _);

                            return Task.CompletedTask;
                        }
                        catch
                        {
                            throw new UnAuthorizedException(Statement.UnAuthorized);
                        }
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(JwtService.Administrator, builder => builder.RequireAssertion(context => context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value == JwtService.Administrator));
            options.AddPolicy(JwtService.Other, builder => builder.RequireAssertion(context => context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value == JwtService.Other));
        });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("docs", new OpenApiInfo
            {
                Title = "Farmina API",
                Version = "v1"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer xxx'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                }
            });

            const string xmlFile = "TwitchNightFall.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });
    }

    public static void UseApplications(this WebApplication application)
    {
        application.UseMiddleware<ExceptionHandler>();
        application.UseSwagger();
        application.UseSwaggerUI(options =>
        {
            options.DocumentTitle = "Farmina API Documentation";
            options.SwaggerEndpoint("/swagger/docs/swagger.json", "Farmina API");
            options.RoutePrefix = "docs";
        });
        application.UseCors("CorsPolicy");
        application.UseAuthentication();
        application.UseAuthorization();
        application.MapControllers();
    }
}