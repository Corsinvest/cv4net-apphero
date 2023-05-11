/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.AppHero.Localization.Types.Database.Models;

public class Localization : IAggregateRoot<int>
{
    [Key] public int Id { get; set; }
    [Required] public string Key { get; set; } = default!;
    public string? Context { get; set; }
    public string Value { get; set; } = default!;
    [Required] public string CultureName { get; set; } = default!;
    public bool Enabled { get; set; } = true;
}