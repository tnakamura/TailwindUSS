using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TailwindUSS.Editor.Tests
{
    internal sealed class TestProjectScope : IDisposable
    {
        private readonly string previousDataPath;

        public TestProjectScope()
        {
            RootPath = Path.Combine(Path.GetTempPath(), "TailwindUSS.Tests", Guid.NewGuid().ToString("N"));
            AssetsPath = Path.Combine(RootPath, "Assets");
            Directory.CreateDirectory(AssetsPath);

            previousDataPath = Application.dataPath;
            Application.dataPath = AssetsPath;
            AssetDatabase.Reset();
            EditorUtility.Reset();
            Debug.Reset();
        }

        public string RootPath { get; private set; }
        public string AssetsPath { get; private set; }

        public string GetAssetPath(params string[] segments)
        {
            return Path.Combine(AssetsPath, Path.Combine(segments));
        }

        public string GetProjectPath(params string[] segments)
        {
            return Path.Combine(RootPath, Path.Combine(segments));
        }

        public string WriteAssetFile(string relativePath, string content)
        {
            return WriteProjectFile(Path.Combine("Assets", relativePath), content);
        }

        public string WriteProjectFile(string relativePath, string content)
        {
            var fullPath = Path.Combine(RootPath, relativePath);
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(fullPath, content);
            return fullPath;
        }

        public void Dispose()
        {
            Application.dataPath = previousDataPath;

            if (Directory.Exists(RootPath))
            {
                Directory.Delete(RootPath, true);
            }
        }
    }
}
