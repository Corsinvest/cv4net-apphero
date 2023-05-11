/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Helpers;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.AppHero.Authentication.ActiveDirectory;

public class SettingDomain : OptionsBase
{
    public bool Default { get; set; } = true;
    public bool Enabled { get; set; }
    public string Name { get; set; } = default!;
    public string Hostname { get; set; } = default!;
    public int Port { get; set; } = 389;
    public string Domain { get; set; } = default!;
    public string SearchBase { get; set; } = default!;
    public string ServiceUserName { get; set; } = default!;

    [JsonIgnore]
    [DataType(DataType.Password)]
    public string ServicePassword
    {
        get => CryptographyHelper.DecryptString(ServicePasswordEncrypted);
        set => ServicePasswordEncrypted = CryptographyHelper.EncryptString(value);
    }

    [Browsable(false)]
    public string ServicePasswordEncrypted { get; set; } = default!;

    public override string ToString() => Name;
}