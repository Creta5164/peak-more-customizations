using UnityEngine;

namespace MoreCustomizations.Data {
    
    [CreateAssetMenu(
        menuName = "PEAK More Customizations/Accessory",
        fileName = "New Custom Accessory",
        order    = int.MinValue
    )]
    public class CustomAccessory_V1 : CustomAccessoryData {
        
        [field: SerializeField]
        public Texture Texture { get; internal set; }
        
        public override Texture IconTexture
            => Texture;
        
        public override bool IsValid()
            => Texture;
    }
}