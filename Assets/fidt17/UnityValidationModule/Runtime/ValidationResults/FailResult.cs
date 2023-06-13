using System;

namespace fidt17.UnityValidationModule.Runtime.ValidationResults
{
    public class FailResult : ValidationResult
    {
        public override bool Result => false;
        public FailResult(string message, Object targetContext) : base(message, targetContext) {}
    }
}