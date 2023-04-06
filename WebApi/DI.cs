using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using Microsoft.OpenApi.Models;

using WebApi.Auth;

using WebApi.Data;

using WebApi.Entities;
using WebApi.Services;
using WebApi.Providers;

namespace WebApi {
    public static class DI {
        public static IServiceCollection AddSwagger(this IServiceCollection services) { 
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(setup => {
                setup.SwaggerDoc("v1",
                    new OpenApiInfo {
                        Title = "My API - V1",
                        Version = "v1"
                    }
                );

                setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });

                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                 });
                var filePath = Path.Combine(AppContext.BaseDirectory, "WebApi.xml");
                setup.IncludeXmlComments(filePath);
            });


            return services;
        }

        public static IServiceCollection AddAuthenticationAndAuthorization(
            this IServiceCollection services,
            IConfiguration configuration) {

            services.AddScoped<IRequestUserProvider, RequestUserProvider>();

            services.AddIdentity<AppUser, IdentityRole>(setup => {

            }).AddEntityFrameworkStores<TodoDbContext>();

            services.AddScoped<IJwtService, JwtService>();

            var jwtConfig = new JwtConfig();
            configuration.GetSection("JWT").Bind(jwtConfig);
            services.AddSingleton(jwtConfig);

            // Add Authentication AFTER Identity
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, setup => {
                setup.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(0),
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret))
                };
            });

            services.AddAuthorization(options => {
                options.AddPolicy("CanTest", policy => {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new CanTestRequirement());
                });
            });

            return services;
        }

        public static IServiceCollection AddDomainDependencies(this IServiceCollection services) {
            services.AddScoped<ITodoService, TodoService>();

            return services;
        }


        public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration) {
            services.AddLogging();

            return services;
        }
    }
}
