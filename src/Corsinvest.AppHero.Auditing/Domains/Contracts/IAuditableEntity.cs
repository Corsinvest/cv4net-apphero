/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
namespace Corsinvest.AppHero.Auditing.Domains.Contracts;

public interface IAuditableEntity
{
    string CreatedBy { get; set; }
    DateTime CreatedOn { get; set; }
    string? LastModifiedBy { get; set; }
    DateTime? LastModifiedOn { get; set; }
}