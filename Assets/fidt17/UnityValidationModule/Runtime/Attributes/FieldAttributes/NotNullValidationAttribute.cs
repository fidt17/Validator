using System.Reflection;
using fidt17.UnityValidationModule.Runtime.ValidationResults;

namespace fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes
{
    public class NotNullValidationAttribute : FieldValidationAttribute
    {
        public NotNullValidationAttribute() : this(true, true)
        {
            
        }
        
        public NotNullValidationAttribute(bool validateInPrefab = true, bool recursiveValidation = true) : base(validateInPrefab, recursiveValidation)
        {
            
        }

        public override ValidationResult ValidateField(FieldInfo field, System.Object target)
        {
            var value = field.GetValue(target);
            if (UnityExtensions.IsUnityNull(value))
            {
                return new FailResult($"Field \"{field.Name}\" of {target.GetType().Name} is missing.", target);
            }
            else
            {
                return new PassResult(targetContext: target);
            }
        }
    }
}