/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Domain.Models;
using MailKit.Security;

namespace Corsinvest.AppHero.Core.Emailing;

public class Options : Credential
{
    [Required] public string From { get; set; } = string.Empty;

    [Display(Name = "SMTP Host")]
    [Required] public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 25;
    [Required] public string DisplayName { get; set; } = string.Empty;
    public SecureSocketOptions SslOptions { get; set; } = SecureSocketOptions.Auto;
}