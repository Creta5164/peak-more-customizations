using UnityEngine;

namespace MoreCustomizations.Data {
    
    [CreateAssetMenu(
        menuName = "PEAK More Customizations/Eye",
        fileName = "New Custom Eye",
        order    = int.MinValue
    )]
    public class CustomEyes_V1 : CustomEyesData {
        
        [field: SerializeField]
        public Texture Texture { get; internal set; }
        
        public override Texture IconTexture
            => Texture;
        
        public override bool IsValid()
            => Texture;
    }
}