/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Extensions;
using Corsinvest.AppHero.Core.Security.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Corsinvest.AppHero.Core.BaseUI.Pages.Authentication;

[AllowAnonymous]
public abstract class SigninPageBase : AHComponentBase
{
    [Inject] private IModularityService ModularityService { get; set; } = default!;
    [Inject] private IOptionsSnapshot<Core.Security.Identity.Options> IdentityOptions { get; set; } = default!;
    [Inject] private IOptionsSnapshot<AppOptions> AppOptions { get; set; } = default!;

    protected IEnumerable<ModuleBase> GetExternalProviders(AutenticationType type)
        => ModularityService.Modules
                            .Implements<IAutentication>()
                            .IsEnabled()
                            .IsConfigured()
                            .Where(a => ((IAutenticationConfig)a).AutenticationType == type);

    protected class LoginDef
    {
        public string Icon { get; set; } = default!;
        public string Description { get; set; } = default!;
        public Type Render { get; set; } = default!;
        public override string ToString() => Description;
    }

    protected abstract Type GetDefaultLoginRender();

    protected IEnumerable<LoginDef> GetLogins()
    {
        var ret = new List<LoginDef>();

        if (IdentityOptions.Value.EnableLocalLogin)
        {
            ret.Add(new()
            {
                Icon = AppOptions.Value.Icon,
                Description = AppOptions.Value.Name,
                Render = GetDefaultLoginRender()
            });
        }

        ret.AddRange(GetExternalProviders(AutenticationType.Inline)
                        .Where(a => a.Link?.Render != null)
                        .Select(a => new LoginDef
                        {
                            Icon = a.Icon,
                            Description = a.Description,
                            Render = a.Link?.Render!
                        }));

        return ret;
    }
}