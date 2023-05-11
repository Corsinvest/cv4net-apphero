/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Auditing.Domains.Entities;
using Corsinvest.AppHero.Core.BaseUI.DataManager;
using Microsoft.AspNetCore.Components;

namespace Corsinvest.AppHero.Auditing.MudBlazorUI;

public partial class RenderIndex
{
    [Inject] protected IDataGridManagerRepository<AuditTrail> DataGridManager { get; set; } = default!;

    protected override void OnInitialized()
    {
        DataGridManager.Title = L["Audit Trails"];
        DataGridManager.DefaultSort = new() { [nameof(AuditTrail.Id)] = true };
    }
}