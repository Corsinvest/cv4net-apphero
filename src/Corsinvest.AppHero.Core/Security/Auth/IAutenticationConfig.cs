/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Core.Security.Auth;

public interface IAutenticationConfig
{
    AutenticationType AutenticationType { get; }
}
