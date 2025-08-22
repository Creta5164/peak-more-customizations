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
        public Texture Icon { get; internal set; }
        public override Texture IconTexture
            => Icon;
        public override bool IsValid() => true;
        [field: SerializeField]
        public bool drawUnderEye { get; internal set; }
        [field: SerializeField]
        public Mesh FitMesh { get; internal set; }
        [field: SerializeField]
        public Texture FitMainTexture { get; internal set; }
        [field: SerializeField]
        public Texture FitShoeTexture { get; internal set; }
        [field: SerializeField]
        public Texture FitOverridePantsTexture { get; internal set; }
        [field: SerializeField]
        public Texture fitOverrideHatTexture { get; internal set; }
        [field: SerializeField]
        public bool isSkirt { get; internal set; }
        [field: SerializeField]
        public bool noPants { get; internal set; }
    }
}