using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Common.Core.Modules;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StackifyLib;

namespace Common.Authentication.Jwt
{
    internal static class Extensions
    {
        private const string SectionName = "jwt";

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
            IConfiguration configuration,
            IList<IModule> modules,
            string sectionName = SectionName,
            Action<JwtBearerOptions> optionsFactory = null)
        {
            var options = configuration.GetSection(sectionName).Get<JwtOptions>();
            services.AddSingleton<IJwtHandler, JwtHandler>();
            services.AddSingleton<IAccessTokenService, InMemoryAccessTokenService>();
            services.AddScoped<AccessTokenValidatorMiddleware>();

            if (options.AuthenticationDisabled)
                services.AddSingleton<IPolicyEvaluator, DisabledAuthenticationPolicyEvaluator>();

            var tokenValidationParameters = new TokenValidationParameters
            {
                RequireAudience = options.RequireAudience,
                ValidIssuer = options.ValidIssuer,
                ValidIssuers = options.ValidIssuers,
                ValidateActor = options.ValidateActor,
                ValidAudience = options.ValidAudience,
                ValidAudiences = options.ValidAudiences,
                ValidateAudience = options.ValidateAudience,
                ValidateIssuer = options.ValidateIssuer,
                ValidateLifetime = options.ValidateLifetime,
                ValidateTokenReplay = options.ValidateTokenReplay,
                ValidateIssuerSigningKey = options.ValidateIssuerSigningKey,
                SaveSigninToken = options.SaveSigninToken,
                RequireExpirationTime = options.RequireExpirationTime,
                RequireSignedTokens = options.RequireSignedTokens,
                ClockSkew = TimeSpan.Zero
            };

            if (!string.IsNullOrWhiteSpace(options.AuthenticationType))
                tokenValidationParameters.AuthenticationType = options.AuthenticationType;

            var hasCertificate = false;
            if (options.Certificate is { })
            {
                X509Certificate2 certificate = null;
                var password = options.Certificate.Password;
                var hasPassword = !string.IsNullOrWhiteSpace(password);
                if (!string.IsNullOrWhiteSpace(options.Certificate.Location))
                {
                    certificate = hasPassword
                        ? new X509Certificate2(options.Certificate.Location, password)
                        : new X509Certificate2(options.Certificate.Location);
                    var keyType = certificate.HasPrivateKey ? "with private key" : "with public key only";
                    Log.Information($"Loaded X.509 certificate from location: '{options.Certificate.Location}' {keyType}.");
                }

                if (!string.IsNullOrWhiteSpace(options.Certificate.RawData))
                {
                    var rawData = Convert.FromBase64String(options.Certificate.RawData);
                    certificate = hasPassword
                        ? new X509Certificate2(rawData, password)
                        : new X509Certificate2(rawData);
                    var keyType = certificate.HasPrivateKey ? "with private key" : "with public key only";
                    Log.Information($"Loaded X.509 certificate from raw data {keyType}.");
                }

                if (certificate is { })
                {
                    if (string.IsNullOrWhiteSpace(options.Algorithm)) options.Algorithm = SecurityAlgorithms.RsaSha256;

                    hasCertificate = true;
                    tokenValidationParameters.IssuerSigningKey = new X509SecurityKey(certificate);
                    var actionType = certificate.HasPrivateKey ? "issuing" : "validating";
                    Log.Information($"Using X.509 certificate for {actionType} tokens.");
                }
            }

            if (!string.IsNullOrWhiteSpace(options.IssuerSigningKey) && !hasCertificate)
            {
                if (string.IsNullOrWhiteSpace(options.Algorithm) || hasCertificate)
                    options.Algorithm = SecurityAlgorithms.HmacSha256;

                var rawKey = Encoding.UTF8.GetBytes(options.IssuerSigningKey);
                tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(rawKey);
                Log.Information("Using symmetric encryption for issuing tokens.");
            }

            if (!string.IsNullOrWhiteSpace(options.NameClaimType))
                tokenValidationParameters.NameClaimType = options.NameClaimType;

            if (!string.IsNullOrWhiteSpace(options.RoleClaimType))
                tokenValidationParameters.RoleClaimType = options.RoleClaimType;

            services
                .AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.Authority = options.Authority;
                    o.Audience = options.Audience;
                    o.MetadataAddress = options.MetadataAddress;
                    o.SaveToken = options.SaveToken;
                    o.RefreshOnIssuerKeyNotFound = options.RefreshOnIssuerKeyNotFound;
                    o.RequireHttpsMetadata = options.RequireHttpsMetadata;
                    o.IncludeErrorDetails = options.IncludeErrorDetails;
                    o.TokenValidationParameters = tokenValidationParameters;
                    if (!string.IsNullOrWhiteSpace(options.Challenge)) o.Challenge = options.Challenge;

                    optionsFactory?.Invoke(o);
                });

            services.AddSingleton(options);
            services.AddSingleton(tokenValidationParameters);

            var policies = modules?.SelectMany(x => x.Policies ?? Enumerable.Empty<string>()) ??
                           Enumerable.Empty<string>();
            services.AddAuthorization(authorization =>
            {
                foreach (var policy in policies)
                {
                    authorization.AddPolicy(policy, x => x.RequireClaim("permissions", policy));
                }
            });

            return services;
        }

        public static IApplicationBuilder UseAccessTokenValidator(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AccessTokenValidatorMiddleware>();
        }
    }
}