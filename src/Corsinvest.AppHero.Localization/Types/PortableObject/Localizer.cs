/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */

//namespace Corsinvest.AppHero.Localization.Types.PortableObject;

//public class Localizer : BaseStringLocalizer
//{
//    private readonly ILogger<Localizer> _logger;
//    private readonly string _path;
//    private readonly JsonSerializer _serializer = new();

//    public Localizer(IDistributedCache cache,
//                               IOptions<Settings> settings,
//                               Type resourceType,
//                               ILoggerFactory loggerFactory)
//        : base(cache, settings, resourceType, loggerFactory)
//    {
//        _logger = loggerFactory.CreateLogger<Localizer>();

//        if (!Directory.Exists(settings.Value.ResourcesPath)) { Directory.CreateDirectory(settings.Value.ResourcesPath); }
//        _path = Path.GetFullPath($"{settings.Value.ResourcesPath}/{CurrentCultureName}.po");

//        //if create file if not exist with key
//        if (Settings.Value.CreateKeyIfNotExists && !File.Exists(_path))
//        {
//            File.WriteAllText(_path, JsonConvert.SerializeObject(new(), Formatting.Indented));
//        }
//    }

//    public override IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
//    {
//        using var fs = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
//        using var sr = new StreamReader(fs);
//        using var jtr = new JsonTextReader(sr);

//        MoveToContext(jtr);

//        while (jtr.Read())
//        {
//            if (jtr.TokenType != JsonToken.PropertyName) { continue; }
//            var key = jtr.Value + "";
//            jtr.Read();
//            var value = _serializer.Deserialize<string>(jtr);
//            yield return new LocalizedString(key, value!, false);
//        }
//    }

//    protected override string GetString(string key)
//        => File.Exists(_path)
//                ? base.GetString(key)
//                : default!;

//    private void MoveToContext(JsonTextReader jtr)
//    {
//        if (Settings.Value.SplitResourceByType)
//        {
//            while (jtr.Read())
//            {
//                if (jtr.TokenType == JsonToken.PropertyName && (string)jtr.Value! == Context)
//                {
//                    break;
//                }
//            }
//        }
//    }

//    protected override string GetStringImp(string key)
//    {
//        if (key == null) { throw new ArgumentNullException(nameof(key)); }

//        using var fs = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
//        using var sr = new StreamReader(fs);
//        using var jtr = new JsonTextReader(sr);

//        MoveToContext(jtr);

//        while (jtr.Read())
//        {
//            if (jtr.TokenType == JsonToken.PropertyName && (string)jtr.Value! == key)
//            {
//                jtr.Read();
//                return _serializer.Deserialize<string>(jtr)!;
//            }
//        }

//        _logger.LogWarning("Localization not exist! Culture: '{CurrentCultureName}', Context: '{Context}', Key: '{key}'",
//                           CurrentCultureName,
//                           Context,
//                           key);

//        if (CreateKeyIfNotExists)
//        {
//            var context = "";

//            fs.Close();
//            var data = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(_path))!;
//            if (Settings.Value.SplitResourceByType)
//            {
//                context = Context;
//                if (!data.ContainsKey(Context)) { data.Add(Context, new JObject()); }
//                ((JObject)data[Context]!).Add(key, key);
//            }
//            else
//            {
//                data.Add(key, key);
//            }

//            _logger.LogInformation("New resource added. Culture: '{CurrentCultureName}', Context: '{context}', Key: '{key}'",
//                                   CurrentCultureName,
//                                   context,
//                                   key);

//            File.WriteAllText(_path, JsonConvert.SerializeObject(data, Formatting.Indented));
//        }

//        return key;
//    }
//}