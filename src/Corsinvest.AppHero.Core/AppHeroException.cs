/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using System.Runtime.Serialization;

namespace Corsinvest.AppHero.Core;

public class AppHeroException : Exception
{
    public AppHeroException() { }
    public AppHeroException(string message) : base(message) { }
    public AppHeroException(string message, Exception innerException) : base(message, innerException) { }
    public AppHeroException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }
}
