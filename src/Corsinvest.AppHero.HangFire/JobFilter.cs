/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Hangfire.Client;
using Hangfire.Logging;

namespace Corsinvest.AppHero.HangFire;

public class JobFilter : IClientFilter
{
    private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();
    private readonly IServiceProvider _serviceProvider;

    public JobFilter(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public void OnCreating(CreatingContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        //Logger.InfoFormat("Set TenantId and UserId parameters to job {0}.{1}...",
        //                  context.Job.Method.ReflectedType?.FullName,
        //                  context.Job.Method.Name);

        //using var scope = _services.CreateScope();

        //var httpContext = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>()?.HttpContext;
        //  _ = httpContext ?? throw new InvalidOperationException("Can't create a info without HttpContext.");

        //var tenantInfo = scope.ServiceProvider.GetRequiredService<ITenantInfo>();
        //context.SetJobParameter(MultitenancyConstants.TenantIdName, tenantInfo);

        //var userId = httpContext.User.GetUserId();
        //context.SetJobParameter(QueryStringKeys.UserId, userId);
    }

    public void OnCreated(CreatedContext context)
        => Logger.InfoFormat("Job created with parameters {0}",
                             context.Parameters.Select(a => a.Key + "=" + a.Value)
                                               .Aggregate((s1, s2) => s1 + ";" + s2));
}