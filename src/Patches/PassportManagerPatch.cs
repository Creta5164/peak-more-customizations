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
        
        Plugin.BaseHatCount = hats.Count;
        Plugin.OverrideHatCount = 0;
        
        foreach (var fit in fits) {
            
            if (fit.overrideHat)
                Plugin.OverrideHatCount++;
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
    
    [HarmonyPatch(typeof(PassportManager), "SetActiveButton")]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> SetActiveButton(IEnumerable<CodeInstruction> instructions) {
        
        //else if (this.activeType == Customization.Type.Hat) {
        // num = playerData.customizationData.currentHat;
        // 
        //  if(num > Plugin.BaseHatCount) {
        //      num -= Plugin.OverrideHatCount;
        //  }
        //   ^^
        //   add a check if hat is custom, if so subtract the OverrideHatCount
        // }
        
        var codes = instructions.ToList();
        for (int i = 0; i < codes.Count - 2; i++) {
            
            // Look for: ldloc.0, ldfld customizationData, ldfld currentHat, stloc.0
            if (codes[i].opcode == OpCodes.Ldloc_0
             && codes[i + 1].opcode == OpCodes.Ldfld
             && codes[i + 1].operand.ToString().Contains("customizationData")
             && codes[i + 2].opcode == OpCodes.Ldfld
             && codes[i + 2].operand.ToString().Contains("currentHat")
             && codes[i + 3].opcode == OpCodes.Stloc_1
            ) {
                
                // Output the sequence up to Ldfld
                yield return codes[i];
                yield return codes[i + 1];
                yield return codes[i + 2];
                
                // Inject: if (num > Plugin.BaseHatCount) num -= Plugin.OverrideHatCount;
                // Stack: <currentHat>
                
                // Duplicate the value so we can compare and still have original for subtraction if needed
                yield return new CodeInstruction(OpCodes.Dup);
                
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Property(typeof(Plugin), nameof(Plugin.BaseHatCount)).GetGetMethod(true));
                
                // if (dup > BaseHatCount) -> leave original on stack and continue
                // We'll branch if less than or equal to BaseHatCount to skip subtraction
                var skipLabel = new Label();
                yield return new CodeInstruction(OpCodes.Ble_S) { operand = skipLabel };
                
                // call get_OverrideHatCount and subtract
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Property(typeof(Plugin), nameof(Plugin.OverrideHatCount)).GetGetMethod(true));
                yield return new CodeInstruction(OpCodes.Sub);
                
                // Mark skip label position
                var nop = new CodeInstruction(OpCodes.Nop);
                nop.labels.Add(skipLabel);
                yield return nop;
                
                // Output stloc.0
                yield return codes[i + 3];
                
                i += 3;
                continue;
            }
            yield return codes[i];
        }
        
        // Yield remaining codes
        for (int k = codes.Count - 2; k < codes.Count; k++) {
            
            if (k >= 0) yield return codes[k];
        }
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
