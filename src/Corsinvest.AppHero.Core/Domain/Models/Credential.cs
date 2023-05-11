/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Newtonsoft.Json;

namespace Corsinvest.AppHero.Core.Domain.Models;

public class Credential
{
    public string Username { get; set; } = default!;

    [Browsable(false)]
    public string PasswordEncrypted{ get; set; }=default!;

    [JsonIgnore]
    [DataType(DataType.Password)]
    public string Password
    {
        get => CryptographyHelper.DecryptString(PasswordEncrypted);
        set => PasswordEncrypted = CryptographyHelper.EncryptString(value);
    }
}