using UnityEngine;
public static class FitMaterialFallback
{
    //parameters scraped from 'M_Scout_Seagull'.
    private static readonly (string, float)[] floats = new (string, float)[] {
                                                                ("_AddPrecomputedVelocity", 0),
                                                                ("_AddSpecular", 0),
                                                                ("_AlphaClip", 0),
                                                                ("_AlphaCutoff", 0.5f),
                                                                ("_AlphaToMask", 0),
                                                                ("_BaseSmooth", 0),
                                                                ("_Blend", 0),
                                                                ("_BlendModePreserveSpecular", 1),
                                                                ("_Brightness", 1),
                                                                ("_BumpScale", 1),
                                                                ("_ClearCoatMask", 0),
                                                                ("_ClearCoatSmoothness", 0),
                                                                ("_Cull", 2),
                                                                ("_Cutoff", 0.5f),
                                                                ("_DetailAlbedoMapScale", 1),
                                                                ("_DetailNormalMapScale", 1),
                                                                ("_DstBlend", 0),
                                                                ("_DstBlendAlpha", 0),
                                                                ("_EnvironmentReflections", 1),
                                                                ("_Flip1", 0),
                                                                ("_Flip2", 0),
                                                                ("_Flip3", 0),
                                                                ("_Flip4", 0),
                                                                ("_FlipHeightMask", 0),
                                                                ("_GlossMapScale", 0),
                                                                ("_Glossiness", 0),
                                                                ("_GlossyReflections", 0),
                                                                ("_Glow", 0),
                                                                ("_Height1", 0),
                                                                ("_Height2", 0),
                                                                ("_Height3", 0),
                                                                ("_Height4", 0),
                                                                ("_HueStr", 1),
                                                                ("_INTERACTABLE", 0),
                                                                ("_Interactable", 0),
                                                                ("_Metalic", 0),
                                                                ("_Metallic", 0),
                                                                ("_Mode", 0),
                                                                ("_OcclusionStrength", 1),
                                                                ("_Opacity", 1),
                                                                ("_Outline", 1),
                                                                ("_Parallax", 0.005f),
                                                                ("_QueueControl", 0),
                                                                ("_QueueOffset", 0),
                                                                ("_ReceiveShadows", 1),
                                                                ("_ReflectionBoost", 0),
                                                                ("_Smooth1", 0),
                                                                ("_Smooth2", 0),
                                                                ("_Smooth3", 0),
                                                                ("_Smooth4", 0),
                                                                ("_Smoothness", 0.5f),
                                                                ("_SmoothnessTextureChannel", 0),
                                                                ("_SpecularHighlights", 1),
                                                                ("_SrcBlend", 1),
                                                                ("_SrcBlendAlpha", 1),
                                                                ("_StatusGlow", 0),
                                                                ("_Surface", 0),
                                                                ("_ToggleSwitch0", 0),
                                                                ("_Triplanar1", 0),
                                                                ("_Triplanar1On", 0),
                                                                ("_Triplanar2", 0),
                                                                ("_Triplanar2On", 0),
                                                                ("_Triplanar3", 0),
                                                                ("_Triplanar3On", 0),
                                                                ("_Triplanar4", 0),
                                                                ("_Triplanar4On", 0),
                                                                ("_UV1", 1.15f),
                                                                ("_UV2", 0.27f),
                                                                ("_UV3", 1),
                                                                ("_UV4", 1),
                                                                ("_UVSec", 0),
                                                                ("_UseHeightMask1", 0),
                                                                ("_UseHeightMask2", 0),
                                                                ("_UseHeightMask3", 0),
                                                                ("_UseHeightMask4", 0),
                                                                ("_UseRawVertexColor", 0),
                                                                ("_UseSkinTone", 0),
                                                                ("_UseTextureAlpha", 0),
                                                                ("_UseVertexColor", 0),
                                                                ("_VertexColor", 1),
                                                                ("_VertexGhost", 0),
                                                                ("_WorkflowMode", 1),
                                                                ("_ZWrite", 1)
                                                            };
    private static readonly (string, Color)[] colors = new (string, Color)[] {
                                                                ("_BaseColor", new Color(r: 1, g: 1, b: 1, a: 1)),
                                                                ("_Color", new Color(r: 1, g: 1, b: 1, a: 1)),
                                                                ("_Color0", new Color(r: 4, g: 2.191751f, b: 0, a: 0)),
                                                                ("_Color1", new Color(r: 0.8584906f, g: 0.6924617f, b: 0.6924617f, a: 1)),
                                                                ("_Color2", new Color(r: 0.6509434f, g: 0.58534664f, b: 0.5066305f, a: 0.6509804f)),
                                                                ("_Color3", new Color(r: 1, g: 1, b: 1, a: 0)),
                                                                ("_Color4", new Color(r: 1, g: 1, b: 1, a: 0)),
                                                                ("_EmissionColor", new Color(r: 0, g: 0, b: 0, a: 1)),
                                                                ("_HeightMaskSmoothness", new Color(r: 0, g: 0, b: 0, a: 0)),
                                                                ("_HueShifts", new Color(r: 0, g: 0, b: 0, a: 0)),
                                                                ("_Remap1", new Color(r: 0.35514024f, g: 0.551402f, b: 0, a: 0)),
                                                                ("_Remap2", new Color(r: 0.31775698f, g: 0.71962625f, b: 0, a: 0)),
                                                                ("_Remap3", new Color(r: 0, g: 1, b: 0, a: 0)),
                                                                ("_Remap4", new Color(r: 0, g: 1, b: 0, a: 0)),
                                                                ("_SkinColor", new Color(r: 1, g: 1, b: 1, a: 0)),
                                                                ("_SpecColor", new Color(r: 0.19999996f, g: 0.19999996f, b: 0.19999996f, a: 1)),
                                                                ("_StatusColor", new Color(r: 2.600001f, g: 0.5094339f, b: 4, a: 1)),
                                                                ("_Tint", new Color(r: 1, g: 1, b: 1, a: 1))
                                                            };

    /// <summary>
    /// Fallback material for character model. Missing some texture references for _Texture1 (cf8299178f38cfc4bbc8cad77f945042) and _Texture2 (8eb990cc0cb1e17439402d1501c7f607)
    /// </summary>
    public static Material MaterialTemplate
    {
        get
        {
            var material = new Material(Shader.Find("W/Character"));
            foreach (var (name, @float) in floats)
                material.SetFloat(name, @float);
            foreach (var (name, color) in colors)
                material.SetColor(name, color);
            return material;
        }
    }
}