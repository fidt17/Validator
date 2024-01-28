using System.Collections;
using System.Reflection;
using fidt17.UnityValidationModule.Runtime.ValidationResults;

namespace fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes
{
    public class NotNullValidationAttribute : FieldValidationAttribute
    {
        public NotNullValidationAttribute() : this(true) { }
        
        public NotNullValidationAttribute(bool validateInPrefab = true, bool recursiveValidation = true) : base(validateInPrefab, recursiveValidation) { }

        public override ValidationResult ValidateField(FieldInfo field, System.Object target)
        {
            return field.GetValue(target) is ICollection ? ValidateCollection(field, target) : ValidateSingleValue(field, target);
        }

        private ValidationResult ValidateSingleValue(FieldInfo field, System.Object target)
        {
            var value = field.GetValue(target);
            if (UnityExtensions.IsUnityNull(value))
            {
                return new FailResult($"Field \"{field.Name}\" of {target.GetType().Name} is missing.", target);
            }

            return new PassResult(targetContext: target);
        }

        private ValidationResult ValidateCollection(FieldInfo field, System.Object target)
        {
            if (!(field.GetValue(target) is ICollection collection))
            {
                return new FailResult($"Incorrect [NotNullValidation] attribute usage on {field.Name} @ {target.GetType()}. Field must derive from ICollection", target);
            }
            
            var baseResult = base.ValidateField(field, target);
            if (baseResult.Result == false) return baseResult;
            
            foreach (var element in collection)
            {
                if (UnityExtensions.IsUnityNull(element))
                {
                    return new FailResult($"Element of collection of type {field.FieldType} on {target.GetType()} is missing.", target);
                }
            }

            return new PassResult(targetContext: target);
        }
    }
}