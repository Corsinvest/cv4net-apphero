/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Validators;

namespace Corsinvest.AppHero.Core.Domain.Models;

public abstract class JobSchedule
{
    public bool Enabled { get; set; }

    [Required]
    [DisplayName("Cron Schedule")]
    [CronExpressionValidator]
    public string CronExpression { get; set; } = default!;

    [Required]
    public string Description { get; set; } = default!;

    [NotMapped]
    public DateTimeOffset? NextRunTime => Enabled ? CronHelper.NextOccurrence(CronExpression) : null;

    [NotMapped]
    [DisplayName("Cron Descriptor")]
    public string CronExpressionDescriptor => CronHelper.GetDescription(CronExpression);

    [NotMapped]
    public bool CronExpressionIsValid => CronHelper.IsValid(CronExpression);
}