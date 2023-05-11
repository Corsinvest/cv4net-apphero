/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Corsinvest.AppHero.Core.Options;

internal class WritableOptionsService<T> : IWritableOptionsService<T> where T : class, new()
{
    private readonly IOptionsMonitor<T> _options;
    private readonly IConfigurationRoot _configuration;
    private readonly string _section;
    private readonly string _fileName;

    public WritableOptionsService(IOptionsMonitor<T> options, IConfigurationRoot configuration, string section, string fileName)
    {
        _options = options;
        _configuration = configuration;
        _section = section;
        _fileName = fileName;

        //not exists create
        if (!File.Exists(_fileName) || new FileInfo(fileName).Length == 0) { Write(new JObject(), new T()); }
    }

    public T Value => _options.CurrentValue;
    public T Get(string name) => _options.Get(name);

    public void Update(Action<T> applyChanges)
    {
        var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(_fileName))!;
        var sectionObject = jObject.TryGetValue(_section, out var section)
                                ? JsonConvert.DeserializeObject<T>(section.ToString())!
                                : (Value ?? new T())!;

        applyChanges(sectionObject);
        Write(jObject, sectionObject);
    }

    public void Update(T applyChanges) => Write(JsonConvert.DeserializeObject<JObject>(File.ReadAllText(_fileName))!, applyChanges);

    public void Write(JObject jObject, T options)
    {
        jObject[_section] = JObject.Parse(JsonConvert.SerializeObject(options));
        File.WriteAllText(_fileName, JsonConvert.SerializeObject(jObject, Formatting.Indented));
        _configuration.Reload();
    }
}