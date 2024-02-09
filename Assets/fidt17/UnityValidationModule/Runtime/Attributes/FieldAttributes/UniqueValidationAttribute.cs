using System;
using System.Collections.Generic;
using System.Reflection;
using fidt17.UnityValidationModule.Runtime.ValidationResults;

namespace fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UniqueValidationAttribute : FieldValidationAttribute
    {
        private readonly string _groupName;

        public UniqueValidationAttribute(string groupName)
        {
            _groupName = groupName;
        }
        
        public override ValidationResult ValidateField(FieldInfo field, object target)
        {
            const string dataKey = "IdValidationTypeToIdMap";
            if (!ValidationSession.Data.ContainsKey(dataKey))
            {
                ValidationSession.Data.Add(dataKey, new Dictionary<string, HashSet<object>>());
            }
            
            var typeToIdMap = (Dictionary<string, HashSet<object>>) ValidationSession.Data[dataKey];
            
            if (!typeToIdMap.ContainsKey(_groupName))
            {
                typeToIdMap.Add(_groupName, new HashSet<object>());
            }

            var value = field.GetValue(target);

            if (value == null || value is string str && string.IsNullOrWhiteSpace(str))
            {
                return new FailResult($"ID is null or empty. Group {_groupName}.\n{target.GetType().Name} {field.Name}.", target);
            }

            if (typeToIdMap[_groupName].Contains(value))
            {
                return new FailResult($"ID {value} is already taken in group {_groupName}.\n{target.GetType().Name} {field.Name}.", target);
            }

            typeToIdMap[_groupName].Add(value);
            
            return base.ValidateField(field, target);
        }
    }
}