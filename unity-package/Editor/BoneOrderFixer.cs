using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace MoreCustomizations.Tools {
    public class BoneOrderFixer : AssetPostprocessor {
        void OnPostprocessModel(GameObject prefab)
        {

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
                    
                    Debug.Log(
                        "Bones do not match required structure. "
                     + $"Missing at least one bone '{boneName}'. Won't post-process this models bone order."
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

            var bindPoses = OutfitBindPoses.TEXT_VALUE.Trim().Split(" - ").Select(bindPose =>
            {
                var elementTexts = bindPose.Trim().Split("\n").Take(16).ToArray();
                var elements = new float[16];
                for (int i = 0; i < 16; i++)
                    elements[i] = float.Parse(elementTexts[i].Trim()[4..]);
                return new Matrix4x4(
                    new(elements[0], elements[4], elements[8], elements[12]),
                    new(elements[1], elements[5], elements[9], elements[13]),
                    new(elements[2], elements[6], elements[10], elements[14]),
                    new(elements[3], elements[7], elements[11], elements[15])
                ) * Matrix4x4.Scale(Vector3.one / 0.3307868f / 0.8258692f) * Matrix4x4.Rotate(Quaternion.Euler(90, 0, 0));
            } //The two magic numbers come from the scale of the 'Scout' (0.3307868f) and the 'MainMesh' (0.8258692f) transforms.
              //The 90° rotation cancels out the -90° rotation of the 'MainMesh' transform
            ).ToArray();

            skinnedMeshRenderer.bones = transforms.ToArray();
            
            skinnedMeshRenderer.sharedMesh.boneWeights = boneWeights;
            skinnedMeshRenderer.sharedMesh.bindposes   = bindPoses;
            
            Debug.Log($"Reordered bones for {prefab.name} to match PEAK™ MainMesh-es!", prefab);
        }
    }
}