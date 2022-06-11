using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditor.Presets;
using UnityEngine;

namespace MixamoAssetPreprocessor
{

    class AssetProcessorUtil
    {
        public static Hash128 ComputePresetHash(IEnumerable<string> presetPaths)
        {
            Hash128 hash = new Hash128();
            foreach (var assetPath in presetPaths.OrderBy(a => a))
            {
                hash.Append(assetPath);
                hash.Append(AssetDatabase.LoadAssetAtPath<Preset>(assetPath).GetTargetFullTypeName());
            }

            return hash;
        }
    }
    
    /// <summary>
   /// This sample class applies Presets automatically to Assets in the folder containing the Preset and any subfolders.
   /// The code is divided into three parts that set up importer dependencies to make sure the importers of all Assets stay deterministic.
   ///
   /// OnPreprocessAsset:
   /// This method goes from the root folder down to the Asset folder for each imported asset
   /// and registers a CustomDependency to each folder in case a Preset is added/removed at a later time.
   /// It then loads all Presets from that folder and tries to apply them to the Asset importer.
   /// If it is applied, the method adds a direct dependency to each Preset so that the Asset can be re-imported when the Preset values are changed.
   /// </summary>
    public class SyntyMixamoPresetPostProcessor : AssetPostprocessor
    {
        public string[] paths;
        void OnPreprocessAsset()
        {
            if (assetPath.StartsWith("Assets/Animation/Animations/Synty/Mixamo/") && Path.GetExtension(assetPath) == ".fbx")
            {
                var path = Path.GetDirectoryName(assetPath);
                ApplyPresetsFromFolderRecursively(path);
            }
        }
        void ApplyPresetsFromFolderRecursively(string folder)
        {
            var parentFolder = Path.GetDirectoryName(folder);

            // Do not consider presets outside the Mixamo directory
            if (!folder.StartsWith("Assets/Animation/Animations/Synty/Mixamo"))
            {
                return;
            }

            if (!string.IsNullOrEmpty(parentFolder))
            {
                ApplyPresetsFromFolderRecursively(parentFolder);
            }

            // Add a dependency to the folder Preset custom key
            // so whenever a Preset is added to or removed from this folder, the Asset is re-imported.
            context.DependsOnCustomDependency($"SyntyMixamoPresetPostProcessor_{folder}");
            // Find all Preset Assets in this folder. Use the System.Directory method instead of the AssetDatabase
            // because the import may run in a separate process which prevents the AssetDatabase from performing a global search.
            var presetPaths =
                Directory.EnumerateFiles(folder, "*.preset", SearchOption.TopDirectoryOnly)
                    .OrderBy(a => a);
            foreach (var presetPath in presetPaths)
            {
                // Load the Preset and try to apply it to the importer.
                var preset = AssetDatabase.LoadAssetAtPath<Preset>(presetPath);
                // The script adds a Presets dependency to an Asset in two cases:
                //1 If the Asset is imported before the Preset, the Preset will not load because it is not yet imported.
                //Adding a dependency between the Asset and the Preset allows the Asset to be re-imported so that Unity loads
                //the assigned Preset and can try to apply its values.
                //2 If the Preset loads successfully, the ApplyTo method returns true if the Preset applies to this Asset's import settings.
                //Adding the Preset as a dependency to the Asset ensures that any change in the Preset values will re-import the Asset using the new values.
                if (preset == null || preset.ApplyTo(assetImporter))
                {
                    // Using DependsOnArtifact here because Presets are native assets and using DependsOnSourceAsset would not work.
                    context.DependsOnArtifact(presetPath);
                }
            }
        }
        
        /// <summary>
        /// This method with the didDomainReload argument will be called every time the project is being loaded or the code is compiled.
        /// It is very important to set all of the hashes correctly at startup
        /// because Unity does not apply the OnPostprocessAllAssets method to previously imported Presets
        /// and the CustomDependencies are not saved between sessions and need to be rebuilt every time.
        /// </summary>
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths, bool didDomainReload)
        {
            if (didDomainReload)
            {
                var presetAssetPaths = AssetDatabase.FindAssets("glob:\"**.preset\"", new []{"Assets/Animation/Animations/Synty/Mixamo/"})
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .GroupBy(Path.GetDirectoryName);
                
                bool dependencyUpdated = false;

                foreach (var path in presetAssetPaths)
                {
                    var folder = path.Key;
                    AssetDatabase.RegisterCustomDependency($"SyntyMixamoPresetPostProcessor_{folder}", AssetProcessorUtil.ComputePresetHash(path));
                    dependencyUpdated = true;
                }
                
                // Only trigger a Refresh if there is at least one dependency updated here.
                if (dependencyUpdated)
                {
                    AssetDatabase.Refresh();
                }
            }
        }
    }
    /// <summary>
    /// OnAssetsModified:
    /// Whenever a Preset is added, removed, or moved from a folder, the CustomDependency for this folder needs to be updated
    /// so Assets that may depend on those Presets are reimported.
    /// </summary>
    public class SyntyMixamoPresetModifiedUpdater : AssetsModifiedProcessor
    {
        /// <summary>
        /// The OnAssetsModified method is called whenever an Asset has been changed in the project.
        /// This methods determines if any Preset has been added, removed, or moved
        /// and updates the CustomDependency related to the changed folder.
        /// </summary>
        protected override void OnAssetsModified(string[] changedAssets, string[] addedAssets, string[] deletedAssets, AssetMoveInfo[] movedAssets)
        {
            HashSet<string> folders = movedAssets
                .SelectMany(x => new[] { x.destinationAssetPath, x.sourceAssetPath })
                .Concat(changedAssets)
                .Concat(addedAssets)
                .Concat(deletedAssets)
                .Where(x => Path.GetDirectoryName(x).StartsWith("Assets/Animation/Animations/Synty/Mixamo"))
                .Where(x => x.EndsWith(".preset"))
                .ToHashSet();
            
            // Do not add a dependency update for no reason.
            if (folders.Count != 0)
            {
                // The dependencies need to be updated outside of the AssetPostprocessor calls.
                // Register the method to the next Editor update.
                EditorApplication.delayCall += () =>
                {
                    DelayedDependencyRegistration(folders);
                };
            }
        }
        /// <summary>
        /// This method loads all Presets in each of the given folder paths
        /// and updates the CustomDependency hash based on the Presets currently in that folder.
        /// </summary>
        static void DelayedDependencyRegistration(HashSet<string> folders)
        {
            foreach (var folder in folders)
            {
                Debug.Log(folder);
                // Get assets in folder
                var presetPaths =
                    AssetDatabase.FindAssets("glob:\"**.preset\"", new[] { folder })
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Where(presetPath => Path.GetDirectoryName(presetPath) == folder);
                
                AssetDatabase.RegisterCustomDependency($"SyntyMixamoPresetPostProcessor_{folder}", AssetProcessorUtil.ComputePresetHash(presetPaths));
            }
            // Manually trigger a Refresh
            // so that the AssetDatabase triggers a dependency check on the updated folder hash.
            AssetDatabase.Refresh();
        }
    }
}