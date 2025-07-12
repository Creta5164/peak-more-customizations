using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using MoreCustomizations.Data;

namespace MoreCustomizations;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public partial class MoreCustomizationsPlugin : BaseUnityPlugin {
    
    internal static MoreCustomizationsPlugin Singleton { get; private set; }
    
    internal static new ManualLogSource Logger;
    
    internal static Harmony _patcher = new(MyPluginInfo.PLUGIN_GUID);
    
    public static IReadOnlyDictionary<Customization.Type, IReadOnlyList<CustomizationData>> AllCustomizationsData { get; private set; }
    
    private void Awake() {
        
        Singleton = this;
        
        Logger = base.Logger;
        
        LoadAllCustomizations();
        
        Logger.LogInfo("Patching methods...");
        _patcher.PatchAll(typeof(Patches.PassportManagerPatch));
        _patcher.PatchAll(typeof(Patches.CharacterCustomizationPatch));
        _patcher.PatchAll(typeof(Patches.PlayerCustomizationDummyPatch));

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void LoadAllCustomizations() {
        
        AllCustomizationsData = null;
        var loadedCustomizationsData = new Dictionary<Customization.Type, List<CustomizationData>>();
        
        string rootPath = Path.Combine(Paths.BepInExRootPath, "plugins");
        
        string[] peakCustomAssetBundlePaths = Directory.GetFiles(rootPath, "*.pcab", SearchOption.AllDirectories);
        
        if (peakCustomAssetBundlePaths.Length == 0)
            throw new FileNotFoundException($"No customization files found in '{Paths.PluginPath}'.");
        
        Logger.LogInfo($"Found {peakCustomAssetBundlePaths.Length} possible contents.");
        
        var fetchedCustomizationData = new List<CustomizationData>();
        
        foreach (string peakCustomAssetBundlePath in peakCustomAssetBundlePaths) {
            
            string trimmedBundleFilePath = peakCustomAssetBundlePath[Paths.PluginPath.Length..];
            string bundleFileName        = Path.GetFileNameWithoutExtension(peakCustomAssetBundlePath);
            
            try {
                
                var assetBundle = AssetBundle.LoadFromFile(peakCustomAssetBundlePath);
                
                Logger.LogInfo($"Asset bundle '{bundleFileName}' loaded! ({trimmedBundleFilePath})");
                Logger.LogInfo($"Catalog list :");
                
                foreach (string assetPath in assetBundle.GetAllAssetNames())
                    Logger.LogInfo($"- {assetPath}");
                
                fetchedCustomizationData.AddRange(assetBundle.LoadAllAssets<CustomizationData>());
                
            } catch (Exception ex) {
                
                Logger.LogError($"Error occurred while loading custom asset bundle. ({trimmedBundleFilePath})");
                Logger.LogError(ex.Message);
                Logger.LogError(ex.StackTrace);
            }
        }
        
        Logger.LogInfo($"Loading {fetchedCustomizationData.Count} customizations...");
        
        foreach (CustomizationData customizationData in fetchedCustomizationData) {
            
            var type = customizationData.Type;
            
            if (!loadedCustomizationsData.TryGetValue(type, out var loadedCustomizations)) {
                
                loadedCustomizations = new List<CustomizationData>();
                loadedCustomizationsData[type] = loadedCustomizations;
            }
            
            loadedCustomizations.Add(customizationData);
            Logger.LogInfo($"Loaded '{customizationData.name}'!");
        }
        
        Logger.LogInfo("Done!");
        
        AllCustomizationsData = loadedCustomizationsData.ToDictionary(
            key   => key.Key,
            value => value.Value as IReadOnlyList<CustomizationData>
        );
    }
}