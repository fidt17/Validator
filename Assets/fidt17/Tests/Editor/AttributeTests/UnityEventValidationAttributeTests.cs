using System.Reflection;
using fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes;
using NUnit.Framework;
using UnityEngine.Events;

namespace fidt17.Tests.Editor.AttributeTests
{
    public class UnityEventValidationAttributeTests
    {
        private class TestClass
        {
            [UnityEventValidation] public UnityEvent NullUnityEvent;
            [UnityEventValidation] public UnityEvent NotNullUnityEvent = new UnityEvent();
        }

        [Test]
        public void TestUnityEventValidationOnNullField()
        {
            var instance = new TestClass();
            var f = instance.GetType().GetField(nameof(TestClass.NullUnityEvent));
            Assert.That(() =>
            {
                var attribute = (UnityEventValidationAttribute)f.GetCustomAttribute(typeof(UnityEventValidationAttribute));
                return attribute.ValidateField(f, instance).Result == false;
            });
        }
        
        [Test]
        public void TestUnityEventValidationOnNotNullField()
        {
            var instance = new TestClass();
            var f = instance.GetType().GetField(nameof(TestClass.NotNullUnityEvent));
            Assert.That(() =>
            {
                var attribute = (UnityEventValidationAttribute)f.GetCustomAttribute(typeof(UnityEventValidationAttribute));
                return attribute.ValidateField(f, instance).Result == true;
            });
        }

        [Test]
        public void TestUnityEventValidationWithCorrectSubscribers()
        {
            void foo()
            {
                
            }
            
            var instance = new TestClass();
            instance.NotNullUnityEvent.AddListener(foo);
            var f = instance.GetType().GetField(nameof(TestClass.NotNullUnityEvent));
            Assert.That(() =>
            {
                var attribute = (UnityEventValidationAttribute)f.GetCustomAttribute(typeof(UnityEventValidationAttribute));
                return attribute.ValidateField(f, instance).Result == true;
            });
        }
    }
}