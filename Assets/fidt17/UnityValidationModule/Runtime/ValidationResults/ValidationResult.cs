using System;

namespace fidt17.UnityValidationModule.Runtime.ValidationResults
{
    public abstract class ValidationResult
    {
        public abstract bool Result { get; }
        public readonly string Message;
        public Object TargetContext;
        
        public ValidationResult(string message = null, Object targetContext = null)
        {
            Message = message;
            TargetContext = targetContext;
        }
    }
}