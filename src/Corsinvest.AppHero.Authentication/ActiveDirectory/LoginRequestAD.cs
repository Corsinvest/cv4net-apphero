/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Security.Identity;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.AppHero.Authentication.ActiveDirectory;

public class LoginRequestAD : LoginRequest
{
    [Required]
    public string Domain { get; set; } = default!;
}
