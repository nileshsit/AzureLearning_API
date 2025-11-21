using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Reflection;
using System.Text;

namespace AzureLearning.API.Configuration
{
    public static class OpenApiConfig
    {
        public static void AddOpenApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var key = Encoding.UTF8.GetBytes(Convert.ToString(configuration["AppSettings:JWT_Secret"]));

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
                options.AddApiVersionParametersWhenVersionNeutral = true;
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(option =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    option.SwaggerDoc(description.GroupName, new OpenApiInfo
                    {
                        Title = $"Azure Learning API {description.ApiVersion}",
                        Version = description.ApiVersion.ToString(),
                        Description = "Azure Learning .NET 10 Core Web API"
                    });
                }

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    option.IncludeXmlComments(xmlPath);
                }

                option.EnableAnnotations();

                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("bearer", document)] = []
                });
            });
        }

        public static void UseOpenApiConfiguration(this WebApplication app)
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            //if (app.Environment.IsDevelopment())
            //{
                app.MapOpenApi();

                // Swagger UI
                app.UseSwagger(opt =>
                {
                    opt.RouteTemplate = "openapi/{documentName}.json";
                });

                app.UseSwaggerUI(options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/openapi/{description.GroupName}.json", description.GroupName.ToUpperInvariant());
                    }
                    options.RoutePrefix = "swagger";
                });

                // Scalar UI
                app.MapScalarApiReference(options =>
                {
                    options
                        .WithTitle("Azure Learning API - Scalar")
                        .WithTheme(ScalarTheme.BluePlanet)
                        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                });
            //}
            // ReDoc UI
            foreach (var description in provider.ApiVersionDescriptions)
            {
                app.UseReDoc(options =>
                {
                    options.RoutePrefix = $"redoc/{description.GroupName}";
                    options.DocumentTitle = $"Azure Learning API - ReDoc {description.GroupName.ToUpperInvariant()}";
                    options.SpecUrl($"/openapi/{description.GroupName}.json");
                });
            }
        }
    }
}
