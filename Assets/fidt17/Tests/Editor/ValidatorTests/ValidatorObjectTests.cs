using System.Collections.Generic;
using System.Linq;
using fidt17.UnityValidationModule.Editor;
using fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes;
using fidt17.UnityValidationModule.Runtime.Attributes.MethodAttributes;
using NUnit.Framework;
using Object = UnityEngine.Object;

namespace fidt17.Tests.Editor.ValidatorTests
{
    public class ValidatorObjectTests
    {
        #region Test Classes

        private class TestNonValidatableClass
        {
            
        }

        private class TestValidatableClass
        {
            [FieldValidation] public Object PublicField;
            [FieldValidation] private Object PrivateField;
            
            [ValidationMethod]
            public bool PublicMethod() => true;

            [ValidationMethod]
            public bool PrivateMethod() => true;
        }

        private class TestRootClass
        {
            [FieldValidation] public TestLeafClass LeafChild;
            [FieldValidation(recursiveValidation: false)] public TestLeafClass LeafChildNoNesting;
            [FieldValidation] public TestLoopChildClass NullChild;
            [FieldValidation] public TestLoopChildClass NotNullLoopChild;
            [FieldValidation] public List<TestLeafClass> ListField;
        }

        private class TestLoopChildClass
        {
            [FieldValidation] public Object Field;
            [FieldValidation] public TestRootClass RootClass;
        }
        
        private class TestLeafClass
        {
            [FieldValidation] public System.Object Field;
        }
        
        #endregion
        
        [Test]
        public void TestValidateNonValidatableObject()
        {
            var instance = new TestNonValidatableClass();
            Assert.That(new Validator().Validate(instance).Count() == 0);
        }

        [Test]
        public void TestNestedValidationWithoutNestedCollection()
        {
            var rootClass = new TestRootClass();
            rootClass.LeafChild = new TestLeafClass();
            rootClass.LeafChildNoNesting = new TestLeafClass();
            rootClass.NotNullLoopChild = new TestLoopChildClass();
            rootClass.NotNullLoopChild.RootClass = rootClass;

            Assert.That(new Validator().Validate(rootClass).Count() == 8);
        }

        [Test]
        public void TestNestedValidationWithNestedCollection()
        {
            var rootClass = new TestRootClass();
            rootClass.ListField = new List<TestLeafClass>();
            rootClass.ListField.Add(new TestLeafClass());
            rootClass.ListField.Add(null);
            rootClass.ListField.Add(rootClass.ListField[0]);
            rootClass.ListField.Add(new TestLeafClass()
            {
                Field = new TestLoopChildClass()
            });

            Assert.That(new Validator().Validate(rootClass).Count() == 9);
        }
    }
}
