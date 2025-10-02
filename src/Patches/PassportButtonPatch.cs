
using HarmonyLib;
using System.Reflection;
using UnityEngine;

using Plugin = MoreCustomizations.MoreCustomizationsPlugin;

namespace MoreCustomizations.Patches;

public class PassportButtonPatch {
        
    [HarmonyPatch(typeof(PassportButton), "SetButton")]
    [HarmonyPostfix]
    private static void SetButton(ref int ___currentIndex, CustomizationOption option, int index) {
        
        if (option == null) return;
        
        if (option.type == Customization.Type.Hat && option.overrideHat) {
            
            ___currentIndex = option.overrideHatIndex;
        }
    }
}
