/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.DependencyInjection;

namespace Corsinvest.AppHero.Core.Emailing;

public interface IMailService : ITransientDependency
{
    public static string TemplateLayout { get; } = Path.Combine(ApplicationHelper.PathExecution, @"\Emailing\Templates\Layout.tpl");
    public static string TemplateMessage { get; } = Path.Combine(ApplicationHelper.PathExecution, @"\Emailing\Templates\Message.tpl");

    Task SendAsync(MailRequest request);
}