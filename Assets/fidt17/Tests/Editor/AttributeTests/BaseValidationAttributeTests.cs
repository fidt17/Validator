using fidt17.Tests.Runtime;
using fidt17.UnityValidationModule.Runtime.Attributes;
using NUnit.Framework;
using UnityEngine;

namespace fidt17.Tests.Editor.AttributeTests
{
    public class BaseValidationAttributeTests
    {
        private class BaseValidationAttributeTestClass : BaseValidationAttribute
        {
            public BaseValidationAttributeTestClass(bool validateInPrefab) : base(validateInPrefab)
            {
                
            }
        }
        
        private ValidatorTester _tester => Object.FindObjectOfType<ValidatorTester>();

        [Test]
        public void TestShouldValidateOnNonPrefab()
        {
            var instance = new BaseValidationAttributeTestClass(false);
            Assert.That(instance.ShouldValidate(_tester.NonPrefabInstance) == true);
            
            instance = new BaseValidationAttributeTestClass(true);
            Assert.That(instance.ShouldValidate(_tester.NonPrefabInstance) == true);
        }

        [Test]
        public void TestShouldValidateOnPrefabInstance()
        {
            var instance = new BaseValidationAttributeTestClass(false);
            Assert.That(instance.ShouldValidate(_tester.PrefabInstance) == true);
            
            instance = new BaseValidationAttributeTestClass(true);
            Assert.That(instance.ShouldValidate(_tester.PrefabInstance) == true);
        }

        [Test]
        public void TestShouldValidateOnPrefabAsset()
        {
            var instance = new BaseValidationAttributeTestClass(false);
            Assert.That(instance.ShouldValidate(_tester.PrefabAsset) == false);
            
            instance = new BaseValidationAttributeTestClass(true);
            Assert.That(instance.ShouldValidate(_tester.PrefabAsset) == true);
        }
    }
}