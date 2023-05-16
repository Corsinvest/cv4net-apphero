/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.AppBss;
using Corsinvest.AppHero.AppBss.Persistence;
using Corsinvest.AppHero.Core;
using Corsinvest.AppHero.Core.Extensions;
using Corsinvest.AppHero.Core.Helpers;
using Corsinvest.AppHero.Core.SoftwareRelease;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Connections;
using Serilog;

//appsetting default
var appSetting = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
if (!File.Exists(appSetting) || new FileInfo(appSetting).Length == 0)
{
    File.WriteAllText(appSetting, File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Default.json")));
}

var builder = WebApplication.CreateBuilder(args);

//configure serilog
builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

var logger = builder.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Main");

logger.LogInformation("Start App....");

builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(ApplicationHelper.PathData, "data-protection-keys")));
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options =>
                {
                    options.DetailedErrors = true;
                })
                .AddHubOptions(options =>
                {
                    options.EnableDetailedErrors = false;
                //    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
                //    options.HandshakeTimeout = TimeSpan.FromSeconds(30);
                })
                .AddCircuitOptions(option =>
                {
                    option.DetailedErrors = true;
                });

builder.Services.ConfigureApp();

builder.Services.AddMvc();
builder.Services.AddControllers();

builder.Services.AddAppHero(builder.Configuration);
builder.Services.Customize();

builder.Services.AddReleaseGitHub()
                .AddReleaseDockerHub();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//migration
app.DatabaseMigrate<ApplicationDbContext>();

await app.OnPreApplicationInitializationAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    #region Swagger
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", $"AppHero V1");
    });
    #endregion
}

app.UseHttpsRedirection();

app.UseRouting();

await app.OnApplicationInitializationAsync();

app.MapBlazorHub(configureOptions: options =>
{
    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
});

app.MapFallbackToPage("/_Index");

await app.OnPostApplicationInitializationAsync();

await app.RunAsync();