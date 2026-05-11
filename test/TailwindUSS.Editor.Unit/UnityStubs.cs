using System;
using System.Collections.Generic;
using System.Text.Json;

namespace UnityEngine
{
    public class Object
    {
    }

    public static class Application
    {
        public static string dataPath { get; set; } = string.Empty;
    }

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

        public static T FromJson<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, Options);
        }

        public static string ToJson(object obj, bool prettyPrint)
        {
            return JsonSerializer.Serialize(obj, prettyPrint ? IndentedOptions : Options);
        }
    }

    public static class Debug
    {
        public sealed class LogEntry
        {
            public LogEntry(string level, string message, Object context)
            {
                Level = level;
                Message = message;
                Context = context;
            }

            public string Level { get; private set; }
            public string Message { get; private set; }
            public Object Context { get; private set; }
        }

        private static readonly List<LogEntry> entries = new List<LogEntry>();

        public static IReadOnlyList<LogEntry> Entries
        {
            get { return entries; }
        }

        public static void Reset()
        {
            entries.Clear();
        }

        public static void Log(object message)
        {
            entries.Add(new LogEntry("Log", message == null ? string.Empty : message.ToString(), null));
        }

        public static void LogWarning(object message)
        {
            entries.Add(new LogEntry("Warning", message == null ? string.Empty : message.ToString(), null));
        }

        public static void LogWarning(object message, Object context)
        {
            entries.Add(new LogEntry("Warning", message == null ? string.Empty : message.ToString(), context));
        }

        public static void LogError(object message)
        {
            entries.Add(new LogEntry("Error", message == null ? string.Empty : message.ToString(), null));
        }

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

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class MenuItem : Attribute
    {
        public MenuItem(string menuItem)
        {
            MenuPath = menuItem;
        }

        public string MenuPath { get; private set; }
    }

    public static class AssetDatabase
    {
        public static int RefreshCallCount { get; private set; }
        public static IList<string> LoadedAssetPaths { get; } = new List<string>();

        public static void Reset()
        {
            RefreshCallCount = 0;
            LoadedAssetPaths.Clear();
        }

        public static void Refresh()
        {
            RefreshCallCount++;
        }

        public static T LoadAssetAtPath<T>(string assetPath) where T : Object, new()
        {
            LoadedAssetPaths.Add(assetPath);
            return new T();
        }
    }

    public static class EditorUtility
    {
        public sealed class DialogCall
        {
            public DialogCall(string title, string message, string ok, string cancel)
            {
                Title = title;
                Message = message;
                Ok = ok;
                Cancel = cancel;
            }

            public string Title { get; private set; }
            public string Message { get; private set; }
            public string Ok { get; private set; }
            public string Cancel { get; private set; }
        }

        public static bool NextDisplayDialogResult { get; set; } = true;
        public static IList<DialogCall> Calls { get; } = new List<DialogCall>();

        public static void Reset()
        {
            NextDisplayDialogResult = true;
            Calls.Clear();
        }

        public static bool DisplayDialog(string title, string message, string ok, string cancel)
        {
            Calls.Add(new DialogCall(title, message, ok, cancel));
            return NextDisplayDialogResult;
        }
    }

    public enum SettingsScope
    {
        User,
        Project
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class SettingsProviderAttribute : Attribute
    {
    }

    public enum MessageType
    {
        None,
        Info,
        Warning,
        Error
    }

    public class SettingsProvider
    {
        public SettingsProvider(string path, SettingsScope scopes)
        {
            settingsPath = path;
            scope = scopes;
        }

        public string settingsPath { get; private set; }

        public SettingsScope scope { get; private set; }

        public string label { get; protected set; }

        public HashSet<string> keywords { get; protected set; }

        public virtual void OnActivate(string searchContext, VisualElement rootElement)
        {
        }

        public virtual void OnGUI(string searchContext)
        {
        }
    }

    public static class SettingsService
    {
        public static string LastOpenedProjectSettingsPath { get; private set; }

        public static void Reset()
        {
            LastOpenedProjectSettingsPath = null;
        }

        public static void OpenProjectSettings(string path)
        {
            LastOpenedProjectSettingsPath = path;
        }
    }

    public static class EditorGUILayout
    {
        public static IList<string> LabelFields { get; } = new List<string>();
        public static IList<HelpBoxCall> HelpBoxes { get; } = new List<HelpBoxCall>();

        public sealed class HelpBoxCall
        {
            public HelpBoxCall(string message, MessageType messageType)
            {
                Message = message;
                MessageType = messageType;
            }

            public string Message { get; private set; }

            public MessageType MessageType { get; private set; }
        }

        public static void Reset()
        {
            LabelFields.Clear();
            HelpBoxes.Clear();
        }

        public static void LabelField(string label)
        {
            LabelFields.Add(label);
        }

        public static string TextField(string label, string text)
        {
            return text;
        }

        public static bool Toggle(string label, bool value)
        {
            return value;
        }

        public static void HelpBox(string message, MessageType messageType)
        {
            HelpBoxes.Add(new HelpBoxCall(message, messageType));
        }

        public static void Space()
        {
        }
    }

    public static class GUILayout
    {
        public static IList<string> Buttons { get; } = new List<string>();

        public static void Reset()
        {
            Buttons.Clear();
        }

        public static bool Button(string text)
        {
            Buttons.Add(text);
            return false;
        }
    }
}

namespace UnityEngine.UIElements
{
    public class VisualElement
    {
    }
}
