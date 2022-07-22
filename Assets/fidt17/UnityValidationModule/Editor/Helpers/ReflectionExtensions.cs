using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using fidt17.UnityValidationModule.Editor.ContextParsers;
using fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes;
using fidt17.UnityValidationModule.Runtime.Attributes.MethodAttributes;
using fidt17.UnityValidationModule.Runtime.ValidationResults;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace fidt17.UnityValidationModule.Editor.Helpers
{
    public static class ReflectionExtensions
    {
        private const BindingFlags b = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private static List<Type> _cachedParserTypes;
        
        /// <summary>
        /// Yields all attributes of type T on provided member
        /// </summary>
        public static IEnumerable<T> GetAttributesOfType<T>(this MemberInfo member)
        {
            if (member == null) throw new ArgumentNullException();
            
            var attributes = Attribute.GetCustomAttributes(member);
            foreach (var attribute in attributes)
            {
                if (!(attribute is T targetAttribute)) continue;
                yield return targetAttribute;
            }
        }
        
        /// <summary>
        /// Yields all fields of provided type with an attribute that derives from [FieldValidationAttribute]
        /// Includes private fields of all base types
        /// </summary>
        public static IEnumerable<FieldInfo> GetValidationFields(this Type t)
        {
            if (t == null) throw new ArgumentNullException();

            var results = new HashSet<string>();
            
            foreach (var field in t.GetFields(b))
            {
                if (!Attribute.GetCustomAttributes(field).Any(x => x is FieldValidationAttribute)) continue;
                if (results.Contains(field.Name)) continue;
                results.Add(field.Name);
                yield return field;
            }
            
            if (t.BaseType != null)
            {
                foreach (var field in t.BaseType.GetValidationFields())
                {
                    if (results.Contains(field.Name)) continue;
                    results.Add(field.Name);
                    yield return field;
                }                
            }
        }

        /// <summary>
        /// Yields all methods of provided type with an attribute that derives from [BaseMethodValidationAttribute]
        /// Includes private methods of all base types
        /// </summary>
        public static IEnumerable<MethodInfo> GetValidationMethods(this Type t)
        {
            if (t == null) throw new ArgumentNullException();

            var results = new HashSet<string>();
            
            foreach (var method in t.GetMethods(b))
            {
                if (!Attribute.GetCustomAttributes(method).Any(x => x is BaseMethodValidationAttribute)) continue;
                if (results.Contains(method.Name)) continue;
                results.Add(method.Name);
                yield return method;
            }

            if (t.BaseType != null)
            {
                foreach (var method in t.BaseType.GetValidationMethods())
                {
                    if (results.Contains(method.Name)) continue;
                    results.Add(method.Name);
                    yield return method;
                }   
            }
        }

        /// <summary>
        /// Returns IValidationResultParser for provided validation result.
        /// Search is performed in the same assembly where ValidationResult's context lies.
        /// Search is performed in the same assembly where provided context parser type lies.
        /// Throws an Exception if search fails.
        /// </summary>
        public static BaseResultParser GetContextParserFor(ValidationResult vl)
        {
            if (vl == null) throw new ArgumentNullException();
            if (vl.TargetContext == null) return new DefaultResultParser(vl);

            if (_cachedParserTypes == null)
            {
                _cachedParserTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(BaseResultParser).IsAssignableFrom(p) && p.IsClass).ToList();
            }

            BaseResultParser bestParser = null;
            var minInheritanceDistance = int.MaxValue;

            foreach (var potentialParserType in _cachedParserTypes)
            {
                foreach (var attribute in potentialParserType.GetAttributesOfType<ValidationResultParserAttribute>())
                {
                    if (attribute.ValueContextType.IsInstanceOfType(vl.TargetContext) == false) continue;
                    
                    var inhDistance = CalculateInheritanceDistance(vl.TargetContext.GetType(), attribute.ValueContextType);
                    if (inhDistance >= minInheritanceDistance) continue;

                    var constructor = potentialParserType.GetConstructor(new[] { typeof(ValidationResult) });
                    if (constructor == null) continue;

                    try
                    {
                        var p = (BaseResultParser)constructor.Invoke(new object[] { vl });
                        if (!p.IsContextValid(vl.TargetContext)) continue;
                        bestParser = p;
                        minInheritanceDistance = inhDistance;
                    }
                    catch (Exception e)
                    {
                        // ignored
                    }
                }
            }
            
            return bestParser == null ? new DefaultResultParser(vl) : bestParser;
        }
        
        //'to' must be assignable from 'from'
        //If ClassA -> ClassB -> ClassC
        //then InheritanceDistance
        //from ClassC to ClassA is 3,
        //from ClassC to Class B is 1
        //from ClassC to Class C is 0
        private static int CalculateInheritanceDistance(Type from, Type to)
        {
            if (to.IsAssignableFrom(from) == false) throw new Exception($"{from.ToString()} must be assignable from {to.ToString()}");
            var distance = 0;
            
            if (from == to) return 0;
            
            while (from != to && from.BaseType != null)
            {
                distance++;
                from = from.BaseType;
            }

            return distance;
        }
    }
}