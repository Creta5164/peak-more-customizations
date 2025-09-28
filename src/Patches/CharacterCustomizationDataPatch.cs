using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

using Plugin = MoreCustomizations.MoreCustomizationsPlugin;

namespace MoreCustomizations.Patches;

public class CharacterCustomizationDataPatch {
    
    [HarmonyPatch(typeof(CharacterCustomizationData), "CorrectValues")]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> CorrectValuesTranspiler(IEnumerable<CodeInstruction> instructions) {
        
        // if (this.currentHat >= Singleton<Customization>.Instance.hats.Length)
        //                                                                    ^^
        //                                            add overrideHatCount to length
        
        var codes = instructions.ToList();
        for (int i = 0; i < codes.Count - 2; i++) {
            
            // Look for: ldarg.0, ldfld currentHat, call get_Instance, ldfld hats, ldlen, conv.i4
            if (codes[i].opcode == OpCodes.Ldarg_0
             && codes[i + 1].opcode == OpCodes.Ldfld
             && codes[i + 1].operand.ToString().Contains("currentHat")
             && codes[i + 2].opcode == OpCodes.Call
             && codes[i + 2].operand.ToString().Contains("get_Instance")
             && codes[i + 3].opcode == OpCodes.Ldfld
             && codes[i + 3].operand.ToString().Contains("hats")
             && codes[i + 4].opcode == OpCodes.Ldlen
             && codes[i + 5].opcode == OpCodes.Conv_I4
            ) {
                
                // Output the sequence up to ldlen
                yield return codes[i];
                yield return codes[i + 1];
                yield return codes[i + 2];
                yield return codes[i + 3];
                yield return codes[i + 4];
                
                // Inject: ldsfld overrideHatCount; add
                yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Plugin), "overrideHatCount"));
                yield return new CodeInstruction(OpCodes.Add);
                
                // Output conv.i4
                yield return codes[i + 5];
                
                i += 5;
                continue;
            }
            yield return codes[i];
        }
        
        // Yield remaining codes
        for (int k = codes.Count - 2; k < codes.Count; k++) {
            
            if (k >= 0) yield return codes[k];
        }
    }
}
