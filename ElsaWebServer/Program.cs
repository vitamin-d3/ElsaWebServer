
using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Elsa.Features.Services;
using Elsa.Workflows.LogPersistence;

namespace ElsaWebServer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddElsa(AddElsa);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        app.UseWorkflowsApi();
        //// Elsa HTTP Endpoint activities.
        app.UseWorkflows();

        app.Run();
    }

    private static void AddElsa(IModule elsa)
    {
        elsa.UseWorkflows(workflows =>
        {
        })
        .UseWorkflowManagement(management =>
        {
            management.UseEntityFrameworkCore(ef =>
            {
                ef.UseContextPooling = true;
                ef.UseSqlite(sp => "Data Source=workflow.db");

                ef.RunMigrations = true;
            });
            management.SetDefaultLogPersistenceMode(LogPersistenceMode.Inherit);
            management.UseReadOnlyMode(false);
        })
        .UseWorkflowRuntime(runtime =>
        {
            runtime.UseEntityFrameworkCore(ef =>
            {
                ef.UseContextPooling = true;
                ef.UseSqlite(sp => "Data Source=workflow.db");

                ef.RunMigrations = true;
            });
        })
        .UseWorkflowsApi(api =>
        {
            api.AddFastEndpointsAssembly<Program>();
        });
        elsa.AddFastEndpointsAssembly<Program>();
    }
}
