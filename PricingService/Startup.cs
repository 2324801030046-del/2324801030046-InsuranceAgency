using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FluentValidation;
using FluentValidation.AspNetCore;
using Newtonsoft.Json;
using PricingService.DataAccess.Marten;
using PricingService.Init;
using Steeltoe.Discovery.Client;

namespace PricingService;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        try
        {
            services.AddDiscoveryClient(Configuration);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Warning] Discovery client registration skipped: {ex.Message}");
        }

        services.AddControllers()
            .AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            });
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<CalculatePriceCommandValidator>();

        var connectionString = Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Set ConnectionStrings__DefaultConnection before starting PricingService.");
        services.AddMartenDataStore(connectionString);

        services.AddPricingDemoInitializer();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly));
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();

        try
        {
            app.UseInitializer();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Info] Initializer notice: {ex.Message}");
        }

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
