/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using System.Linq.Expressions;

namespace Corsinvest.AppHero.Core.BackgroundJob;

public interface IJobService
{
    bool Delete(string jobId);
    bool Delete(string jobId, string fromState);
    string Enqueue(Expression<Action> methodCall);
    string Enqueue(Expression<Func<Task>> methodCall);
    string Enqueue<T>(Expression<Action<T>> methodCall);
    string Enqueue<T>(Expression<Func<T, Task>> methodCall);

    string Schedule(Expression<Action> methodCall, DateTimeOffset enqueueAt);
    string Schedule(Expression<Action> methodCall, TimeSpan delay);
    string Schedule(Expression<Func<Task>> methodCall, DateTimeOffset enqueueAt);
    string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay);
    string Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt);
    string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);
    string Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt);
    string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay);

    void Schedule(string recurringJobId, Expression<Action> methodCall, string cronExpression, TimeZoneInfo timezone);
    void Schedule(string recurringJobId, Expression<Func<Task>> methodCall, string cronExpression, TimeZoneInfo timezone);
    void Schedule<T>(string recurringJobId, Expression<Action<T>> methodCall, string cronExpression, TimeZoneInfo timezone);
    void Schedule<T>(string recurringJobId, Expression<Func<T, Task>> methodCall, string cronExpression, TimeZoneInfo timezone);

    void RemoveIfExists(string recurringJobId);
    void TriggerJob(string recurringJobId);
}