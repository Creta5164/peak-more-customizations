using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using MoreCustomizations.Data;
using UnityEngine;

using Plugin = MoreCustomizations.MoreCustomizationsPlugin;

namespace MoreCustomizations.Patches;

public class CharacterCustomizationPatch {

    public const string HAT_PATH = @"Scout/" + Helpers.CustomizationRefsHelper.REF_TO_HATS_PATH;
    private static Shader _characterShader;
    private static MaterialPropertyBlock _materialPropertyBlock = new();
    private static readonly Vector3 INITIAL_HAT_OFFSET = new(0, 0.2f, 6.0f);
    
    [HarmonyPatch(typeof(CharacterCustomization), "Awake")]
    [HarmonyPostfix]
    private static void Awake(CharacterCustomization __instance) {
        
        var allCustomizationsData = Plugin.AllCustomizationsData;
        
        if (allCustomizationsData == null) {
            
            Plugin.Logger.LogError("Customizations data are not loaded!");
            return;
        }
        
        if (allCustomizationsData.Count == 0) {
            
            Plugin.Logger.LogWarning("There's no customizations data.");
            return;
        }
        
        if (!_characterShader)
            _characterShader = Shader.Find("W/Character");

        //Hats
        if (allCustomizationsData.TryGetValue(Customization.Type.Hat, out var customizationsData)) {
            
            var hatTransform = __instance.transform.Find(HAT_PATH);
            
            if (!hatTransform) {
                
                Plugin.Logger.LogError($"Something went wrong in {nameof(CharacterCustomizationPatch)}...");
                return;
            }
            
            var instantiatedHats = new List<Renderer>(__instance.refs.playerHats);
            
            foreach (var customizationData in customizationsData.OfType<CustomHat_V1>()) {
                
                if (!customizationData || !customizationData.IsValid)
                    continue;
                
                GameObject hatInstance = Object.Instantiate(customizationData.Prefab, hatTransform, false);
                Renderer   hatInstanceRenderer = hatInstance.GetComponentInChildren<Renderer>();
                
                if (!hatInstanceRenderer) {
                    
                    Plugin.Logger.LogError(
                        $"Cannot find Renderer component of customization data '{customizationData.name}'."
                    );
                    Object.Destroy(hatInstance);
                    continue;
                }
                
                hatInstance.transform.localPosition
                    = INITIAL_HAT_OFFSET
                    + customizationData.SwizzledPositionOffset;
                
                hatInstance.transform.localRotation
                    = Quaternion.Euler(customizationData.SwizzledRotationOffset)
                    * Quaternion.AngleAxis(90, Vector3.right);
                
                hatInstanceRenderer.gameObject.SetActive(false);
                hatInstanceRenderer.name = customizationData.name;
                
                var mainMaterial = new Material(_characterShader);
                var subMaterial  = new Material(_characterShader);
                
                if (customizationData.MainTexture)
                    mainMaterial.SetTexture("_MainTex", customizationData.MainTexture);
                
                if (customizationData.SubTexture)
                    subMaterial.SetTexture("_MainTex", customizationData.SubTexture);
                
                hatInstanceRenderer.materials = [
                    mainMaterial,
                    subMaterial
                ];
                
                instantiatedHats.Add(hatInstanceRenderer);
            }
            
            __instance.refs.playerHats = instantiatedHats.ToArray();
        }
    }
	
    [HarmonyPatch(typeof(CharacterCustomization), "SetCharacterHat")]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> SetCharacterHatTranspiler(IEnumerable<CodeInstruction> instructions) {
        // if (index >= Singleton<Customization>.Instance.hats.Length)
        //                                                         ^^
        //                                 add overrideHatCount to length
		
        CodeInstruction prev = null;
        foreach (var instruction in instructions) {
			
            if (prev != null && prev.opcode == OpCodes.Ldlen && instruction.opcode == OpCodes.Conv_I4) {
				
				// yield Ldlen
				yield return prev;
				// inject: ldsfld Plugin.OverrideHatCount; add
				yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Plugin), "overrideHatCount"));
				yield return new CodeInstruction(OpCodes.Add);
				// yield Conv_I4
				yield return instruction;
				
				prev = null;
				continue;
			}
			
            if (prev != null)
				yield return prev;
			
            prev = instruction;
        }
        if (prev != null)
            yield return prev;
    }
}
