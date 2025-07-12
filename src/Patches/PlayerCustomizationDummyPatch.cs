using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

using Plugin = MoreCustomizations.MoreCustomizationsPlugin;

namespace MoreCustomizations.Patches;

public class PlayerCustomizationDummyPatch {
    [HarmonyPatch(typeof(PlayerCustomizationDummy), "SetPlayerHat")]
    [HarmonyPrefix]
    private static void SetPlayerHat(PlayerCustomizationDummy __instance, int index) {
        if (Character.localCharacter == null)
            return;

        CharacterCustomization characterCustomization = Character.localCharacter.gameObject.GetComponent<CharacterCustomization>();
        if (characterCustomization == null)
            return;

        // check if character hats more than dummy hats
        if (characterCustomization.refs.playerHats.Length > __instance.refs.playerHats.Length) {
            Plugin.Logger.LogInfo($"{characterCustomization.refs.playerHats.Length}-{__instance.refs.playerHats.Length}");

            var scrHatTransform = characterCustomization.transform.Find(CharacterCustomizationPatch.HAT_PATH);
            var hatTransform = __instance.transform.Find(CharacterCustomizationPatch.HAT_PATH);
            var hatRenderList = new List<Renderer>(__instance.refs.playerHats);
            var layer = __instance.refs.playerHats[0].gameObject.layer;     // layer-[Character]

            for (int i = __instance.refs.playerHats.Length; i < characterCustomization.refs.playerHats.Length; i++) {
                GameObject srcHatInstance = characterCustomization.refs.playerHats[i].gameObject;
                // get the top gameobject of hat 
                while (srcHatInstance.transform.parent != scrHatTransform)
                    srcHatInstance = srcHatInstance.transform.parent.gameObject;
                // copy the top gameobject of hat 
                GameObject hatInstance = Object.Instantiate(srcHatInstance, __instance.refs.playerHats[0].transform.parent, false);
                // get Renderer and add it to list
                Renderer hatInstanceRender = hatInstance.GetComponentInChildren<Renderer>(true);
                hatInstanceRender.gameObject.layer = layer;
                hatRenderList.Add(hatInstanceRender);
            }

            __instance.refs.playerHats = hatRenderList.ToArray();
        }
    }
}
