using System.Collections.Generic;
using UnityEngine;

namespace MoreCustomizations.Data {
    
    [CreateAssetMenu(
        menuName = "PEAK More Customizations/Mouth",
        fileName = "New Custom Mouth",
        order    = int.MinValue
    )]
    public class CustomMouth_V1 : CustomMouthData {
        
        [field: SerializeField]
        public Texture Texture { get; internal set; }
        
        public override Texture IconTexture
            => Texture;
        
        public override IEnumerable<ValidateStatus> GetValidateContentStatuses() {
            
            if (!Texture) {
                
                yield return ValidateStatus.Error($"{nameof(Texture)} is empty.");
                yield break;
            }
            
            yield return ValidateStatus.Valid;
        }
    }
}