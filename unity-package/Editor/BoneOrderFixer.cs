using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace MoreCustomizations.Tools {
    
    public class BoneOrderFixer : AssetPostprocessor {
        
        private string[] BoneOrder = new string[] {
            "Hip",
            "Hip_R",
            "Leg_R",
            "Knee_R",
            "Foot_R",
            "S_Toe_1_R",
            "S_Toe_2_R",
            "S_Heel_R",
            "ShoeWeight_R",
            "Hip_L.014",
            "Hip_L.018",
            "Hip_L.015",
            "Hip_L.019",
            "Hip_L.016",
            "Hip_L.020",
            "Hip_L.017",
            "Hip_L.021",
            "PantWeight_R",
            "Hip_L.008",
            "Hip_L.009",
            "Hip_L.012",
            "Hip_L.010",
            "Hip_L.011",
            "Hip_L",
            "Leg_L",
            "Knee_L",
            "Foot_L",
            "S_Toe_1_L",
            "S_Toe_2_L",
            "S_Heel_L",
            "ShoeWeight_L",
            "Hip_R.014",
            "Hip_R.018",
            "Hip_R.015",
            "Hip_R.019",
            "Hip_R.016",
            "Hip_R.020",
            "Hip_R.017",
            "Hip_R.021",
            "PantWeight_L",
            "Hip_R.008",
            "Hip_R.009",
            "Hip_R.012",
            "Hip_R.010",
            "Hip_R.011",
            "Waist",
            "WaistR1",
            "WaistL1",
            "WaistF1",
            "WaistB1",
            "Mid",
            "Sash_Base",
            "SashWeight",
            "AimJoint",
            "Torso",
            "S_Shoulder_R",
            "Arm_R",
            "Elbow_R",
            "Hand_R",
            "Hand_Upper_R",
            "Pinky_1_R",
            "Pinky_2_R",
            "Pinky_3_R",
            "Middle_1_R",
            "Middle_2_R",
            "Middle_3_R",
            "Index_1_R",
            "Index_2_R",
            "Index_3_R",
            "Thumb_R_1",
            "Thumb_R_2",
            "Thumb_R_3",
            "ShirtArm_R",
            "Hip.002_L.018",
            "Hip.002_L.022",
            "Hip.002_L.019",
            "Hip.002_L.023",
            "Hip.002_L.020",
            "Hip.002_L.021",
            "S_Shoulder_L",
            "Arm_L",
            "Elbow_L",
            "Hand_L",
            "Hand_Upper_L",
            "Pinky_1_L",
            "Pinky_2_L",
            "Pinky_3_L",
            "Middle_1_L",
            "Middle_2_L",
            "Middle_3_L",
            "Index_1_L",
            "Index_2_L",
            "Index_3_L",
            "Thumb_1_L",
            "Thumb_2_L",
            "Thumb_3_L",
            "ShirtArm_L",
            "Hip.002_R.018",
            "Hip.002_R.023",
            "Hip.002_R.019",
            "Hip.002_R.024",
            "Hip.002_R.020",
            "Hip.002_R.021",
            "Sash_Shoulder",
            "Hip.002_R.025",
            "Hip.002_R.026",
            "Head",
            "Face",
            "Detail",
            "Chest",
            "ChestF",
            "ChestB",
            "Collar",
            "CollarB",
            "Collar_R",
            "Collar_R.001",
            "Collar_L",
            "Collar_L.001",
            "Sash_F",
            "Sash_B" };
        
        void OnPostprocessModel(GameObject prefab) {
            
            var skinnedMeshRenderer = prefab.GetComponentInChildren<SkinnedMeshRenderer>();
            
            if (!skinnedMeshRenderer)
                return;
            
            var transforms = new List<Transform>(BoneOrder.Length);
            
            for (int index = 0; index < BoneOrder.Length; index++) {
                
                var boneName = BoneOrder[index];
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
            
            //TODO: Ask what permut meaning
            var permut = new Dictionary<int, int>();
            
            for (int index = 0; index < skinnedMeshRenderer.bones.Length; index++)
                permut[index] = transforms.IndexOf(skinnedMeshRenderer.bones[index]);
            
            var boneWeights = skinnedMeshRenderer.sharedMesh.boneWeights;
            
            for (int i = 0; i < boneWeights.Length; i++) {
                
                boneWeights[i].boneIndex0 = permut[boneWeights[i].boneIndex0];
                boneWeights[i].boneIndex1 = permut[boneWeights[i].boneIndex1];
                boneWeights[i].boneIndex2 = permut[boneWeights[i].boneIndex2];
                boneWeights[i].boneIndex3 = permut[boneWeights[i].boneIndex3];
            }
            
            var bindPoses = new Matrix4x4[skinnedMeshRenderer.sharedMesh.bindposes.Length];
            
            for (int index = 0; index < bindPoses.Length; index++)
                bindPoses[permut[index]] = skinnedMeshRenderer.sharedMesh.bindposes[index];
            
            skinnedMeshRenderer.bones = transforms.ToArray();
            
            skinnedMeshRenderer.sharedMesh.boneWeights = boneWeights;
            skinnedMeshRenderer.sharedMesh.bindposes   = bindPoses;
            
            Debug.Log($"Reordered bones for {prefab.name} to match PEAK™ MainMesh-es!", prefab);
        }
    }
}