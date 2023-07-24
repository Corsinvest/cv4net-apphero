/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */

// Ignore Spelling: Requeue

using Corsinvest.AppHero.Core.BackgroundJob;
using Hangfire;
using System.Linq.Expressions;
using BackgroundJobHelper = Hangfire.BackgroundJob;

namespace Corsinvest.AppHero.HangFire;

public class JobService : IJobService
{
    public bool Delete(string jobId) => BackgroundJobHelper.Delete(jobId);
    public bool Delete(string jobId, string fromState) => BackgroundJobHelper.Delete(jobId, fromState);

    public string Enqueue(Expression<Func<Task>> methodCall) => BackgroundJobHelper.Enqueue(methodCall);
    public string Enqueue<T>(Expression<Action<T>> methodCall) => BackgroundJobHelper.Enqueue(methodCall);
    public string Enqueue(Expression<Action> methodCall) => BackgroundJobHelper.Enqueue(methodCall);
    public string Enqueue<T>(Expression<Func<T, Task>> methodCall) => BackgroundJobHelper.Enqueue(methodCall);
    public bool Requeue(string jobId) => BackgroundJobHelper.Requeue(jobId);
    public bool Requeue(string jobId, string fromState) => BackgroundJobHelper.Requeue(jobId, fromState);

    public string Schedule(Expression<Action> methodCall, TimeSpan delay) => BackgroundJobHelper.Schedule(methodCall, delay);
    public string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay) => BackgroundJobHelper.Schedule(methodCall, delay);
    public string Schedule(Expression<Action> methodCall, DateTimeOffset enqueueAt) => BackgroundJobHelper.Schedule(methodCall, enqueueAt);
    public string Schedule(Expression<Func<Task>> methodCall, DateTimeOffset enqueueAt) => BackgroundJobHelper.Schedule(methodCall, enqueueAt);
    public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay) => BackgroundJobHelper.Schedule(methodCall, delay);
    public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay) => BackgroundJobHelper.Schedule(methodCall, delay);
    public string Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt) => BackgroundJobHelper.Schedule(methodCall, enqueueAt);
    public string Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt) => BackgroundJobHelper.Schedule(methodCall, enqueueAt);

    //Recurring Job
    public void Schedule(string recurringJobId, Expression<Action> methodCall, string cronExpression, TimeZoneInfo timezone)
        => RecurringJob.AddOrUpdate(recurringJobId,
                                    methodCall,
                                    cronExpression,
                                    new RecurringJobOptions() { TimeZone = timezone });

    public void Schedule(string recurringJobId, Expression<Func<Task>> methodCall, string cronExpression, TimeZoneInfo timezone)
        => RecurringJob.AddOrUpdate(recurringJobId,
                                    methodCall,
                                    cronExpression,
                                    new RecurringJobOptions() { TimeZone = timezone });

    public void Schedule<T>(string recurringJobId, Expression<Action<T>> methodCall, string cronExpression, TimeZoneInfo timezone)
        => RecurringJob.AddOrUpdate(recurringJobId,
                                    methodCall,
                                    cronExpression,
                                    new RecurringJobOptions() { TimeZone = timezone });

    public void Schedule<T>(string recurringJobId, Expression<Func<T, Task>> methodCall, string cronExpression, TimeZoneInfo timezone)
        => RecurringJob.AddOrUpdate(recurringJobId,
                                    methodCall,
                                    cronExpression,
                                    new RecurringJobOptions()
                                    {
                                        TimeZone = timezone ?? TimeZoneInfo.Local
                                    });

    public void RemoveIfExists(string recurringJobId) => RecurringJob.RemoveIfExists(recurringJobId);
    public void TriggerJob(string recurringJobId) => RecurringJob.TriggerJob(recurringJobId);
}