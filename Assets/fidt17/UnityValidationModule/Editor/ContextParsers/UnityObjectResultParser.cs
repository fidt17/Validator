using System;
using fidt17.UnityValidationModule.Editor.Helpers;
using fidt17.UnityValidationModule.Runtime.ValidationResults;

namespace fidt17.UnityValidationModule.Editor.ContextParsers
{
    internal abstract class UnityObjectResultParser : DefaultResultParser
    {
        public readonly string ContextName;
        
        protected UnityObjectResultParser(ValidationResult vl) : base(vl)
        {
            ContextName = FormContextName(GetUnityContext(vl));
        }

        protected static UnityEngine.Object GetUnityContext(ValidationResult vl)
        {
            var unityContext = vl.TargetContext as UnityEngine.Object;
            if (unityContext == null) throw new Exception("Could not cast validation result's context object to UnityEngine.Object");
            return unityContext;
        }
        
        private static string FormContextName(UnityEngine.Object unityContext)
        {
            var contextName = "";
            contextName += unityContext.name;
            if (contextName.Length > ValidatorEditorConstants.MaxLengthOfContextName)
            {
                contextName = contextName.Substring(0, ValidatorEditorConstants.MaxLengthOfContextName - 3) + "...";
            }
            return contextName;
        }
    }
}