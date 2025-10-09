using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using MoreCustomizations.Tools.Data;

namespace MoreCustomizations.Tools {
    
    public class BoneOrderFixer : AssetPostprocessor {
        
        private void OnPostprocessModel(GameObject prefab) {
            
            if (!PrefabUtility.IsPartOfImmutablePrefab(prefab))
                return;
            
            var skinnedMeshRenderer = prefab.GetComponentInChildren<SkinnedMeshRenderer>();
            
            if (!skinnedMeshRenderer)
                return;
            
            var transforms = new List<Transform>(OutfitBoneOrder.Value.Length);
            
            for (int index = 0; index < OutfitBoneOrder.Value.Length; index++) {
                
                var boneName = OutfitBoneOrder.Value[index];
                boneName = Regex.Replace(boneName, @"[^a-zA-Z0-9]", "_");
                
                Transform boneTransform = skinnedMeshRenderer.bones
                    .FirstOrDefault(t => t.name.Equals(boneName, System.StringComparison.InvariantCultureIgnoreCase));
                
                if (!boneTransform) {
                    
                    Debug.LogError(
                        "Bones do not match required structure. "
                     + $"Missing at least one bone '{boneName}'. Won't post-process this models bone order.",
                        prefab
                    );
                    
                    return;
                }
                
                transforms.Add(boneTransform);
            }
            
            var permutations = new Dictionary<int, int>();
            
            for (int index = 0; index < skinnedMeshRenderer.bones.Length; index++)
                permutations[index] = transforms.IndexOf(skinnedMeshRenderer.bones[index]);
            
            var boneWeights = skinnedMeshRenderer.sharedMesh.boneWeights;
            
            for (int i = 0; i < boneWeights.Length; i++) {
                
                boneWeights[i].boneIndex0 = permutations[boneWeights[i].boneIndex0];
                boneWeights[i].boneIndex1 = permutations[boneWeights[i].boneIndex1];
                boneWeights[i].boneIndex2 = permutations[boneWeights[i].boneIndex2];
                boneWeights[i].boneIndex3 = permutations[boneWeights[i].boneIndex3];
            }
            
            skinnedMeshRenderer.bones = transforms.ToArray();
            
            skinnedMeshRenderer.sharedMesh.boneWeights = boneWeights;
            skinnedMeshRenderer.sharedMesh.bindposes   = OutfitBindPoses.Value.ToArray();
            
            Debug.Log($"Reordered bones for {prefab.name} to match PEAK™ MainMesh-es!", prefab);
        }
    }
}