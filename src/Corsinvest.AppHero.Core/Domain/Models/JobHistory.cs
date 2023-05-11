/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Domain.Models;

public class JobHistory
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Duration => (End - Start).ToString("hh':'mm':'ss");
    public bool Status { get; set; }
    public string Log { get; set; } = default!;
}