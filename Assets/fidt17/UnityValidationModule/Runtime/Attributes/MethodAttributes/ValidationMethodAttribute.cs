using System;
using System.Reflection;
using fidt17.UnityValidationModule.Runtime.ValidationResults;
using Object = System.Object;

namespace fidt17.UnityValidationModule.Runtime.Attributes.MethodAttributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidationMethodAttribute : BaseMethodValidationAttribute
    {
        private readonly string _message;

        public ValidationMethodAttribute() : this("") { }
        
        public ValidationMethodAttribute(string message = "")
        {
            _message = message;
        }
        
        public override ValidationResult ValidateMethod(MethodInfo method, Object target)
        {
            if (method.ReturnType != typeof(bool))
            {
                return new FailResult($"Incorrect [ValidationMethod] attribute usage on {method.Name} @ {target.GetType()}. Method must return bool.", target);
            }

            if (method.GetParameters().Length != 0)
            {
                return new FailResult($"Incorrect [ValidationMethod] attribute usage on {method.Name} @ {target.GetType()}. Method mustn't have parameters.", target);
            }

            var result = (bool) method.Invoke(target, null);
            return result
                ? (ValidationResult) new PassResult()
                : new FailResult(string.IsNullOrEmpty(_message) ? $"Validation method {method.Name} of {target.GetType()} failed." : _message, target);
        }
    }
}