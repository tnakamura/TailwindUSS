using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TailwindUSS.Editor
{
    /// <summary>
    /// Provides the tailwind uss menu functionality.
    /// </summary>
    internal static class TailwindUssMenu
    {
        [MenuItem("Tools/TailwindUSS/Generate")]
        private static void Generate()
        {
            new GenerationService().Generate();
        }

        [MenuItem("Tools/TailwindUSS/Validate")]
        private static void Validate()
        {
            new ValidationService().Validate();
        }

        [MenuItem("Tools/TailwindUSS/Create Default Config")]
        private static void CreateDefaultConfig()
        {
            var configPath = ConfigLoader.GetConfigPath();
            if (File.Exists(configPath))
            {
                var shouldOverwrite = EditorUtility.DisplayDialog(
                    "TailwindUSS",
                    string.Format("'{0}' already exists. Overwrite it?", ConfigLoader.FileName),
                    "Overwrite",
                    "Cancel");

                if (!shouldOverwrite)
                {
                    return;
                }
            }

            try
            {
                ConfigLoader.WriteDefaultConfig();
                AssetDatabase.Refresh();
                Debug.Log(string.Format("TailwindUSS default config created at '{0}'.", configPath));
            }
            catch (Exception exception)
            {
                Debug.LogError(string.Format("Failed to create TailwindUSS config: {0}", exception.Message));
            }
        }

        [MenuItem("Tools/TailwindUSS/Settings")]
        private static void OpenSettings()
        {
            TailwindUssSettingsProvider.OpenProjectSettings();
        }
    }
}
