/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Security.Identity;

public class TemplateOptions
{
    [DisplayName("Path ResetPassword confirm registration")]
    public string ConfirmRegistration { get; set; } = Path.Combine(ApplicationHelper.PathExecution, @"\Security\Identity\EmailTemplates\ConfirmRegistration.tpl");

    [DisplayName("Path ResetPassword template")]
    public string ResetPassword { get; set; } = Path.Combine(ApplicationHelper.PathExecution, @"\Security\Identity\EmailTemplates\ResetPassword.tpl");
}