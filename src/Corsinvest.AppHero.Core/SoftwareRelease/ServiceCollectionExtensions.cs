/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */

namespace Corsinvest.AppHero.Core.SoftwareRelease;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReleaseGitHub(this IServiceCollection services)
    {
        if (!ApplicationHelper.InDocker)
        {
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(ServiceCollectionExtensions));
            logger.LogInformation("Add GitHubReleaseService");

            services.AddScoped<IReleaseService, GitHubReleaseService>();
        }

        return services;
    }

    public static IServiceCollection AddReleaseDockerHub(this IServiceCollection services)
    {
        if (ApplicationHelper.InDocker)
        {
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(ServiceCollectionExtensions));
            logger.LogInformation("Add DockerHubReleaseService");

            services.AddScoped<IReleaseService, DockerHubReleaseService>();
        }

        return services;
    }
}