﻿/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;

namespace Corsinvest.AppHero.Core.Security.Identity;

public class MemoryCacheTicketStore : ITicketStore
{
    private const string KeyPrefix = "AuthSessionStore-";
    private readonly int _loginCookieExpirationHours;
    private readonly MemoryCache _cache;

    public MemoryCacheTicketStore(int loginCookieExpirationHours)
    {
        _loginCookieExpirationHours = loginCookieExpirationHours;
        _cache = new MemoryCache(new MemoryCacheOptions());
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var guid = Guid.NewGuid();
        var key = KeyPrefix + guid.ToString();
        await RenewAsync(key, ticket);
        return key;
    }

    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        var options = new MemoryCacheEntryOptions();
        var expiresUtc = ticket.Properties.ExpiresUtc;
        if (expiresUtc.HasValue) { options.SetAbsoluteExpiration(expiresUtc.Value); }
        options.SetSlidingExpiration(TimeSpan.FromHours(_loginCookieExpirationHours));

        _cache.Set(key, ticket, options);

        return Task.FromResult(0);
    }

    public Task<AuthenticationTicket> RetrieveAsync(string key)
    {
        _cache.TryGetValue(key, out AuthenticationTicket? ticket);
        return Task.FromResult(ticket);
    }

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        return Task.FromResult(0);
    }
}