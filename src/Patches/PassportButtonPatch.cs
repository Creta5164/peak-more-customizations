
using HarmonyLib;

using Plugin = MoreCustomizations.MoreCustomizationsPlugin;

namespace MoreCustomizations.Patches;

public class PassportButtonPatch {
    
    [HarmonyPatch(typeof(PassportButton), "SetButton")]
    [HarmonyPostfix]
    private static void SetButton(ref int ___currentIndex, CustomizationOption option) {
        
        if (option == null) return;
        
        if (option.type == Customization.Type.Hat && ___currentIndex >= Plugin.BaseHatCount) {
            
            ___currentIndex += Plugin.OverrideHatCount;
        }
    }
}
