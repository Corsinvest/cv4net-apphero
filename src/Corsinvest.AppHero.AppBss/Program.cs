/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.AppBss;
using Corsinvest.AppHero.AppBss.Persistence;
using Corsinvest.AppHero.Core;
using Corsinvest.AppHero.Core.Extensions;
using Microsoft.AspNetCore.Http.Connections;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//configure serilog
builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

// Add services to the container.
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