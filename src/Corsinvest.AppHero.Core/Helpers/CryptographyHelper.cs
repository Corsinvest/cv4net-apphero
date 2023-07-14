/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using System.Security.Cryptography;
using System.Text;

namespace Corsinvest.AppHero.Core.Helpers;

public class CryptographyHelper
{
    private static readonly string Key = "012345678901234567890123";

    public static string EncryptString(string clearText, string? passphrase = null)
    {
        if (string.IsNullOrEmpty(clearText)) { return string.Empty; }

        var dataArray = Encoding.UTF8.GetBytes(clearText);

        using var tDes = TripleDES.Create();
        tDes.Mode = CipherMode.ECB;
        tDes.Key = Encoding.UTF8.GetBytes(passphrase ?? Key);
        tDes.Padding = PaddingMode.PKCS7;

        using var cTransform = tDes.CreateEncryptor();
        var resultArray = cTransform.TransformFinalBlock(dataArray, 0, dataArray.Length);
        tDes.Clear();

        return Convert.ToBase64String(resultArray);
    }

    public static string DecryptString(string encrypted, string? passphrase = null)
    {
        if (string.IsNullOrEmpty(encrypted)) { return string.Empty; }

        var resultArray = Array.Empty<byte>();

        try
        {
            var dataArray = Convert.FromBase64String(encrypted);

            using var tDes = TripleDES.Create();
            tDes.Mode = CipherMode.ECB;
            tDes.Key = Encoding.UTF8.GetBytes(passphrase ?? Key);
            tDes.Padding = PaddingMode.PKCS7;

            using var cTransform = tDes.CreateDecryptor();
            resultArray = cTransform.TransformFinalBlock(dataArray, 0, dataArray.Length);
            tDes.Clear();
        }
        catch (Exception ex) { }
        return Encoding.UTF8.GetString(resultArray);
    }
}
