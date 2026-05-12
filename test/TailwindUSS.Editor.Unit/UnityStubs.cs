using System;
using System.Collections.Generic;
using System.Text.Json;

namespace UnityEngine
{
    /// <summary>
    /// Represents a stub implementation of Unity's <see cref="Object"/> type for unit tests.
    /// </summary>
    public class Object
    {
    }

    /// <summary>
    /// Provides a stub implementation of Unity's <see cref="Application"/> API for unit tests.
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// Gets or sets the data path.
        /// </summary>
        public static string dataPath { get; set; } = string.Empty;
    }

    /// <summary>
    /// Provides JSON utility functionality for unit tests.
    /// </summary>
    public static class JsonUtility
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            IncludeFields = true
        };

        private static readonly JsonSerializerOptions IndentedOptions = new JsonSerializerOptions
        {
            IncludeFields = true,
            WriteIndented = true
        };

        /// <summary>
        /// Deserializes a JSON string to an object of type <typeparamref name="T"/>.
        /// </summary>
        public static T FromJson<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, Options);
        }

        /// <summary>
        /// Serializes an object to a JSON string.
        /// </summary>
        public static string ToJson(object obj, bool prettyPrint)
        {
            return JsonSerializer.Serialize(obj, prettyPrint ? IndentedOptions : Options);
        }
    }

    /// <summary>
    /// Provides debug logging functionality for unit tests.
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// Represents a captured debug log entry for test verification.
        /// </summary>
        public sealed class LogEntry
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LogEntry"/> type.
            /// </summary>
            public LogEntry(string level, string message, Object context)
            {
                Level = level;
                Message = message;
                Context = context;
            }

            /// <summary>
            /// Gets the level.
            /// </summary>
            public string Level { get; private set; }
            /// <summary>
            /// Gets the message.
            /// </summary>
            public string Message { get; private set; }
            /// <summary>
            /// Gets the context.
            /// </summary>
            public Object Context { get; private set; }
        }

        private static readonly List<LogEntry> entries = new List<LogEntry>();

        /// <summary>
        /// Gets the captured log entries.
        /// </summary>
        public static IReadOnlyList<LogEntry> Entries
        {
            get { return entries; }
        }

        /// <summary>
        /// Clears all captured log entries.
        /// </summary>
        public static void Reset()
        {
            entries.Clear();
        }

        /// <summary>
        /// Captures a log message for test verification.
        /// </summary>
        public static void Log(object message)
        {
            entries.Add(new LogEntry("Log", message == null ? string.Empty : message.ToString(), null));
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        public static void LogWarning(object message)
        {
            entries.Add(new LogEntry("Warning", message == null ? string.Empty : message.ToString(), null));
        }

        /// <summary>
        /// Logs a warning message with context.
        /// </summary>
        public static void LogWarning(object message, Object context)
        {
            entries.Add(new LogEntry("Warning", message == null ? string.Empty : message.ToString(), context));
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        public static void LogError(object message)
        {
            entries.Add(new LogEntry("Error", message == null ? string.Empty : message.ToString(), null));
        }

        /// <summary>
        /// Logs an error message with context.
        /// </summary>
        public static void LogError(object message, Object context)
        {
            entries.Add(new LogEntry("Error", message == null ? string.Empty : message.ToString(), context));
        }
    }
}

namespace UnityEditor
{
    using System.Collections.Generic;
    using UnityEngine.UIElements;
    using UnityEngine;

    /// <summary>
    /// Represents a menu item attribute for Unity Editor menu commands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class MenuItem : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItem"/> type.
        /// </summary>
        public MenuItem(string menuItem)
        {
            MenuPath = menuItem;
        }

        /// <summary>
        /// Gets the menu path.
        /// </summary>
        public string MenuPath { get; private set; }
    }

    /// <summary>
    /// Provides the asset database functionality.
    /// </summary>
    public static class AssetDatabase
    {
        /// <summary>
        /// Gets or sets the refresh call count.
        /// </summary>
        public static int RefreshCallCount { get; private set; }
        /// <summary>
        /// Gets the list of asset paths loaded through the stub.
        /// </summary>
        public static IList<string> LoadedAssetPaths { get; } = new List<string>();

        /// <summary>
        /// Resets the refresh call count and clears loaded asset paths.
        /// </summary>
        public static void Reset()
        {
            RefreshCallCount = 0;
            LoadedAssetPaths.Clear();
        }

        /// <summary>
        /// Increments the refresh call count for test verification.
        /// </summary>
        public static void Refresh()
        {
            RefreshCallCount++;
        }

        /// <summary>
        /// Loads an asset at the specified path.
        /// </summary>
        public static T LoadAssetAtPath<T>(string assetPath) where T : Object, new()
        {
            LoadedAssetPaths.Add(assetPath);
            return new T();
        }
    }

    /// <summary>
    /// Provides the editor utility functionality.
    /// </summary>
    public static class EditorUtility
    {
        /// <summary>
        /// Represents the dialog call.
        /// </summary>
        public sealed class DialogCall
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DialogCall"/> type.
            /// </summary>
            public DialogCall(string title, string message, string ok, string cancel)
            {
                Title = title;
                Message = message;
                Ok = ok;
                Cancel = cancel;
            }

            /// <summary>
            /// Gets the title.
            /// </summary>
            public string Title { get; private set; }
            /// <summary>
            /// Gets the message.
            /// </summary>
            public string Message { get; private set; }
            /// <summary>
            /// Gets the text for the OK button.
            /// </summary>
            public string Ok { get; private set; }
            /// <summary>
            /// Gets the text for the Cancel button.
            /// </summary>
            public string Cancel { get; private set; }
        }

        /// <summary>
        /// Gets or sets the next display dialog result.
        /// </summary>
        public static bool NextDisplayDialogResult { get; set; } = true;
        /// <summary>
        /// Gets the captured dialog calls for test verification.
        /// </summary>
        public static IList<DialogCall> Calls { get; } = new List<DialogCall>();

        /// <summary>
        /// Resets the configured dialog result and clears captured dialog calls.
        /// </summary>
        public static void Reset()
        {
            NextDisplayDialogResult = true;
            Calls.Clear();
        }

        /// <summary>
        /// Captures a dialog call and returns the configured result for test verification.
        /// </summary>
        public static bool DisplayDialog(string title, string message, string ok, string cancel)
        {
            Calls.Add(new DialogCall(title, message, ok, cancel));
            return NextDisplayDialogResult;
        }
    }

    /// <summary>
    /// Defines the settings scope.
    /// </summary>
    public enum SettingsScope
    {
        User,
        Project
    }

    /// <summary>
    /// Represents the settings provider attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class SettingsProviderAttribute : Attribute
    {
    }

    /// <summary>
    /// Defines the message type.
    /// </summary>
    public enum MessageType
    {
        None,
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// Represents the settings provider.
    /// </summary>
    public class SettingsProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsProvider"/> type.
        /// </summary>
        public SettingsProvider(string path, SettingsScope scopes)
        {
            settingsPath = path;
            scope = scopes;
        }

        /// <summary>
        /// Gets the settings path.
        /// </summary>
        public string settingsPath { get; private set; }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        public SettingsScope scope { get; private set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string label { get; protected set; }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        public HashSet<string> keywords { get; protected set; }

        /// <summary>
        /// Executes the on activate operation.
        /// </summary>
        public virtual void OnActivate(string searchContext, VisualElement rootElement)
        {
        }

        /// <summary>
        /// Executes the on gui operation.
        /// </summary>
        public virtual void OnGUI(string searchContext)
        {
        }
    }

    /// <summary>
    /// Provides the settings service functionality.
    /// </summary>
    public static class SettingsService
    {
        /// <summary>
        /// Gets or sets the last opened project settings path.
        /// </summary>
        public static string LastOpenedProjectSettingsPath { get; private set; }

        /// <summary>
        /// Resets the operation.
        /// </summary>
        public static void Reset()
        {
            LastOpenedProjectSettingsPath = null;
        }

        /// <summary>
        /// Opens project settings.
        /// </summary>
        public static void OpenProjectSettings(string path)
        {
            LastOpenedProjectSettingsPath = path;
        }
    }

    /// <summary>
    /// Provides the editor gui layout functionality.
    /// </summary>
    public static class EditorGUILayout
    {
        /// <summary>
        /// Gets the label fields rendered through the stub.
        /// </summary>
        public static IList<string> LabelFields { get; } = new List<string>();
        /// <summary>
        /// Gets the help boxes rendered through the stub.
        /// </summary>
        public static IList<HelpBoxCall> HelpBoxes { get; } = new List<HelpBoxCall>();

        /// <summary>
        /// Represents the help box call.
        /// </summary>
        public sealed class HelpBoxCall
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="HelpBoxCall"/> type.
            /// </summary>
            public HelpBoxCall(string message, MessageType messageType)
            {
                Message = message;
                MessageType = messageType;
            }

            /// <summary>
            /// Gets the message.
            /// </summary>
            public string Message { get; private set; }

            /// <summary>
            /// Gets the message type.
            /// </summary>
            public MessageType MessageType { get; private set; }
        }

        /// <summary>
        /// Clears captured label fields and help boxes.
        /// </summary>
        public static void Reset()
        {
            LabelFields.Clear();
            HelpBoxes.Clear();
        }

        /// <summary>
        /// Captures a label field for test verification.
        /// </summary>
        public static void LabelField(string label)
        {
            LabelFields.Add(label);
        }

        /// <summary>
        /// Returns the provided text value unchanged.
        /// </summary>
        public static string TextField(string label, string text)
        {
            return text;
        }

        /// <summary>
        /// Returns the provided boolean value unchanged.
        /// </summary>
        public static bool Toggle(string label, bool value)
        {
            return value;
        }

        /// <summary>
        /// Captures a help box for test verification.
        /// </summary>
        public static void HelpBox(string message, MessageType messageType)
        {
            HelpBoxes.Add(new HelpBoxCall(message, messageType));
        }

        /// <summary>
        /// Renders a vertical space as a no-op in the stub implementation.
        /// </summary>
        public static void Space()
        {
        }
    }

    /// <summary>
    /// Provides the gui layout functionality.
    /// </summary>
    public static class GUILayout
    {
        /// <summary>
        /// Gets the button labels rendered through the stub.
        /// </summary>
        public static IList<string> Buttons { get; } = new List<string>();

        /// <summary>
        /// Clears captured button labels.
        /// </summary>
        public static void Reset()
        {
            Buttons.Clear();
        }

        /// <summary>
        /// Captures a button label and returns <see langword="false"/>.
        /// </summary>
        public static bool Button(string text)
        {
            Buttons.Add(text);
            return false;
        }
    }
}

namespace UnityEngine.UIElements
{
    /// <summary>
    /// Represents the visual element.
    /// </summary>
    public class VisualElement
    {
    }
}
