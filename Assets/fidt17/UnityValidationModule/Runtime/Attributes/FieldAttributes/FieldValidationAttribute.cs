using System.Reflection;
using fidt17.UnityValidationModule.Runtime.ValidationResults;
using Object = System.Object;

namespace fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class FieldValidationAttribute : BaseValidationAttribute
    {
        public bool RecursiveValidation { get; }

        public virtual ValidationResult ValidateField(FieldInfo field, Object target) => new PassResult(targetContext: target);

        public FieldValidationAttribute(bool validateInPrefab = true, bool recursiveValidation = true) : base(validateInPrefab)
        {
            RecursiveValidation = recursiveValidation;
        }
    }
}