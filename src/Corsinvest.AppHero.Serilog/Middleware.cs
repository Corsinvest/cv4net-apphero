/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;

namespace Corsinvest.AppHero.Serilog;

public class Middleware : IMiddleware
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public Middleware(IServiceScopeFactory serviceScopeFactory) => _serviceScopeFactory = serviceScopeFactory;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var currentUserService = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
        //var multiTenantService = scope.ServiceProvider.GetService<IMultiTenantService>();

        var enrichers = new ILogEventEnricher[]
        {
            new PropertyEnricher("UserId", currentUserService.UserId),
            new PropertyEnricher("UserName", currentUserService.UserName),
            new PropertyEnricher("IP", currentUserService.IpAddress),
            //new PropertyEnricher("TenantId", multiTenantService?.TenantId ?? "")
        };

        using (LogContext.Push(enrichers))
        {
            await next(context);
        }
    }
}