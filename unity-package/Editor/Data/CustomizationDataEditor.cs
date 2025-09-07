using UnityEngine;
using UnityEditor;
using MoreCustomizations.Data;

namespace MoreCustomizationsEditor.Data {
    
    [CustomEditor(typeof(CustomizationData), true)]
    public class CustomizationDataEditor : Editor {
        
        public override void OnInspectorGUI() {
            
            base.OnInspectorGUI();
            
            var target = this.target as CustomizationData;
            
            if (!target)
                return;
            
            foreach (ValidateStatus validateStatus in target.GetValidateContentStatuses()) {
                
                var validateHelpBoxType = validateStatus.Type switch {
                    
                    ValidateStatus.ValidateType.Warning => MessageType.Warning,
                    ValidateStatus.ValidateType.Error   => MessageType.Error,
                    
                    _ => MessageType.Info
                };
                
                EditorGUILayout.HelpBox(validateStatus.Message, validateHelpBoxType);
            }
        }
    }
}