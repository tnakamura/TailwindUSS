using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TailwindUSS.Editor.Tests
{
    /// <summary>
    /// Represents the test project scope.
    /// </summary>
    internal sealed class TestProjectScope : IDisposable
    {
        private readonly string previousDataPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestProjectScope"/> type.
        /// </summary>
        public TestProjectScope()
        {
            RootPath = Path.Combine(Path.GetTempPath(), "TailwindUSS.Tests", Guid.NewGuid().ToString("N"));
            AssetsPath = Path.Combine(RootPath, "Assets");
            Directory.CreateDirectory(AssetsPath);

            previousDataPath = Application.dataPath;
            Application.dataPath = AssetsPath;
            AssetDatabase.Reset();
            EditorUtility.Reset();
            EditorGUILayout.Reset();
            GUILayout.Reset();
            SettingsService.Reset();
            Debug.Reset();
            IncrementalGenerationCache.Shared.Reset();
        }

        /// <summary>
        /// Gets the root path.
        /// </summary>
        public string RootPath { get; private set; }
        /// <summary>
        /// Gets the assets path.
        /// </summary>
        public string AssetsPath { get; private set; }

        /// <summary>
        /// Gets the full path for an asset relative to the Assets directory.
        /// </summary>
        public string GetAssetPath(params string[] segments)
        {
            return Path.Combine(AssetsPath, Path.Combine(segments));
        }

        /// <summary>
        /// Gets the full path for a file relative to the project root.
        /// </summary>
        public string GetProjectPath(params string[] segments)
        {
            return Path.Combine(RootPath, Path.Combine(segments));
        }

        /// <summary>
        /// Writes a file to the Assets directory and returns its full path.
        /// </summary>
        public string WriteAssetFile(string relativePath, string content)
        {
            return WriteProjectFile(Path.Combine("Assets", relativePath), content);
        }

        /// <summary>
        /// Writes a file to the project directory and returns its full path.
        /// </summary>
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

        /// <summary>
        /// Cleans up the temporary test project and restores the original Unity data path.
        /// </summary>
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
