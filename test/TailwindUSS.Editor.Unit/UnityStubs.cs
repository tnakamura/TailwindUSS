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
}
