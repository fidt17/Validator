using System;
using System.Reflection;
using fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes;
using fidt17.UnityValidationModule.Runtime.ValidationResults;
using UnityEngine;

namespace fidt17.Tests.Example_Extension
{
    public class GreenColorValidationAttribute : FieldValidationAttribute
    {
        public override ValidationResult ValidateField(FieldInfo field, object target)
        {
            if (typeof(Color).IsAssignableFrom(field.FieldType) == false) throw new Exception($"Invalid type on [{typeof(GreenColorValidationAttribute)}]");

            var value = (Color) field.GetValue(target);

            if (value != Color.green)
            {
                return new FailResult($"Assigned color is: {value.ToString()}. Yet should be green", targetContext: target);
            }
            return new PassResult();
        }
    }
}