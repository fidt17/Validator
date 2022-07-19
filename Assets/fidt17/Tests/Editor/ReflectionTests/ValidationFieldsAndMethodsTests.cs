using System.Linq;
using fidt17.UnityValidationModule.Editor.Helpers;
using fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes;
using fidt17.UnityValidationModule.Runtime.Attributes.MethodAttributes;
using Microsoft.SqlServer.Server;
using NUnit.Framework;
using UnityEngine;

namespace fidt17.UnityValidationModule.Tests.Editor.ReflectionTests
{
    public class ValidationFieldsAndMethodsTests
    {
        private class TestClass
        {
            [FieldValidation] public Object FieldA;
            [NotNullValidation] public Object FieldB;
            [SerializeField] public Object FieldC;
            [FieldValidation] private Object FieldD;
            
            [ValidationMethod]
            public bool Method() => true;
            
            [SqlMethod]
            public void Method2() {}

            [ValidationMethod]
            private bool Method3() => false;
        }

        private class DerivativeFromTestClass : TestClass
        {
            [FieldValidation] private Object FieldE;

            [ValidationMethod]
            private bool Method4() => true;
        }

        private class DerivativeFromDerivativeFromTestClass : DerivativeFromTestClass
        {
            [FieldValidation] private Object FieldF;

            [ValidationMethod]
            private bool Method5() => true;
        }
        
        [Test]
        public void TestGetValidationFields()
        {
            Assert.That(typeof(TestClass).GetValidationFields().Count() == 3);
        }

        [Test]
        public void TestGetValidationMethods()
        {
            Assert.That(typeof(TestClass).GetValidationMethods().Count() == 2);
        }

        [Test]
        public void TestGetValidationFieldsOnDerivativeClass()
        {
            Assert.That(typeof(DerivativeFromTestClass).GetValidationFields().Count() == 4);
        }

        [Test]
        public void TestGetValidationMethodsOnDerivativeClass()
        {
            Assert.That(typeof(DerivativeFromTestClass).GetValidationMethods().Count() == 3);
        }
        
        [Test]
        public void TestGetValidationFieldsOnSecondDerivativeClass()
        {
            Assert.That(typeof(DerivativeFromDerivativeFromTestClass).GetValidationFields().Count() == 5);
        }

        [Test]
        public void TestGetValidationMethodsOnSecondDerivativeClass()
        {
            Assert.That(typeof(DerivativeFromDerivativeFromTestClass).GetValidationMethods().Count() == 4);
        }

    }
}