using UnityEngine;

namespace MoreCustomizations.Data
{
    [CreateAssetMenu(
        menuName = "PEAK More Customizations/Fit",
        fileName = "New Custom Fit",
        order = int.MinValue
    )]
    public class CustomFit_V1 : CustomFitData
    {
        [field: SerializeField]
        [field: Tooltip(
            "The texture for the icon in the Passport"
        )]
        public Texture Icon { get; internal set; }
        
        public override Texture IconTexture
            => Icon;
        
        public override bool IsValid()
            => true;
        
        [field: SerializeField]
        [field: Tooltip(
            "If set to true, the eyes are rendered on top of the outfit at all times"
        )]
        public bool DrawUnderEye { get; internal set; }
        
        [field: SerializeField]
        [field: Tooltip(
            "The Mesh of the outfit we imported earlier"
        )]
        public Mesh FitMesh { get; internal set; }
        
        [field: SerializeField]
        [field: Tooltip(
            "The color texture for the second material slot"
        )]
        public Texture FitMainTexture { get; internal set; }
        
        [field: SerializeField]
        [field: Tooltip(
            "The color texture for the third material slot"
        )]
        public Texture FitShoeTexture { get; internal set; }
        
        [field: SerializeField]
        [field: Tooltip(
            "The color texture that can override the pants material. Leave blank to use the default pants material."
        )]
        public Texture FitOverridePantsTexture { get; internal set; }
        
        [field: SerializeField]
        [field: Tooltip(
            "The color texture that can override the hat material. Only works on hats 0 and 1. Leave blank to use the default hat material."
        )]
        public Texture FitOverrideHatTexture { get; internal set; }
        
        [field: SerializeField]
        [field: Tooltip(
            "Indicated whether the skirt or shorts mesh should be enabled for this outfit variant."
        )]
        public bool IsSkirt { get; internal set; }
        
        [field: SerializeField]
        [field: Tooltip(
            "If true, hides both the skirt and shorts mesh for this outfit. Used for outfits like the Astronaut suit, that have no gendered variants."
        )]
        public bool NoPants { get; internal set; }


    }
}