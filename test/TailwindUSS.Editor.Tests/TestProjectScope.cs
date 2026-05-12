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
        }

        /// <summary>
        /// Gets or sets the root path.
        /// </summary>
        public string RootPath { get; private set; }
        /// <summary>
        /// Gets or sets the assets path.
        /// </summary>
        public string AssetsPath { get; private set; }

        /// <summary>
        /// Tests that get asset path.
        /// </summary>
        public string GetAssetPath(params string[] segments)
        {
            return Path.Combine(AssetsPath, Path.Combine(segments));
        }

        /// <summary>
        /// Tests that get project path.
        /// </summary>
        public string GetProjectPath(params string[] segments)
        {
            return Path.Combine(RootPath, Path.Combine(segments));
        }

        /// <summary>
        /// Tests that write asset file.
        /// </summary>
        public string WriteAssetFile(string relativePath, string content)
        {
            return WriteProjectFile(Path.Combine("Assets", relativePath), content);
        }

        /// <summary>
        /// Tests that write project file.
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
        /// Tests that dispose.
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
