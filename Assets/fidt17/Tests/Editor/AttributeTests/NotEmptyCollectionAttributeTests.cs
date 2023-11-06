using System;
using System.Collections.Generic;
using System.Reflection;
using fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes.CollectionFieldAttributes;
using NUnit.Framework;

namespace fidt17.Tests.Editor.AttributeTests
{
    public class NotEmptyCollectionAttributeTests
    {
        private class TestClass
        {
            [NotEmptyCollection] public List<Object> NullField;
            [NotEmptyCollection] public Object IncorrectField;
            [NotEmptyCollection(allowNullElements: true)] public List<string> ListCollection = new List<string>();
            [NotEmptyCollection(allowNullElements: false)] public List<string> ListCollectionNoNullElements = new List<string>();
        }
        
        [Test]
        public void TestNotEmptyCollectionOnNullField()
        {
            var instance = new TestClass();
            var f = instance.GetType().GetField(nameof(TestClass.NullField));
            Assert.That(() =>
            {
                var attribute = (NotEmptyCollectionAttribute) f.GetCustomAttribute(typeof(NotEmptyCollectionAttribute));
                return attribute.ValidateField(f, instance).Result == false;
            });
        }
        
        [Test]
        public void TestNotEmptyCollectionOnEmptyCollection()
        {
            var instance = new TestClass();
            var f = instance.GetType().GetField(nameof(TestClass.ListCollection));
            Assert.That(() =>
            {
                var attribute = (NotEmptyCollectionAttribute) f.GetCustomAttribute(typeof(NotEmptyCollectionAttribute));
                return attribute.ValidateField(f, instance).Result == false;
            });
        }
        
        [Test]
        public void TestNotNullCollectionOnIncorrectField()
        {
            var instance = new TestClass();
            var f = instance.GetType().GetField(nameof(TestClass.IncorrectField));
            Assert.That(() =>
            {
                var attribute = (NotEmptyCollectionAttribute) f.GetCustomAttribute(typeof(NotEmptyCollectionAttribute));
                return attribute.ValidateField(f, instance).Result == false;
            });
        }
        
        [Test]
        public void TestNotNullCollectionOnNonEmptyCollection()
        {
            var instance = new TestClass();
            instance.ListCollection.Add("42");
            var f = instance.GetType().GetField(nameof(TestClass.ListCollection));
            Assert.That(() =>
            {
                var attribute = (NotEmptyCollectionAttribute) f.GetCustomAttribute(typeof(NotEmptyCollectionAttribute));
                return attribute.ValidateField(f, instance).Result == true;
            });
        }
        
        [Test]
        public void TestNotNullCollectionOnNonEmptyCollectionNoNullElements()
        {
            var instance = new TestClass();
            instance.ListCollection.Add("42");
            instance.ListCollection.Add(null);
            
            instance.ListCollectionNoNullElements.Add("42");
            instance.ListCollectionNoNullElements.Add(null);

            var f = instance.GetType().GetField(nameof(TestClass.ListCollection));
            Assert.That(() =>
            {
                var attribute = (NotEmptyCollectionAttribute) f.GetCustomAttribute(typeof(NotEmptyCollectionAttribute));
                return attribute.ValidateField(f, instance).Result == true;
            });
            
            f = instance.GetType().GetField(nameof(TestClass.ListCollectionNoNullElements));
            Assert.That(() =>
            {
                var attribute = (NotEmptyCollectionAttribute) f.GetCustomAttribute(typeof(NotEmptyCollectionAttribute));
                return attribute.ValidateField(f, instance).Result == false;
            });
        }
    }
}