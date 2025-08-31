using UnityEngine;

namespace MoreCustomizations.Data {
    
    public struct ValidateStatus {
        
        public enum ValidateType {
            
            Valid,
            Warning,
            Error
        }
        
        public static readonly ValidateStatus Valid = new("This asset is valid.", ValidateType.Valid);
        
        public readonly string Message;
        public readonly ValidateType Type;
        
        public ValidateStatus(string message, ValidateType type) {
            
            Message = message;
            Type    = type;
        }
        
        public static ValidateStatus Warning(string message)
            => new(message, ValidateType.Warning);
        
        public static ValidateStatus Error(string message)
            => new(message, ValidateType.Error);
    }
}