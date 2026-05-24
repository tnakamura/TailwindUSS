using UnityEditor;

namespace TailwindUSS.Editor
{
    /// <summary>
    /// Automatically regenerates USS when tracked UXML assets are saved.
    /// </summary>
    internal sealed class TailwindUssAutoGenerator : AssetPostprocessor
    {
        private static bool isGenerating;

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (isGenerating)
            {
                return;
            }

            if (!HasRelevantChanges(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths))
            {
                return;
            }

            if (!ConfigLoader.TryLoad(out var config, out _, out _))
            {
                new GenerationService().GenerateIncremental(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
                return;
            }

            if (!config.autoGenerateOnUxmlSave)
            {
                return;
            }

            try
            {
                isGenerating = true;
                new GenerationService().GenerateIncremental(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
            }
            finally
            {
                isGenerating = false;
            }
        }

        private static bool HasRelevantChanges(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            return HasUxmlAsset(importedAssets) ||
                HasUxmlAsset(deletedAssets) ||
                HasUxmlAsset(movedAssets) ||
                HasUxmlAsset(movedFromAssetPaths);
        }

        private static bool HasUxmlAsset(string[] assetPaths)
        {
            if (assetPaths == null)
            {
                return false;
            }

            foreach (var assetPath in assetPaths)
            {
                if (!string.IsNullOrWhiteSpace(assetPath) && assetPath.EndsWith(".uxml", System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
