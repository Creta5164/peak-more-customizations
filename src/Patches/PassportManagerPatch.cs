using System.Linq;
using System.Reflection.Emit;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using MoreCustomizations.Data;
using MoreCustomizations.Helpers;

using Plugin = MoreCustomizations.MoreCustomizationsPlugin;

namespace MoreCustomizations.Patches;

public class PassportManagerPatch {
    
    private static Material materialTemplate;
    
    [HarmonyPatch(typeof(PassportManager), "Awake")]
    [HarmonyPostfix]
    private static void Awake(PassportManager __instance) {
        
        var allCustomizationsData = Plugin.AllCustomizationsData;
        
        if (allCustomizationsData == null) {
            
            Plugin.Logger.LogError("Customizations data are not loaded!");
            return;
        }
        
        if (allCustomizationsData.Count == 0) {
            
            Plugin.Logger.LogWarning("There's no customizations data.");
            return;
        }
        
        var customization = __instance.GetComponent<Customization>();
        
        var skins       = new List<CustomizationOption>(customization.skins);
        var accessories = new List<CustomizationOption>(customization.accessories);
        var eyes        = new List<CustomizationOption>(customization.eyes);
        var mouths      = new List<CustomizationOption>(customization.mouths);
        var fits        = new List<CustomizationOption>(customization.fits);
        var hats        = new List<CustomizationOption>(customization.hats);
        
        //FIXME: Workaround for hidden "Ice Hood Blue" and "Ice Hood Pink" hats.
        //     : Add two placeholder hat entries so indices and UI positions match the game's expectations.
        string[] missingHats = [ "Ice Hood Blue", "Ice Hood Pink" ];
        
        foreach (var missingHat in missingHats) {
            
            if (!hats.Any(h => h.name == missingHat))
                hats.Add(CreateHatPlaceholder());
        }
        
        foreach (var (type, customizationsData) in allCustomizationsData) {
            
            var customizationOptions = type switch {
                
                Customization.Type.Skin      => skins,
                Customization.Type.Accessory => accessories,
                Customization.Type.Eyes      => eyes,
                Customization.Type.Mouth     => mouths,
                Customization.Type.Fit       => fits,
                Customization.Type.Hat       => hats,
                
                _ => null
            };
            
            if (customizationOptions == null)
                continue;
            
            foreach (var customizationData in customizationsData) {
                
                if (!customizationData || !customizationData.IsValid)
                    continue;
                 
                var option = ScriptableObject.CreateInstance<CustomizationOption>();
                
                option.requiredAchievement = ACHIEVEMENTTYPE.NONE;
                
                option.name    = customizationData.name;
                option.type    = customizationData.Type;
                option.texture = customizationData.IconTexture;
                
                if (type == Customization.Type.Fit) {
                    
                    var fitData = customizationData as CustomFit_V1;
                    
                    if (!fitData)
                        continue;
                    
                    if (!materialTemplate) {
                        
                        materialTemplate = customization?.fits.FirstOrDefault()?.fitMaterial;
                        
                        if (!materialTemplate) {
                            
                            Plugin.Logger.LogWarning(
                                "Could not find existing fitMaterial to copy! Using fallback material, "
                                + "expect some visual errors"
                            );
                            
                            materialTemplate = FitMaterialFallback.MaterialTemplate;
                        }
                    }
                    
                    option.fitMesh      = fitData.FitMesh;
                    option.isSkirt      = fitData.IsSkirt;
                    option.noPants      = fitData.NoPants;
                    option.drawUnderEye = fitData.DrawUnderEye;
                    
                    option.fitMaterial = Object.Instantiate(materialTemplate);
                    option.fitMaterial.SetTexture("_MainTex", fitData.FitMainTexture);
                    
                    option.fitMaterialShoes = Object.Instantiate(materialTemplate);
                    option.fitMaterialShoes.SetTexture("_MainTex", fitData.FitShoeTexture);
                    
                    if (fitData.FitOverrideHatTexture) {
                        
                        option.fitMaterialOverrideHat = Object.Instantiate(materialTemplate);
                        option.fitMaterialOverrideHat.SetTexture("_MainTex", fitData.FitOverrideHatTexture);
                    }
                    
                    if (fitData.FitOverridePantsTexture) {

                        option.fitMaterialOverridePants = Object.Instantiate(materialTemplate);
                        option.fitMaterialOverridePants.SetTexture("_MainTex", fitData.FitOverridePantsTexture);
                    }
                }
                
                customizationOptions.Add(option);
            }
        }
        
        customization.skins       = skins.ToArray();
        customization.accessories = accessories.ToArray();
        customization.eyes        = eyes.ToArray();
        customization.mouths      = mouths.ToArray();
        customization.fits        = fits.ToArray();
        customization.hats        = hats.ToArray();
    }
    
    /// <remarks>
    /// Required for temporary workaround hat de-sync.
    /// </remarks>
    private static CustomizationOption CreateHatPlaceholder() {
        
        var option = ScriptableObject.CreateInstance<CustomizationOption>();
        option.name = "None";
        option.type = Customization.Type.Hat;
        option.requiresAscent = true;
        option.requiredAscent = 99999;
        return option;
    }
    
    [HarmonyPatch(typeof(PassportManager), "CameraIn")]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> CameraInTranspiler(IEnumerable<CodeInstruction> instructions) {
        
        //this.dummyCamera.transform.DOLocalMove(new Vector3(0f, 1.65f, 1f), 0.2f, false);
        //                                                              ^^
        //                                                              Modifying this to 3f.
        
        foreach (CodeInstruction instruction in instructions) {
            
            if (instruction.opcode == OpCodes.Ldc_R4
             && instruction.operand != null
             && instruction.operand.Equals(1f)) {
                
                instruction.operand = 3f;
                yield return instruction;
                continue;
            }
            
            yield return instruction;
        }
    }
    
    [HarmonyPatch(typeof(PassportManager), "CameraOut")]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> CameraOutTranspiler(IEnumerable<CodeInstruction> instructions) {
        
        //this.dummyCamera.transform.DOLocalMove(new Vector3(0f, 1.05f, 1f), 0.2f, false);
        //                                                              ^^
        //                                                              Modifying this to 3f.
        
        foreach (CodeInstruction instruction in instructions) {
            
            if (instruction.opcode == OpCodes.Ldc_R4
             && instruction.operand != null
             && instruction.operand.Equals(1f)) {
                
                instruction.operand = 3f;
                yield return instruction;
                continue;
            }
            
            yield return instruction;
        }
    }
}
