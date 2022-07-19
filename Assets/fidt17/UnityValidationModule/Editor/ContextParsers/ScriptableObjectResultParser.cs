using System;
using fidt17.UnityValidationModule.Editor.ValidationResultDrawers;
using fidt17.UnityValidationModule.Runtime.ValidationResults;
using UnityEngine;

namespace fidt17.UnityValidationModule.Editor.ContextParsers
{
    [ValidationResultParser(typeof(UnityEngine.Object))]
    public class ScriptableObjectResultParser : DefaultResultParser
    {
        public readonly ScriptableObject ScriptableObject;
        
        public ScriptableObjectResultParser(ValidationResult vl) : base(vl)
        {
            ScriptableObject = vl.TargetContext as ScriptableObject;
            
            if (ScriptableObject == null)
            {
                throw new Exception("Validation result context must be scriptable object");
            }
        }

        public override IValidationResultDrawer GetDrawer() => new ScriptableObjectResultDrawer(this);

        public override bool IsContextValid(object obj) => obj is ScriptableObject;
    }
}