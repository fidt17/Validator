using System.Reflection;
using fidt17.UnityValidationModule.Runtime.ValidationResults;
using UnityEngine.Events;

namespace fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes
{
    public class UnityEventValidationAttribute : NotNullValidationAttribute
    {
        public UnityEventValidationAttribute() : this(true, true)
        {
            
        }
        
        public UnityEventValidationAttribute(bool validateInPrefab = true, bool recursiveValidation = true) : base(validateInPrefab, recursiveValidation)
        {
            
        }

        public override ValidationResult ValidateField(FieldInfo field, System.Object target)
        {
            var baseResult = base.ValidateField(field, target);
            if (baseResult.Result == false) return baseResult;

            var unityEvent = field.GetValue(target) as UnityEvent;
            if (unityEvent == null)
            {
                return new FailResult($"Incorrect [UnityEventValidationAttribute] attribute usage on {field.Name} @ {target.GetType()}. Field must derive from UnityEvent", target);
            }

            if (UnityExtensions.IsUnityEventValid(unityEvent) == false)
            {
                return new FailResult($"UnityEvent \"{field.Name}\" of {target.GetType().Name} is invalid.", target);
            }
            
            return new PassResult(targetContext: target);
        }
    }
}