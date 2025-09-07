using System.Collections.Generic;
using UnityEngine;

namespace MoreCustomizations.Data {
    
    [CreateAssetMenu(
        menuName = "PEAK More Customizations/Hat",
        fileName = "New Custom Hat",
        order    = int.MinValue
    )]
    public class CustomHat_V1 : CustomHatData {
        
        [field: SerializeField]
        public Texture Icon { get; internal set; }
        
        [field: SerializeField]
        public GameObject Prefab { get; internal set; }
        
        [field: SerializeField]
        public Texture MainTexture { get; internal set; }
        
        [field: SerializeField]
        public Texture SubTexture { get; internal set; }
        
        [field: SerializeField]
        public Vector3 PositionOffset { get; internal set; }
        
        [field: SerializeField]
        public Vector3 EulerAngleOffset { get; internal set; }
        
        public Vector3 SwizzledPositionOffset
            => new Vector3(
                PositionOffset.x,
                -PositionOffset.z,
                PositionOffset.y
            );
        
        public Vector3 SwizzledRotationOffset
            => new Vector3(
                EulerAngleOffset.x,
                -EulerAngleOffset.z,
                -EulerAngleOffset.y
            );
        
        public override Texture IconTexture
            => Icon;
        
        public override IEnumerable<ValidateStatus> GetValidateContentStatuses() {
            
            bool isInvalid = false;
            
            if (!Icon) {
                
                yield return ValidateStatus.Error($"{nameof(Icon)} is empty.");
                isInvalid = true;
            }
            
            if (!Prefab) {
                
                yield return ValidateStatus.Error($"{nameof(Prefab)} is empty.");
                isInvalid = true;
            }
            
            if (isInvalid)
                yield break;
            
            yield return ValidateStatus.Valid;
        }
    }
}