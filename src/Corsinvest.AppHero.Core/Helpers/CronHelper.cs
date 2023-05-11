/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using CronExpressionDescriptor;
using Cronos;

namespace Corsinvest.AppHero.Core.Helpers;

public class CronHelper
{
    public static DateTimeOffset? NextOccurrence(string cronExpression)
        => IsValid(cronExpression)
                ? CronExpression.Parse(cronExpression).GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local)!.Value
                : null;

    public static bool IsValid(string cronExpression)
    {
        try
        {
            var schedule = CronExpression.Parse(cronExpression);
            return true;
        }
        catch { }
        return false;
    }

    public static string GetDescription(string cronExpression)
    {
        var ret = "";
        if (IsValid(cronExpression))
        {
            try
            {
                ret = ExpressionDescriptor.GetDescription(cronExpression);
            }
            catch
            {

                ret = ExpressionDescriptor.GetDescription(CronExpression.Parse(cronExpression).ToString());
            }
        }
        return ret;
    }
}