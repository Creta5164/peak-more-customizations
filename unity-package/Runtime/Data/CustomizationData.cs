using UnityEngine;

namespace MoreCustomizations.Data {
    
    public abstract partial class CustomizationData : ScriptableObject {
        
        public abstract Texture IconTexture { get; }
        
        public abstract bool IsValid();
    }
    
    //NOTE: These are for determining customization types statically, must be empty body.
    public abstract partial class CustomAccessoryData : CustomizationData { }
    public abstract partial class CustomEyesData      : CustomizationData { }
    public abstract partial class CustomMouthData     : CustomizationData { }
    public abstract partial class CustomHatData       : CustomizationData { }
    public abstract partial class CustomFitData       : CustomizationData { }
}

#if MOD_AREA
namespace MoreCustomizations.Data {
    
    using static Customization;
    
    partial class CustomizationData {
        
        public abstract Type Type { get; }
    }
    
    partial class CustomAccessoryData {
        
        public sealed override Type Type => Type.Accessory;
    }
    
    partial class CustomEyesData {
        
        public sealed override Type Type => Type.Eyes;
    }
    
    partial class CustomMouthData {
        
        public sealed override Type Type => Type.Mouth;
    }
    
    partial class CustomHatData {
        
        public sealed override Type Type => Type.Hat;
    }

    partial class CustomFitData { 
        public sealed override Type Type => Type.Fit;
    }
}
#endif