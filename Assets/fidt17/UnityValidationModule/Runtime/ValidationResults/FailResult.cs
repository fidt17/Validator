using System;

namespace fidt17.UnityValidationModule.Runtime.ValidationResults
{
    public class FailResult : ValidationResult
    {
        public override bool Result => false;
        public FailResult(string message = null, Object targetContext = null) : base(message, targetContext) {}
    }
}