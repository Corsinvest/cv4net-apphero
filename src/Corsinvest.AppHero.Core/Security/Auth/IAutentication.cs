/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Modularity;

namespace Corsinvest.AppHero.Core.Security.Auth;

public interface IAutentication : IGroupableService
{
    string IGroupableService.GetGroupName() => "Autentication";
    string IGroupableService.GetGroupIcon() => UI.UIIcon.Key.GetName();

    AutenticationType AutenticationType { get; }
}