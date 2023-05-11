/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
global using Corsinvest.AppHero.Core.BaseUI.DataManager;
global using Corsinvest.AppHero.Core.BaseUI.Shared.Components;
global using Corsinvest.AppHero.Core.Domain.Contracts;
global using Corsinvest.AppHero.Core.Extensions;
global using Corsinvest.AppHero.Core.Helpers;
global using Corsinvest.AppHero.Core.Modularity;
global using Corsinvest.AppHero.Core.MudBlazorUI.Extensions;
global using Corsinvest.AppHero.Core.Options;
global using Corsinvest.AppHero.Core.Security.Auth;
global using Corsinvest.AppHero.Core.Security.Identity;
global using Corsinvest.AppHero.Core.UI;
global using Corsinvest.AppHero.Core.Validators;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Forms;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using MudBlazor;