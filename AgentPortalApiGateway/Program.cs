using System.Text;
using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Eureka;

namespace AgentPortalApiGateway;

public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args)
    {
        var signingKey = Environment.GetEnvironmentVariable("Jwt__SigningKey");
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException("Set Jwt__SigningKey before starting the API gateway.");
        }

        var key = Encoding.UTF8.GetBytes(signingKey);

        return WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config
                    .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true,
                        true)
                    .AddJsonFile("ocelot.json", false, false)
                    .AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices(s =>
            {
                s.AddCors();
                s.AddAuthentication(x =>
                    {
                        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer("ApiSecurity", x =>
                    {
                        x.RequireHttpsMetadata = false;
                        x.SaveToken = true;
                        x.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                    });
                s.AddOcelot().AddEureka().AddCacheManager(x => x.WithDictionaryHandle());
            })
            .Configure(a =>
            {
                var appSettings = new AppSettings();
                a.ApplicationServices.GetService<IConfiguration>()
                    .GetSection("AppSettings")
                    .Bind(appSettings);

                a.UseCors
                (b => b
                    .WithOrigins(appSettings.AllowedChatOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
                a.UseOcelot().Wait();
            })
            .Build();
    }
}
