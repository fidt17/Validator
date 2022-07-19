using System;
using System.Collections.Generic;
using System.Reflection;
using fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes;
using NUnit.Framework;

namespace fidt17.UnityValidationModule.Tests.Editor.AttributeTests
{
    public class NotNullCollectionAttributeTests
    {
        private class TestClass
        {
            [NotNullCollection] public List<Object> NullField;
            [NotNullCollection] public Object IncorrectField;
            [NotNullCollection] public Dictionary<int, int> EmptyCollection = new Dictionary<int, int>();
            [NotNullCollection] public List<string> ListCollection = new List<string>();
        }
        
        [Test]
        public void TestNotNullCollectionOnNullField()
        {
            var instance = new TestClass();
            var f = instance.GetType().GetField(nameof(TestClass.NullField));
            Assert.That(() =>
            {
                var attribute = (NotNullValidationAttribute) f.GetCustomAttribute(typeof(NotNullValidationAttribute));
                return attribute.ValidateField(f, instance).Result == false;
            });
        }
        
        [Test]
        public void TestNotNullCollectionOnIncorrectField()
        {
            var instance = new TestClass();
            var f = instance.GetType().GetField(nameof(TestClass.IncorrectField));
            Assert.Throws<Exception>(() =>
            {
                var attribute = (NotNullValidationAttribute) f.GetCustomAttribute(typeof(NotNullValidationAttribute));
                attribute.ValidateField(f, instance);
            });
        }

        [Test]
        public void TestNotNullCollectionOnEmptyCollection()
        {
            var instance = new TestClass();
            var f = instance.GetType().GetField(nameof(TestClass.EmptyCollection));
            Assert.That(() =>
            {
                var attribute = (NotNullValidationAttribute)f.GetCustomAttribute(typeof(NotNullCollectionAttribute));
                return attribute.ValidateField(f, instance).Result == true;
            });
        }

        [Test]
        public void TestCollectionWithoutNullElements()
        {
            var instance = new TestClass();
            instance.ListCollection = new List<string>
            {
                "AAA",
                "BBB",
                "",
                "NULL",
                "null"
            };
            var f = instance.GetType().GetField(nameof(TestClass.ListCollection));
            Assert.That(() =>
            {
                var attribute = (NotNullValidationAttribute)f.GetCustomAttribute(typeof(NotNullCollectionAttribute));
                return attribute.ValidateField(f, instance).Result == true;
            });
        }
        
        [Test]
        public void TestCollectionWithNullElements()
        {
            
            var instance = new TestClass();
            instance.ListCollection = new List<string>
            {
                "AAA",
                null,
                "",
                "NULL",
                "null"
            };
            var f = instance.GetType().GetField(nameof(TestClass.ListCollection));
            Assert.That(() =>
            {
                var attribute = (NotNullValidationAttribute)f.GetCustomAttribute(typeof(NotNullCollectionAttribute));
                return attribute.ValidateField(f, instance).Result == false;
            });
        }
    }
}