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
    
    /// <summary>Number of base hats in the game.</summary>
    public static int BaseHatCount { get; internal set; } = 0;
    
    /// <summary>Number of extra placeholder hats introduced by fits that override hats.</summary>
    public static int OverrideHatCount { get; internal set; } = 0;
    
    public static IReadOnlyDictionary<Customization.Type, IReadOnlyList<CustomizationData>> AllCustomizationsData { get; private set; }
    
    private void Awake() {
        
        Singleton = this;
        
        Logger = base.Logger;
        
        LoadAllCustomizations();
        
        Logger.LogInfo("Patching methods...");
        _patcher.PatchAll(typeof(Patches.PassportManagerPatch));
        _patcher.PatchAll(typeof(Patches.PassportButtonPatch));
        _patcher.PatchAll(typeof(Patches.CharacterCustomizationPatch));
        _patcher.PatchAll(typeof(Patches.CharacterCustomizationDataPatch));
        _patcher.PatchAll(typeof(Patches.PlayerCustomizationDummyPatch));
        _patcher.PatchAll(typeof(Patches.PeakHandlePatch));

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
                
                var bundledCustomizations = assetBundle.LoadAllAssets<CustomizationData>();
                
                foreach (var bundledCustomization in bundledCustomizations)
                    bundledCustomization.name = $"{trimmedBundleFilePath.GetHashCode()}_{bundledCustomization.name}";
                
                fetchedCustomizationData.AddRange(bundledCustomizations);
                
            } catch (Exception ex) {
                
                Logger.LogError($"Error occurred while loading custom asset bundle. ({trimmedBundleFilePath})");
                Logger.LogError(ex.Message);
                Logger.LogError(ex.StackTrace);
            }
        }
        
        Logger.LogInfo($"Loading {fetchedCustomizationData.Count} customizations...");
        
        foreach (CustomizationData customizationData in fetchedCustomizationData) {
            
            var warningLevelValidationStatuses = customizationData.GetValidateContentStatuses()
                .Where(static status => status.Type != ValidateStatus.ValidateType.Valid);
            
            bool isInvalid = false;
            
            foreach (ValidateStatus validationStatus in warningLevelValidationStatuses) {
                
                isInvalid |= validationStatus.Type == ValidateStatus.ValidateType.Error;
                
                switch (validationStatus.Type) {
                    
                    case ValidateStatus.ValidateType.Warning:
                        
                        Logger.LogWarning($"[Validation Warning:{customizationData.name}] {validationStatus.Message}");
                        break;
                    
                    case ValidateStatus.ValidateType.Error:
                        
                        Logger.LogError($"[Validation Error:{customizationData.name}] {validationStatus.Message}");
                        break;
                }
            }
            
            if (isInvalid)
                continue;
            
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
