/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.DependencyInjection;

namespace Corsinvest.AppHero.Core.Emailing;

public interface IEmailTemplateService : ITransientDependency
{
    string GenerateEmailTemplate<T>(string templateName, T mailTemplateModel);
    string GenerateEmail<T>(string template, T mailTemplateModel);
}