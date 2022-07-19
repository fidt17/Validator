using System;
using System.Reflection;
using fidt17.UnityValidationModule.Runtime.ValidationResults;

namespace fidt17.UnityValidationModule.Runtime.Attributes.MethodAttributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class BaseMethodValidationAttribute : BaseValidationAttribute
    {
        public abstract ValidationResult ValidateMethod(MethodInfo method, System.Object target);

        protected BaseMethodValidationAttribute(bool validateInPrefab = true) : base(validateInPrefab)
        {
        }
    }
}