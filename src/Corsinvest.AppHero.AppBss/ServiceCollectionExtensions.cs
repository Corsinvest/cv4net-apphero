/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.AppBss.Persistence;
using Corsinvest.AppHero.Auditing.Domains.Entities;
using Corsinvest.AppHero.Core.Domain.Repository;
using Corsinvest.AppHero.Core.Extensions;
using Corsinvest.AppHero.Core.Helpers;
using Corsinvest.AppHero.Core.Options;
using Corsinvest.AppHero.Core.Security.Identity;
using Hangfire;
using Hangfire.Storage.SQLite;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Corsinvest.AppHero.AppBss;

public static class ServiceCollectionExtensions
{
    public static void Customize(this IServiceCollection services)
    {
        var writableAppOptions = services.GetWritableOptions<AppOptions>();
        writableAppOptions.Value.Name = "AppHero";
        writableAppOptions.Value.Author = "Corsinvest Srl";
        writableAppOptions.Value.Icon = "<path id=\"path1\" d=\"M 1.353 0.022 C 1.353 0.044 0.044 1.357 0.022 1.357 C -0.007 1.357 -0.008 22.636 0.024 22.636 C 0.049 22.636 1.353 23.943 1.353 23.969 C 1.353 24 22.476 23.999 22.589 24 C 22.716 24.002 24.003 22.759 24 22.647 C 23.997 22.542 23.997 1.395 24 1.363 C 24.001 1.336 22.575 0.049 22.575 0.024 C 22.575 -0.008 1.353 -0.007 1.353 0.022 M 10.612 7.461 C 10.713 7.488 11.006 7.777 11.006 7.848 C 11.006 7.865 11.114 8.07 11.114 8.07 L 11.12 12.312 C 11.126 16.467 11.126 16.554 11.08 16.61 L 11.034 16.668 C 11.034 16.668 9.356 16.675 9.333 16.648 C 9.314 16.625 9.293 14.434 9.293 14.434 L 6.479 14.428 C 3.854 14.422 3.663 14.424 3.635 14.463 C 3.612 14.493 3.605 16.58 3.605 16.58 L 3.541 16.63 C 3.44 16.71 1.884 16.707 1.809 16.627 C 1.76 16.574 1.76 16.565 1.768 14.195 C 1.778 11.494 1.799 10.973 1.904 10.831 C 1.914 10.818 1.93 10.734 1.939 10.645 C 1.949 10.552 1.979 10.44 2.01 10.381 C 2.04 10.325 2.072 10.238 2.081 10.19 C 2.09 10.141 2.313 9.719 2.322 9.68 C 2.331 9.641 2.364 9.578 2.397 9.54 C 2.429 9.502 2.455 9.457 2.455 9.441 C 2.455 9.425 2.482 9.387 2.515 9.359 C 2.548 9.33 2.575 9.292 2.575 9.274 C 2.575 9.257 2.597 9.222 2.624 9.196 C 2.652 9.172 2.689 9.116 2.707 9.073 C 2.725 9.03 2.751 8.994 2.764 8.994 C 2.778 8.994 2.805 8.962 2.825 8.924 C 2.845 8.885 2.883 8.846 2.91 8.838 C 2.936 8.83 2.958 8.812 2.958 8.798 C 2.958 8.771 3.255 8.466 3.282 8.466 C 3.291 8.466 3.328 8.436 3.364 8.4 C 3.477 8.287 3.591 8.203 3.632 8.202 C 3.653 8.202 3.689 8.174 3.71 8.141 C 3.732 8.109 3.764 8.082 3.781 8.082 C 3.799 8.082 3.832 8.06 3.855 8.034 C 3.879 8.009 3.936 7.975 3.981 7.96 C 4.027 7.944 4.085 7.913 4.108 7.889 C 4.132 7.866 4.187 7.838 4.231 7.829 C 4.275 7.819 4.336 7.792 4.366 7.768 C 4.396 7.744 4.456 7.717 4.498 7.708 C 4.54 7.699 4.6 7.671 4.63 7.647 C 4.661 7.623 4.736 7.596 4.798 7.587 C 4.86 7.578 4.962 7.55 5.026 7.525 C 5.089 7.501 5.238 7.47 5.357 7.458 C 5.476 7.446 5.588 7.423 5.607 7.407 C 5.679 7.347 10.431 7.337 10.431 7.337 M 14.65 7.383 L 14.706 7.429 L 14.73 11.071 C 14.73 11.071 20.311 11.079 20.341 11.071 L 20.395 11.058 C 20.395 11.058 20.403 7.414 20.42 7.384 C 20.459 7.31 22.094 7.311 22.168 7.385 C 22.217 7.434 22.246 16.524 22.197 16.598 C 22.147 16.672 20.465 16.675 20.433 16.642 C 20.399 16.608 20.388 13.049 20.38 13.008 L 20.365 12.933 L 14.718 12.933 L 14.718 16.561 L 14.601 16.679 L 12.968 16.679 L 12.85 16.561 L 12.862 7.429 L 12.919 7.383 C 13.006 7.312 14.562 7.312 14.65 7.383 M 5.984 9.209 C 5.916 9.233 5.766 9.266 5.652 9.281 C 5.537 9.297 5.42 9.324 5.392 9.342 C 5.364 9.36 5.293 9.388 5.233 9.405 C 5.174 9.421 5.109 9.449 5.088 9.466 C 5.068 9.484 5.014 9.51 4.968 9.526 C 4.923 9.541 4.867 9.573 4.843 9.598 C 4.82 9.622 4.772 9.649 4.736 9.658 C 4.7 9.667 4.651 9.7 4.627 9.731 C 4.603 9.762 4.573 9.787 4.559 9.787 C 4.546 9.787 4.044 10.303 4.027 10.372 C 4.019 10.406 3.985 10.462 3.952 10.498 C 3.919 10.533 3.892 10.58 3.892 10.602 C 3.892 10.624 3.867 10.677 3.835 10.719 C 3.804 10.762 3.771 10.842 3.762 10.898 C 3.753 10.953 3.73 11.016 3.709 11.039 C 3.662 11.092 3.597 12.57 3.597 12.57 L 3.655 12.584 C 3.686 12.592 9.293 12.585 9.293 12.585 L 9.306 9.162 C 9.306 9.162 6.1 9.167 5.984 9.209\" stroke=\"none\" fill=\"#239cec\" fill-rule=\"evenodd\" />";
        writableAppOptions.Value.Url = "https://www.corsinvest.it";
        writableAppOptions.Update(writableAppOptions.Value);
    }

    private static IServiceCollection ConfigureHangfire(this IServiceCollection services)
    {
        services.AddHangfireServer();
        GlobalConfiguration.Configuration.UseStorage(new SQLiteStorage(Path.Combine(ApplicationHelper.PathData, "hangfire.db")));
        return services;
    }

    private static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        //set options
        services.AddIdentity<ApplicationUser, ApplicationRole>(options => services.GetOptionsSnapshot<Core.Security.Identity.Options>()!.Value.SetIdentityOptions(options))
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

        return services;
    }

    private static IServiceCollection ConfigureDbApp(this IServiceCollection services)
    {
        //application db
        services.AddDbContext<ApplicationDbContext>(builder =>
        {
#if DEBUG
            builder.EnableDetailedErrors();
            builder.EnableSensitiveDataLogging();
#endif
            builder.UseSqlite(DataBaseHelper.CreateSQLitePath(Path.Combine(ApplicationHelper.PathData, "app.db")));
        })
        .AddRepository(typeof(ApplicationDbContext), typeof(AuditTrail));

        return services;
    }

    public static IServiceCollection ConfigureApp(this IServiceCollection services)
        => services.ConfigureHangfire()
                   .ConfigureDbApp()
                   .ConfigureIdentity();
}