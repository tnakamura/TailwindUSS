using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TailwindUSS.Editor
{
    /// <summary>
    /// Represents the tailwind uss settings provider.
    /// </summary>
    internal sealed class TailwindUssSettingsProvider : SettingsProvider
    {
        private readonly List<string> inputGlobs = new List<string>();
        private readonly List<ThemeEntry> colorEntries = new List<ThemeEntry>();
        private readonly List<ThemeEntry> spacingEntries = new List<ThemeEntry>();
        private readonly List<ThemeEntry> fontSizeEntries = new List<ThemeEntry>();
        private readonly List<ThemeEntry> fontEntries = new List<ThemeEntry>();
        private readonly List<ThemeEntry> backgroundImageEntries = new List<ThemeEntry>();
        private bool isLoaded;
        private bool isDirty;
        private string outputUssPath = string.Empty;
        private bool autoAttachGeneratedUss;
        private bool autoGenerateOnUxmlSave;
        private string statusMessage;
        private MessageType statusMessageType = MessageType.Info;

        /// <summary>
        /// Initializes a new instance of the <see cref="TailwindUssSettingsProvider"/> type.
        /// </summary>
        internal TailwindUssSettingsProvider(string path, SettingsScope scope)
            : base(path, scope)
        {
            label = "TailwindUSS";
            keywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "TailwindUSS",
                "USS",
                "UXML",
                "Tailwind"
            };
        }

        /// <summary>
        /// Gets the path where the settings appear in Unity's Project Settings window.
        /// </summary>
        internal static string SettingsPath
        {
            get { return "Project/TailwindUSS"; }
        }

        /// <summary>
        /// Creates a settings provider instance for Unity's settings system.
        /// </summary>
        [SettingsProvider]
        internal static SettingsProvider CreateSettingsProvider()
        {
            return new TailwindUssSettingsProvider(SettingsPath, SettingsScope.Project);
        }

        /// <summary>
        /// Opens Unity's Project Settings window to the TailwindUSS settings page.
        /// </summary>
        internal static void OpenProjectSettings()
        {
            SettingsService.OpenProjectSettings(SettingsPath);
        }

        /// <summary>
        /// Called when the settings page is activated and reloads the configuration.
        /// </summary>
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            Reload();
        }

        /// <summary>
        /// Renders the settings UI in Unity's Project Settings window.
        /// </summary>
        public override void OnGUI(string searchContext)
        {
            if (!isLoaded)
            {
                Reload();
            }

            EditorGUILayout.LabelField("TailwindUSS stores project settings in tailwinduss.config.json at the Unity project root.");
            EditorGUILayout.Space();

            if (!string.IsNullOrEmpty(statusMessage))
            {
                EditorGUILayout.HelpBox(statusMessage, statusMessageType);
                EditorGUILayout.Space();
            }

            if (isDirty)
            {
                EditorGUILayout.HelpBox("You have unsaved TailwindUSS settings changes.", MessageType.Warning);
                EditorGUILayout.Space();
            }

            if (GUILayout.Button("Reload"))
            {
                Reload();
            }

            if (GUILayout.Button("Save"))
            {
                Save();
            }

            EditorGUILayout.Space();
            DrawInputGlobsSection();
            outputUssPath = UpdateValue(outputUssPath, EditorGUILayout.TextField("Output USS Path", outputUssPath));
            autoAttachGeneratedUss = UpdateValue(autoAttachGeneratedUss, EditorGUILayout.Toggle("Auto Attach Generated USS", autoAttachGeneratedUss));
            autoGenerateOnUxmlSave = UpdateValue(autoGenerateOnUxmlSave, EditorGUILayout.Toggle("Auto Generate On UXML Save", autoGenerateOnUxmlSave));
            EditorGUILayout.Space();
            DrawThemeSection("Theme Colors", colorEntries, "Add Color");
            DrawThemeSection("Theme Spacing", spacingEntries, "Add Spacing Entry");
            DrawThemeSection("Theme Font Sizes", fontSizeEntries, "Add Font Size");
            DrawThemeSection("Theme Fonts", fontEntries, "Add Font");
            DrawThemeSection("Theme Background Images", backgroundImageEntries, "Add Background Image");
        }

        private void Reload()
        {
            statusMessage = null;
            statusMessageType = MessageType.Info;

            if (!ConfigLoader.TryLoadEditable(out var config, out var errorMessage, out var fileExists))
            {
                config = TailwindUssConfig.CreateDefault();
                statusMessage = string.Format("Failed to load '{0}': {1} Editing defaults in memory until you save.", ConfigLoader.FileName, errorMessage);
                statusMessageType = MessageType.Error;
            }
            else if (!fileExists)
            {
                statusMessage = string.Format("'{0}' was not found. Save from this page to create it.", ConfigLoader.FileName);
            }

            LoadFromConfig(config);
            isLoaded = true;
            isDirty = false;
        }

        private void Save()
        {
            try
            {
                ConfigLoader.WriteConfig(BuildConfig());
                AssetDatabase.Refresh();
                statusMessage = string.Format("Saved '{0}'.", ConfigLoader.FileName);
                statusMessageType = MessageType.Info;
                isDirty = false;
            }
            catch (Exception exception)
            {
                statusMessage = string.Format("Failed to save '{0}': {1}", ConfigLoader.FileName, exception.Message);
                statusMessageType = MessageType.Error;
                Debug.LogError(statusMessage);
            }
        }

        private void LoadFromConfig(TailwindUssConfig config)
        {
            inputGlobs.Clear();
            if (config.inputGlobs != null)
            {
                inputGlobs.AddRange(config.inputGlobs);
            }

            outputUssPath = config.outputUssPath ?? string.Empty;
            autoAttachGeneratedUss = config.autoAttachGeneratedUss;
            autoGenerateOnUxmlSave = config.autoGenerateOnUxmlSave;

            LoadEntries(colorEntries, config.theme == null ? null : config.theme.colors);
            LoadEntries(spacingEntries, config.theme == null ? null : config.theme.spacing);
            LoadEntries(fontSizeEntries, config.theme == null ? null : config.theme.fontSizes);
            LoadEntries(fontEntries, config.theme == null ? null : config.theme.fonts);
            LoadEntries(backgroundImageEntries, config.theme == null ? null : config.theme.backgroundImages);
        }

        private void DrawInputGlobsSection()
        {
            EditorGUILayout.LabelField("Input Globs");
            if (inputGlobs.Count == 0)
            {
                EditorGUILayout.HelpBox("If empty, TailwindUSS falls back to Assets/**/*.uxml.", MessageType.None);
            }

            for (var index = 0; index < inputGlobs.Count; index++)
            {
                var updatedValue = EditorGUILayout.TextField(string.Format("Pattern {0}", index + 1), inputGlobs[index] ?? string.Empty);
                if (!string.Equals(updatedValue, inputGlobs[index], StringComparison.Ordinal))
                {
                    inputGlobs[index] = updatedValue;
                    isDirty = true;
                }

                if (GUILayout.Button(string.Format("Remove Pattern {0}", index + 1)))
                {
                    inputGlobs.RemoveAt(index);
                    isDirty = true;
                    break;
                }
            }

            if (GUILayout.Button("Add Input Glob"))
            {
                inputGlobs.Add(string.Empty);
                isDirty = true;
            }

            EditorGUILayout.Space();
        }

        private void DrawThemeSection(string title, IList<ThemeEntry> entries, string addButtonLabel)
        {
            EditorGUILayout.LabelField(title);
            if (entries.Count == 0)
            {
                EditorGUILayout.HelpBox("No overrides configured.", MessageType.None);
            }

            for (var index = 0; index < entries.Count; index++)
            {
                var entry = entries[index];
                var updatedKey = EditorGUILayout.TextField("Key", entry.Key ?? string.Empty);
                var updatedValue = EditorGUILayout.TextField("Value", entry.Value ?? string.Empty);
                if (!string.Equals(updatedKey, entry.Key, StringComparison.Ordinal) ||
                    !string.Equals(updatedValue, entry.Value, StringComparison.Ordinal))
                {
                    entries[index] = new ThemeEntry(updatedKey, updatedValue);
                    isDirty = true;
                }

                if (GUILayout.Button(string.Format("Remove {0} Entry {1}", title, index + 1)))
                {
                    entries.RemoveAt(index);
                    isDirty = true;
                    break;
                }

                EditorGUILayout.Space();
            }

            if (GUILayout.Button(addButtonLabel))
            {
                entries.Add(new ThemeEntry(string.Empty, string.Empty));
                isDirty = true;
            }

            EditorGUILayout.Space();
        }

        private TailwindUssConfig BuildConfig()
        {
            return new TailwindUssConfig
            {
                inputGlobs = inputGlobs.ToArray(),
                outputUssPath = outputUssPath,
                autoAttachGeneratedUss = autoAttachGeneratedUss,
                autoGenerateOnUxmlSave = autoGenerateOnUxmlSave,
                theme = BuildTheme()
            };
        }

        private TailwindUssTheme BuildTheme()
        {
            var theme = new TailwindUssTheme
            {
                colors = BuildEntries(colorEntries),
                spacing = BuildEntries(spacingEntries),
                fontSizes = BuildEntries(fontSizeEntries),
                fonts = BuildEntries(fontEntries),
                backgroundImages = BuildEntries(backgroundImageEntries)
            };

            if (theme.colors == null &&
                theme.spacing == null &&
                theme.fontSizes == null &&
                theme.fonts == null &&
                theme.backgroundImages == null)
            {
                return null;
            }

            return theme;
        }

        private static void LoadEntries(ICollection<ThemeEntry> target, IDictionary<string, string> source)
        {
            target.Clear();
            if (source == null)
            {
                return;
            }

            foreach (var pair in source)
            {
                target.Add(new ThemeEntry(pair.Key, pair.Value));
            }
        }

        private static Dictionary<string, string> BuildEntries(IEnumerable<ThemeEntry> entries)
        {
            Dictionary<string, string> result = null;
            foreach (var entry in entries)
            {
                if (string.IsNullOrWhiteSpace(entry.Key) && string.IsNullOrWhiteSpace(entry.Value))
                {
                    continue;
                }

                if (result == null)
                {
                    result = new Dictionary<string, string>(StringComparer.Ordinal);
                }

                result[entry.Key ?? string.Empty] = entry.Value ?? string.Empty;
            }

            return result;
        }

        private string UpdateValue(string currentValue, string updatedValue)
        {
            if (!string.Equals(currentValue, updatedValue, StringComparison.Ordinal))
            {
                isDirty = true;
                return updatedValue;
            }

            return currentValue;
        }

        private bool UpdateValue(bool currentValue, bool updatedValue)
        {
            if (currentValue != updatedValue)
            {
                isDirty = true;
                return updatedValue;
            }

            return currentValue;
        }

        private readonly struct ThemeEntry
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ThemeEntry"/> struct with the specified key and value.
            /// </summary>
            public ThemeEntry(string key, string value)
            {
                Key = key;
                Value = value;
            }

            /// <summary>
            /// Gets the configuration key.
            /// </summary>
            public string Key { get; }

            /// <summary>
            /// Gets the configuration value.
            /// </summary>
            public string Value { get; }
        }
    }
}
