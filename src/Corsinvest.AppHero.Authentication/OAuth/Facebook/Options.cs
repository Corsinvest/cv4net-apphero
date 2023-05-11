/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Helpers;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.AppHero.Authentication.OAuth.Facebook;

public class Options : OptionsBase
{
    public string AppId { get; set; } = default!;

    [Browsable(false)]
    public string AppSecretEncrypted { get; set; } = default!;

    [JsonIgnore]
    [DataType(DataType.Password)]
    public string AppSecret
    {
        get => CryptographyHelper.DecryptString(AppSecretEncrypted);
        set => AppSecretEncrypted = CryptographyHelper.EncryptString(value);
    }
}