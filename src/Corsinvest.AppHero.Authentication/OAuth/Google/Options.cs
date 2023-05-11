/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Helpers;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.AppHero.Authentication.OAuth.Google;

public class Options : OptionsBase
{
    public string ClientId { get; set; } = default!;

    [Browsable(false)]
    public string ClientSecretEncrypted { get; set; } = default!;

    [JsonIgnore]
    [DataType(DataType.Password)]
    public string ClientSecret
    {
        get => CryptographyHelper.DecryptString(ClientSecretEncrypted);
        set => ClientSecretEncrypted = CryptographyHelper.EncryptString(value);
    }
}