using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

namespace TailwindUSS.Editor
{
    internal static class ConfigLoader
    {
        internal const string FileName = "tailwinduss.config.json";
        private static readonly JsonSerializerOptions ReadOptions = new JsonSerializerOptions
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = false
        };

        private static readonly JsonSerializerOptions WriteOptions = new JsonSerializerOptions
        {
            IncludeFields = true,
            PropertyNamingPolicy = null,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
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
                config = JsonSerializer.Deserialize<TailwindUssConfig>(json, ReadOptions);
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

        internal static void WriteDefaultConfig()
        {
            var configPath = GetConfigPath();
            var json = JsonSerializer.Serialize(TailwindUssConfig.CreateDefault(), WriteOptions);
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
    }
}
