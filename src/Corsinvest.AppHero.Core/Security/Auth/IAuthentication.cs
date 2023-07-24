/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Modularity;

namespace Corsinvest.AppHero.Core.Security.Auth;

public interface IAuthentication : IGroupableModule
{
    string IGroupableModule.GetGroupName() => "Autentication";
    string IGroupableModule.GetGroupIcon() => UI.UIIcon.Key.GetName();

    AuthenticationType AuthenticationType { get; }
}