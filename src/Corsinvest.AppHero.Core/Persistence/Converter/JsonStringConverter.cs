/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Corsinvest.AppHero.Core.Persistence.Converter;

public class JsonStringConverter<T> : ValueConverter<T, string> where T : class
{
    public JsonStringConverter() : base(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<T>(v)!) { }
}