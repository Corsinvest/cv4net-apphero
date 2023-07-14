/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Authentication.Helper;
using Corsinvest.AppHero.Core.Extensions;
using Corsinvest.AppHero.Core.Security.Auth;
using Corsinvest.AppHero.Core.Security.Identity;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;

namespace Corsinvest.AppHero.Authentication.ActiveDirectory;

public class AutenticationActiveDirectory : IAutenticationActiveDirectory
{
    private readonly ILogger<AutenticationActiveDirectory> _logger;
    private readonly IOptionsSnapshot<Options> _options;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthenticationService _autentication;

    public AutenticationActiveDirectory(IOptionsSnapshot<Options> options,
                                        UserManager<ApplicationUser> usermanager,
                                        IAuthenticationService autentication,
                                        ILogger<AutenticationActiveDirectory> logger)
    {
        _logger = logger;
        _options = options;
        _userManager = usermanager;
        _autentication = autentication;
    }

    public async Task<IResult<bool>> LoginAsync(LoginRequestAD loginRequestAD)
    {
        IResult<bool> ret = Result.Ok(true);

        var settingDomain = _options.Value.Domains.FirstOrDefault(a => a.Domain == loginRequestAD.Domain)!;
        var userName = @$"{loginRequestAD.Domain}\{loginRequestAD.Username}";

        var userNameAHLogin = userName.Replace(@"\", ".").Replace(" ", "");
        var user = await _userManager.FindByNameAsync(userNameAHLogin);
        if (user == null && !settingDomain.AutoImportUser)
        {
            ret = Result.Fail<bool>("Autentication AutoImportUser disabled");
        }
        else if (user != null && !user.IsActive)
        {
            ret = Result.Fail<bool>("User is not active");
        }
        else
        {
            using var ldap = new LdapConnection();
            ldap.Connect(settingDomain.Hostname, settingDomain.Port);
            ret = ldap.TryLogin(userName, loginRequestAD.Password);
            if (ret.IsSuccess)
            {
                //get info user
                if (user == null)
                {
                    //user service for info
                    ret = ldap.TryLogin(@$"{settingDomain.Domain}\{settingDomain.ServiceUserName}", settingDomain.ServicePassword);
                    if (ret.IsSuccess)
                    {
                        var userLdap = ldap.Search(settingDomain.SearchBase, $"(&(objectClass=user)(sAMAccountName={loginRequestAD.Username}))", null!)
                                           .FirstOrDefault();
                        if (userLdap == null)
                        {
                            ret = Result.Fail<bool>("Error read Active Directory!");
                        }
                        else
                        {
                            user = new ApplicationUser
                            {
                                UserName = userNameAHLogin,
                                EmailConfirmed = true,
                                IsActive = true,
                                //ProfileImageUrl =,
                                DefaultCulture = "en-US",
                            };

                            var attrs = userLdap.GetAttributeSet();
                            if (attrs.TryGetValue("givenName", out var firstName)) { user.FirstName = firstName.StringValue; }
                            if (attrs.TryGetValue("sn", out var lastName)) { user.LastName = lastName.StringValue; }
                            if (attrs.TryGetValue("mail", out var mail))
                            {
                                user.Email = mail.StringValue;
                            }
                            else
                            {
                                //fuck mail
                                user.Email = $"{loginRequestAD.Username.Replace(" ", "")}@local.local";
                            }

                            //if (attrs.TryGetValue("memberOf", out var memberOf))
                            //{
                            //    //TODO memberOf  roles
                            //    var roles = memberOf.StringValueArray
                            //                        .Select(x => GetGroup(x))
                            //                        .Where(x => x != null)
                            //                        .Distinct()
                            //                        .ToArray();
                            //}

                            try
                            {
                                var identityResult = await _userManager.CreateAsync(user);
                                if (identityResult.Succeeded)
                                {
                                    var roles = (settingDomain.DefaultRolesNewUser + "").Split(",").Where(a => !string.IsNullOrEmpty(a));
                                    if (roles.Any())
                                    {
                                        identityResult = await _userManager.AddRolesToUserAsync(user, roles);
                                    }
                                }

                                if (!identityResult.Succeeded) { ret = Result.Fail<bool>(identityResult.Errors.Select(a => $"{a.Code} - {a.Description}")); }
                            }
                            catch (Exception ex)
                            {
                                ret = Result.Fail<bool>(ex.Message);
                            }

                        }
                    }

                    //redirect to login
                    if (ret.IsSuccess) { await _autentication.LoginAsync(loginRequestAD); }
                }
            }
        }

        ret.Errors.ForEach(a => _logger.LogInformation(a.Message));
        return ret;
    }

    //private string? GetGroup(string value)
    //{
    //    var match = Regex.Match(value, "^CN=([^,]*)");
    //    return match.Success
    //            ? match.Groups[1].Value
    //            : null;
    //}
}