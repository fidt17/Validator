using System;

namespace fidt17.UnityValidationModule.Runtime.ValidationResults
{
    public class PassResult : ValidationResult
    {
        public override bool Result => true;
        public PassResult(string message = null, Object targetContext = null) : base(message, targetContext) {}
    }
}