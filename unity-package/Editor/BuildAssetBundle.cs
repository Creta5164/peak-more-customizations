using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using MoreCustomizations.Data;

namespace MoreCustomizations.Tools {
    
    internal static class BuildAssetBundle {
        
        [MenuItem("For PEAK/Build asset bundle")]
        private static void Build() {
            
            var outputPath = "Assets/AssetBundles";
            
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
            
            var allValidateStatus = AssetDatabase.FindAssets($"t:{nameof(CustomizationData)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<CustomizationData>)
                .OfType<CustomizationData>()
                .SelectMany(static data => data.GetValidateContentStatuses().Select(status => (data, status)));
            
            foreach (var (data, status) in allValidateStatus) {
                
                string message = $"[{data.name}] {status.Message}";
                
                switch (status.Type) {
                    
                    case ValidateStatus.ValidateType.Valid:   Debug.Log(message, data);        break;
                    case ValidateStatus.ValidateType.Warning: Debug.LogWarning(message, data); break;
                    case ValidateStatus.ValidateType.Error:   Debug.LogError(message, data);   break;
                }
            }
            
            if (allValidateStatus.Any(static pair => pair.status.Type == ValidateStatus.ValidateType.Error)) {
                
                EditorUtility.DisplayDialog(
                    "Build PEAK bundle failed",
                    "Failed to build PEAK bundles, check console for more details.",
                    "OK"
                );
                
                return;
            }
            
            BuildPipeline.BuildAssetBundles(
                outputPath,
                BuildAssetBundleOptions.RecurseDependencies,
                BuildTarget.StandaloneWindows
            );
            
            AssetDatabase.Refresh();
        }
    }
}