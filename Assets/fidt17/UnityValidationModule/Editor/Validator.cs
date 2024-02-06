using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using fidt17.UnityValidationModule.Editor.Helpers;
using fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes;
using fidt17.UnityValidationModule.Runtime.Attributes.MethodAttributes;
using fidt17.UnityValidationModule.Runtime.ValidationResults;
using Object = UnityEngine.Object;

[assembly: InternalsVisibleTo("UnityValidationModule.Tests")]
[assembly: InternalsVisibleTo("UnityValidationModule.Tests.Editor")]

namespace fidt17.UnityValidationModule.Editor
{
    internal class Validator
    {
        private struct FieldAttributePair
        {
            public FieldInfo Field;
            public FieldValidationAttribute Attribute;
        }
        
        private struct MethodAttributePair
        {
            public MethodInfo Method;
            public BaseMethodValidationAttribute Attribute;
        }

        private readonly Dictionary<Type, List<FieldAttributePair>> _cachedValidationFields = new Dictionary<Type, List<FieldAttributePair>>();
        private readonly Dictionary<Type, List<MethodAttributePair>> _cachedValidationMethods = new Dictionary<Type, List<MethodAttributePair>>();
        private readonly HashSet<System.Object> _validatedObjects = new HashSet<object>();

        public IEnumerable<ValidationResult> Validate(System.Object validationTarget, HashSet<System.Object> alreadyValidated = null, Object lastContext = null)
        {
            if (validationTarget == null || _validatedObjects.Contains(validationTarget)) yield break;
            _validatedObjects.Add(validationTarget);

            if (alreadyValidated == null)
            {
                alreadyValidated = new HashSet<System.Object>();
            }
            
            if (alreadyValidated.Contains(validationTarget)) yield break;
            
            alreadyValidated.Add(validationTarget);

            var targetType = validationTarget.GetType();
            FindValidationFields(targetType);
            FindValidationMethods(targetType);

            foreach (var validationResult in ValidateFields(validationTarget, lastContext, alreadyValidated))
            {
                yield return validationResult;
            }

            foreach (var validationResult in ValidateMethods(validationTarget))
            {
                yield return validationResult;
            }
        }

        private void FindValidationFields(Type type)
        {
            if (_cachedValidationFields.ContainsKey(type)) return;
            
            _cachedValidationFields[type] = new List<FieldAttributePair>();
            foreach (var field in type.GetValidationFields())
            {
                foreach (var attribute in field.GetAttributesOfType<FieldValidationAttribute>())
                {
                    _cachedValidationFields[type].Add(new FieldAttributePair
                    {
                        Field = field,
                        Attribute = attribute
                    });
                }
            }
        }

        private void FindValidationMethods(Type type)
        {
            if (_cachedValidationMethods.ContainsKey(type)) return;
            
            _cachedValidationMethods[type] = new List<MethodAttributePair>();
            
            foreach (var validationMethod in type.GetValidationMethods())
            {
                foreach (var validationAttribute in validationMethod.GetAttributesOfType<BaseMethodValidationAttribute>())
                {
                    _cachedValidationMethods[type].Add(new MethodAttributePair
                    {
                        Attribute = validationAttribute,
                        Method = validationMethod
                    });
                }
            }
        }

        private IEnumerable<ValidationResult> ValidateFields(System.Object validationTarget, Object lastContext, HashSet<System.Object> alreadyValidated = null)
        {
            var targetType = validationTarget.GetType();
            
            var validationPairs = _cachedValidationFields[targetType];
            for (var i = 0; i < validationPairs.Count; i++)
            {
                var attribute = validationPairs[i].Attribute;
                var field = validationPairs[i].Field;

                if (attribute.ShouldValidate(validationTarget) == false)
                {
                    yield return new PassResult();
                    continue;
                }

                var r = attribute.ValidateField(field, validationTarget);
                if (!(r.TargetContext is Object)) r.TargetContext = lastContext;
                else lastContext = r.TargetContext as Object;

                yield return r;
                    
                if (attribute.RecursiveValidation == false) continue;
                    
                var fieldValue = field.GetValue(validationTarget);
                if (fieldValue == null) continue;
                if (fieldValue is ICollection collection)
                {
                    foreach (var o in collection)
                    {
                        if (o == null) continue;
                        foreach (var validationResult in Validate(o, alreadyValidated, lastContext))
                        {
                            yield return validationResult;
                        }
                    }
                }
                else
                {
                    foreach (var validationResult in Validate(fieldValue, alreadyValidated, lastContext))
                    {
                        yield return validationResult;
                    }
                }
            }
        }

        private IEnumerable<ValidationResult> ValidateMethods(System.Object validationTarget)
        {
            var targetType = validationTarget.GetType();
            var validationMethods = _cachedValidationMethods[targetType];
            for (var i = 0; i < validationMethods.Count; i++)
            {
                var attribute = validationMethods[i].Attribute;
                var method = validationMethods[i].Method;
                
                ValidationResult r;
                try
                {
                    r = attribute.ValidateMethod(method, validationTarget);
                }
                catch (Exception e)
                {
                    r = new FailResult($"Validation Failed with exception:\n {e}", targetContext: validationTarget);
                }
                yield return r;
            }
        }
    }
}