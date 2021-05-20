using System;
using System.Text;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OnlineStore.Modules.Identity.Application.Features.Permissions.Services;
using OnlineStore.Modules.Identity.Application.Features.Roles.Services;
using OnlineStore.Modules.Identity.Application.Features.Users.Contracts;
using OnlineStore.Modules.Identity.Domain.Common;
using OnlineStore.Modules.Identity.Domain.Configurations.Options;
using OnlineStore.Modules.Identity.Domain.Configurations.Settings;
using OnlineStore.Modules.Identity.Domain.Users;
using OnlineStore.Modules.Identity.Domain.Users.Services;
using OnlineStore.Modules.Identity.Domain.Users.Types;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Permissions;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Roles.Services;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Models;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Repositories;
using OnlineStore.Modules.Identity.Infrastructure.Domain.Users.Services;

namespace OnlineStore.Modules.Identity.Infrastructure.Extensions
{
    public static class IdentityServiceExtensions
    {
        internal static IServiceCollection AddIdentityServices(this IServiceCollection services,
            IConfiguration configuration,
            Action<AuthorizationOptions> setupAction = null)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<IdentityDbContext>(options =>
                    options.UseInMemoryDatabase("OnlineStore"));
            }
            else
            {
                services.AddDbContext<IdentityDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("IdentityConnection"),
                        b => b.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));
            }


            services.TryAddScoped<IUserNameResolver, UserNameResolver>();
            services.TryAddScoped<IPermissionService, PermissionService>();
            services.TryAddScoped<IRoleSearchService, RoleSearchService>();
            services.TryAddTransient<IUserRepository, UserRepository>();

            //Identity dependencies override
            services.TryAddScoped<RoleManager<ApplicationRole>, CustomRoleManager>();
            services.TryAddSingleton<Func<RoleManager<ApplicationRole>>>(provider =>
                () => provider.CreateScope().ServiceProvider.GetService<RoleManager<ApplicationRole>>());


            services.TryAddScoped<UserManager<ApplicationUser>, CustomUserManager>();
            services.TryAddSingleton<Func<UserManager<ApplicationUser>>>(provider =>
                () => provider.CreateScope().ServiceProvider.GetService<UserManager<ApplicationUser>>());

            services.TryAddSingleton<Func<SignInManager<ApplicationUser>>>(provider =>
                () => provider.CreateScope().ServiceProvider.GetService<SignInManager<ApplicationUser>>());

            //Use custom ClaimsPrincipalFactory to add system roles claims for user principal
            services.TryAddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomUserClaimsPrincipalFactory>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            if (setupAction != null) services.Configure(setupAction);

            //some dependencies will add here if not registered before
            services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.Stores.MaxLengthForKeys = 128)
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });

            services.Configure<IdentityOptions>(configuration.GetSection("IdentityOptions"));
            services.Configure<UserOptionsExtended>(configuration.GetSection("IdentityOptions:User"));

            return services;
        }


        internal static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JWTSettings"));
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = configuration["JWTSettings:Issuer"],
                        ValidAudience = configuration["JWTSettings:Audience"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                    };
                    o.Events = new JwtBearerEvents()
                    {
                        OnAuthenticationFailed = c =>
                        {
                            c.NoResult();
                            c.Response.StatusCode = 500;
                            c.Response.ContentType = "text/plain";
                            return c.Response.WriteAsync(c.Exception.ToString());
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            var result =
                                JsonConvert.SerializeObject(
                                    new Response<string>(
                                        "You are not Authorized - 401 Not authorized")); // or -->   var result = JsonConvert.SerializeObject("401 Not authorized");
                            return context.Response.WriteAsync(result);
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(
                                new Response<string>(
                                    "You are not authorized to access this resource - 403 Not authorized")); // or -->  var result = JsonConvert.SerializeObject("403 Not authorized");
                            return context.Response.WriteAsync(result);
                        },
                    };
                });
            return services;
        }
    }
}