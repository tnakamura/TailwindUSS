using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace TailwindUSS.Editor
{
    internal static class ConfigLoader
    {
        internal const string FileName = "tailwinduss.config.json";
        private static readonly JsonSerializerSettings WriteSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        internal static string GetProjectRoot()
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        }

        internal static string GetConfigPath()
        {
            return Path.Combine(GetProjectRoot(), FileName);
        }

        internal static bool TryLoad(out TailwindUssConfig config, out string errorMessage, out bool usedDefaultConfig)
        {
            var configPath = GetConfigPath();
            usedDefaultConfig = false;
            errorMessage = null;

            if (!File.Exists(configPath))
            {
                config = TailwindUssConfig.CreateDefault();
                Normalize(config);
                usedDefaultConfig = true;
                return true;
            }

            try
            {
                var json = File.ReadAllText(configPath);
                config = JsonConvert.DeserializeObject<TailwindUssConfig>(json);
                if (config == null)
                {
                    errorMessage = "Config JSON could not be parsed.";
                    return false;
                }

                if (!TryValidateThemeEntries(config.theme, out errorMessage))
                {
                    config = null;
                    return false;
                }

                Normalize(config);
                return true;
            }
            catch (Exception exception)
            {
                config = null;
                errorMessage = exception.Message;
                return false;
            }
        }

        internal static bool TryLoadEditable(out TailwindUssConfig config, out string errorMessage, out bool fileExists)
        {
            var configPath = GetConfigPath();
            fileExists = File.Exists(configPath);
            errorMessage = null;

            if (!fileExists)
            {
                config = TailwindUssConfig.CreateDefault();
                return true;
            }

            try
            {
                var json = File.ReadAllText(configPath);
                config = JsonConvert.DeserializeObject<TailwindUssConfig>(json);
                if (config == null)
                {
                    errorMessage = "Config JSON could not be parsed.";
                    return false;
                }

                if (!TryValidateThemeEntries(config.theme, out errorMessage))
                {
                    config = null;
                    return false;
                }

                return true;
            }
            catch (Exception exception)
            {
                config = null;
                errorMessage = exception.Message;
                return false;
            }
        }

        internal static void WriteDefaultConfig()
        {
            WriteConfig(TailwindUssConfig.CreateDefault());
        }

        internal static void WriteConfig(TailwindUssConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (!TryValidateThemeEntries(config.theme, out var errorMessage))
            {
                throw new InvalidOperationException(errorMessage);
            }

            var configPath = GetConfigPath();
            var json = JsonConvert.SerializeObject(CreateSerializableCopy(config), WriteSettings);
            File.WriteAllText(configPath, json + Environment.NewLine);
        }

        private static void Normalize(TailwindUssConfig config)
        {
            if (config.inputGlobs == null || config.inputGlobs.Length == 0)
            {
                config.inputGlobs = TailwindUssConfig.CreateDefault().inputGlobs;
            }

            if (string.IsNullOrWhiteSpace(config.outputUssPath))
            {
                config.outputUssPath = TailwindUssConfig.CreateDefault().outputUssPath;
            }

            config.theme = TailwindUssTheme.CreateMerged(config.theme);
        }

        private static bool TryValidateThemeEntries(TailwindUssTheme theme, out string errorMessage)
        {
            errorMessage = null;
            if (theme == null)
            {
                return true;
            }

            if (!TryValidateEntries("theme.colors", theme.colors, out errorMessage))
            {
                return false;
            }

            if (!TryValidateEntries("theme.spacing", theme.spacing, out errorMessage))
            {
                return false;
            }

            if (!TryValidateEntries("theme.fontSizes", theme.fontSizes, out errorMessage))
            {
                return false;
            }

            if (!TryValidateEntries("theme.fonts", theme.fonts, out errorMessage))
            {
                return false;
            }

            return TryValidateEntries("theme.backgroundImages", theme.backgroundImages, out errorMessage);
        }

        private static bool TryValidateEntries(string sectionName, IDictionary<string, string> entries, out string errorMessage)
        {
            errorMessage = null;
            if (entries == null)
            {
                return true;
            }

            foreach (var pair in entries)
            {
                if (string.IsNullOrWhiteSpace(pair.Key))
                {
                    errorMessage = string.Format("Config section '{0}' contains an empty key.", sectionName);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(pair.Value))
                {
                    errorMessage = string.Format("Config section '{0}' contains an empty value for '{1}'.", sectionName, pair.Key);
                    return false;
                }
            }

            return true;
        }

        private static TailwindUssConfig CreateSerializableCopy(TailwindUssConfig config)
        {
            return new TailwindUssConfig
            {
                inputGlobs = config.inputGlobs ?? Array.Empty<string>(),
                outputUssPath = config.outputUssPath ?? string.Empty,
                autoAttachGeneratedUss = config.autoAttachGeneratedUss,
                theme = CreateSerializableTheme(config.theme)
            };
        }

        private static TailwindUssTheme CreateSerializableTheme(TailwindUssTheme theme)
        {
            if (theme == null)
            {
                return null;
            }

            var serializableTheme = new TailwindUssTheme
            {
                colors = CloneEntries(theme.colors),
                spacing = CloneEntries(theme.spacing),
                fontSizes = CloneEntries(theme.fontSizes),
                fonts = CloneEntries(theme.fonts),
                backgroundImages = CloneEntries(theme.backgroundImages)
            };

            if (serializableTheme.colors == null &&
                serializableTheme.spacing == null &&
                serializableTheme.fontSizes == null &&
                serializableTheme.fonts == null &&
                serializableTheme.backgroundImages == null)
            {
                return null;
            }

            return serializableTheme;
        }

        private static Dictionary<string, string> CloneEntries(IDictionary<string, string> entries)
        {
            if (entries == null || entries.Count == 0)
            {
                return null;
            }

            var clone = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var pair in entries)
            {
                clone[pair.Key] = pair.Value;
            }

            return clone;
        }
    }
}
