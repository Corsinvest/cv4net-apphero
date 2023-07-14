/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;

namespace Corsinvest.AppHero.HangFire;

public class JobActivatorEx : JobActivator
{
    private readonly IServiceScopeFactory _scopeFactory;

    public JobActivatorEx(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    public override JobActivatorScope BeginScope(PerformContext context) => new Scope(context, _scopeFactory.CreateScope());

    class Scope : JobActivatorScope, IServiceProvider
    {
        private readonly PerformContext _context;
        private readonly IServiceScope _scope;

        public Scope(PerformContext context, IServiceScope scope)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));

            ReceiveParameters();
        }

        private void ReceiveParameters()
        {
            //var tenantInfo = _context.GetJobParameter<FSHTenantInfo>(MultitenancyConstants.TenantIdName);
            //if (tenantInfo is not null)
            //{
            //    _scope.ServiceProvider.GetRequiredService<IMultiTenantContextAccessor>()
            //        .MultiTenantContext = new MultiTenantContext<FSHTenantInfo>
            //        {
            //            TenantInfo = tenantInfo
            //        };
            //}

            //var userId = _context.GetJobParameter<string>(QueryStringKeys.UserId);
            //if (!string.IsNullOrEmpty(userId))
            //{
            //    //    _scope.ServiceProvider.GetRequiredService<ICurrentUserInitializer>()
            //    //        .SetCurrentUserId(userId);
            //}
        }

        public override object Resolve(Type type) => ActivatorUtilities.GetServiceOrCreateInstance(this, type);

        object? IServiceProvider.GetService(Type serviceType)
            => serviceType == typeof(PerformContext)
                ? _context
                : _scope.ServiceProvider.GetService(serviceType);
    }
}