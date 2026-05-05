using System;
using System.IO;
using UnityEngine;

namespace TailwindUSS.Editor
{
    internal static class ConfigLoader
    {
        internal const string FileName = "tailwinduss.config.json";

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
                usedDefaultConfig = true;
                return true;
            }

            try
            {
                var json = File.ReadAllText(configPath);
                config = JsonUtility.FromJson<TailwindUssConfig>(json);
                if (config == null)
                {
                    errorMessage = "Config JSON could not be parsed.";
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
            var json = JsonUtility.ToJson(TailwindUssConfig.CreateDefault(), true);
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
        }
    }
}
