﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using SMCEC = Sucrose.Manager.Converter.EnumConverter;
using SMCIPAC = Sucrose.Manager.Converter.IPAddressConverter;
using SMHR = Sucrose.Manager.Helper.Reader;
using SMHV = Sucrose.Manager.Helper.Validator;
using SMHW = Sucrose.Manager.Helper.Writer;
using SMMRF = Sucrose.Memory.Manage.Readonly.Folder;
using SMMRG = Sucrose.Memory.Manage.Readonly.General;
using SMMRP = Sucrose.Memory.Manage.Readonly.Path;

namespace Sucrose.Manager
{
    public class SettingManager
    {
        private Settings _settings = new();
        private readonly string _settingsFilePath;
        private DateTime _lastWrite = DateTime.Now;
        private readonly JsonSerializerSettings _serializerSettings;

        public SettingManager(string settingsFileName, Formatting formatting = Formatting.Indented, TypeNameHandling typeNameHandling = TypeNameHandling.None)
        {
            _settingsFilePath = Path.Combine(SMMRP.ApplicationData, SMMRG.AppName, SMMRF.Setting, settingsFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(_settingsFilePath));

            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = typeNameHandling,
                Formatting = formatting,
                Converters =
                {
                    new SMCEC(),
                    new SMCIPAC(),
                    //new StringEnumConverter()
                }
            };

            ControlFile();
        }

        public T GetSetting<T>(string key, T back = default)
        {
            try
            {
                if (CheckFile())
                {
                    if (File.GetLastWriteTime(_settingsFilePath) > _lastWrite)
                    {
                        string json = SMHR.ReadStream(_settingsFilePath);

                        _settings = JsonConvert.DeserializeObject<Settings>(json, _serializerSettings);

                        _lastWrite = DateTime.Now;
                    }

                    if (_settings != null && _settings.Properties != null && _settings.Properties.TryGetValue(key, out object value))
                    {
                        return ConvertToType<T>(value);
                    }
                }
            }
            catch { }

            return back;
        }

        public T GetSettingStable<T>(string key, T back = default)
        {
            try
            {
                if (CheckFile())
                {
                    if (File.GetLastWriteTime(_settingsFilePath) > _lastWrite)
                    {
                        string json = SMHR.ReadStream(_settingsFilePath);

                        _settings = JsonConvert.DeserializeObject<Settings>(json, _serializerSettings);

                        _lastWrite = DateTime.Now;
                    }

                    if (_settings != null && _settings.Properties != null && _settings.Properties.TryGetValue(key, out object value))
                    {
                        return JsonConvert.DeserializeObject<T>(value.ToString());
                    }
                }
            }
            catch { }

            return back;
        }

        public T GetSettingAddress<T>(string key, T back = default)
        {
            try
            {
                if (CheckFile())
                {
                    if (File.GetLastWriteTime(_settingsFilePath) > _lastWrite)
                    {
                        string json = SMHR.ReadStream(_settingsFilePath);

                        _settings = JsonConvert.DeserializeObject<Settings>(json, _serializerSettings);

                        _lastWrite = DateTime.Now;
                    }

                    if (_settings != null && _settings.Properties != null && _settings.Properties.TryGetValue(key, out object value))
                    {
                        return ConvertToType<T>(value);
                    }
                }
            }
            catch { }

            return back;
        }

        public void SetSetting<T>(string key, T value)
        {
            SetSetting(new KeyValuePair<string, T>[]
            {
                new(key, value)
            });
        }

        public void SetSetting<T>(KeyValuePair<string, T>[] pairs)
        {
            try
            {
                if (CheckFile())
                {
                    _settings = JsonConvert.DeserializeObject<Settings>(ReadSetting(), _serializerSettings);
                }
                else
                {
                    _settings = new Settings();
                }

                foreach (KeyValuePair<string, T> pair in pairs)
                {
                    _settings.Properties[pair.Key] = ConvertToType<T>(pair.Value);
                }

                SMHW.WriteStream(_settingsFilePath, JsonConvert.SerializeObject(_settings, _serializerSettings));

                _lastWrite = DateTime.Now;
            }
            catch { }
        }

        public string ReadSetting()
        {
            try
            {
                return SMHR.ReadStream(_settingsFilePath);
            }
            catch
            {
                return string.Empty;
            }
        }

        public void ApplySetting()
        {
            try
            {
                _settings = new();

                SMHW.WriteStream(_settingsFilePath, JsonConvert.SerializeObject(_settings, _serializerSettings));

                _lastWrite = DateTime.Now;
            }
            catch { }
        }

        public bool CheckFile()
        {
            return File.Exists(_settingsFilePath);
        }

        public string SettingFile()
        {
            return _settingsFilePath;
        }

        private void ControlFile()
        {
            if (CheckFile())
            {
                string json = ReadSetting();

                if (string.IsNullOrEmpty(json))
                {
                    ApplySetting();
                }
                else if (!SMHV.Json(json))
                {
                    ApplySetting();
                }
                else
                {
                    try
                    {
                        _settings = JsonConvert.DeserializeObject<Settings>(json, _serializerSettings);

                        if (_settings != null && _settings.Properties != null)
                        {
                            _lastWrite = DateTime.Now;
                        }
                        else
                        {
                            ApplySetting();
                        }
                    }
                    catch
                    {
                        ApplySetting();
                    }
                }
            }
            else
            {
                ApplySetting();
            }
        }

        private T ConvertToType<T>(object value)
        {
            Type type = typeof(T);

            if (type == typeof(IPAddress))
            {
                return (T)(object)IPAddress.Parse(value.ToString());
            }
            else if (type == typeof(Uri))
            {
                return (T)(object)new Uri(value.ToString());
            }
            else if (type == typeof(Guid))
            {
                return (T)(object)Guid.Parse(value.ToString());
            }
            else if (type.IsEnum)
            {
                return (T)Enum.Parse(type, value.ToString());
            }
            else if (type == typeof(KeyValuePair<string, string>))
            {
                string[] parts = value.ToString().Split(':');

                return (T)(object)new KeyValuePair<string, string>(parts[0].Trim(), parts[1].Trim());
            }
            else if (type == typeof(string[]))
            {
                if (value is string[] array)
                {
                    return (T)(object)array;
                }
                else if (value is JArray jArray)
                {
                    return (T)(object)jArray.Select(jValue => (string)jValue).ToArray();
                }
            }
            else if (type == typeof(List<string>))
            {
                if (value is List<string> list)
                {
                    return (T)(object)list;
                }
                else if (value is JArray jArray)
                {
                    return (T)(object)jArray.Select(jValue => (string)jValue).ToList();
                }
            }
            else if (type == typeof(Dictionary<string, string>))
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(value.ToString());
                }
                catch
                {
                    return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(value));
                }
            }

            return (T)value;
        }

        private class Settings
        {
            public Dictionary<string, object> Properties { get; set; } = new();
        }
    }
}