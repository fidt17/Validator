using System.Linq;
using fidt17.UnityValidationModule.Editor;
using fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes;
using NUnit.Framework;
using UnityEngine;

namespace fidt17.Tests.Editor.ValidatorTests
{
    public class ValidatorGameObjectTests
    {
        private class TestMonoValidatable : MonoBehaviour
        {
            [FieldValidation] public Object Field;
        }
        
        [Test]
        public void TestValidationOnNonValidatableGameObject()
        {
            var obj = new GameObject();
            
            Assert.That(() => !Validator.Validate(obj).Any());

            Object.DestroyImmediate(obj);
        }
        
        [Test]
        public void TestValidateGameObjectWithSingleComponent()
        {
            var obj = new GameObject();
            obj.AddComponent<TestMonoValidatable>();

            Assert.That(Validator.Validate(obj).Count() == 1);

            Object.DestroyImmediate(obj);
        }

        [Test]
        public void TestValidateDisabledNestedGameObject()
        {
            var obj = new GameObject();
            var childEnabled = new GameObject();
            childEnabled.transform.SetParent(obj.transform);
            childEnabled.AddComponent<TestMonoValidatable>();
            
            var childDisabled = new GameObject();
            childDisabled.transform.SetParent(obj.transform);
            childDisabled.AddComponent<TestMonoValidatable>();
            childDisabled.gameObject.SetActive(false);
            
            Assert.That(Validator.Validate(obj).Count() == 2);
            
            Object.DestroyImmediate(obj);
            Object.DestroyImmediate(childEnabled);
            Object.DestroyImmediate(childDisabled);
        }
        
        [Test]
        public void TestValidateGameObjectWithMultipleComponents()
        {
            var obj = new GameObject();
            obj.AddComponent<TestMonoValidatable>();
            obj.AddComponent<TestMonoValidatable>();
            
            Assert.That(Validator.Validate(obj).Count() == 2);

            Object.DestroyImmediate(obj);
        }
        
        [Test]
        public void TestValidateOnNonValidatableGameObjectWithValidatableChildren()
        {
            var rootObj = new GameObject();
            var child = new GameObject();
            child.transform.SetParent(rootObj.transform);
            child.AddComponent<TestMonoValidatable>();
            
            Assert.That(Validator.Validate(rootObj).Count() == 1);
            
            Object.DestroyImmediate(rootObj);
        }
        
        [Test]
        public void TestValidateOnValidatableGameObjectWithValidatableChildren()
        {
            var rootObj = new GameObject();
            rootObj.AddComponent<TestMonoValidatable>();
            var child = new GameObject();
            child.transform.SetParent(rootObj.transform);
            child.AddComponent<TestMonoValidatable>();
            
            Assert.That(Validator.Validate(rootObj).Count() == 2);
            
            Object.DestroyImmediate(rootObj);
        }
    }
}