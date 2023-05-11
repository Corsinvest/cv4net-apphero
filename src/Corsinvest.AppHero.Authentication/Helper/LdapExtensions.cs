/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using FluentResults;
using Novell.Directory.Ldap;

namespace Corsinvest.AppHero.Authentication.Helper;

public static class LdapExtensions
{
    public static List<LdapEntry> Search(this LdapConnection connection,
                                         string searchBase,
                                         string filter,
                                         string[] attributes)
    {
        var ret = new List<LdapEntry>();
        var results = connection.Search(searchBase, LdapConnection.ScopeSub, filter, attributes, false);
        while (results.HasMore())
        {
            try
            {
                var data = results.Next();
                if (data != null) { ret.Add(data); }
            }
            catch
            {
                continue;
            }
        }

        return ret;
    }

    public static IResult<bool> TryLogin(this LdapConnection connection, string username, string password)
        => Result.Try(() =>
        {
            connection.Bind(username, password);
            return connection.Connected;
        });
}