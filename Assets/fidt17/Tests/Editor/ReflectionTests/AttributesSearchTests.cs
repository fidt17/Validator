using System.Linq;
using fidt17.UnityValidationModule.Editor.Helpers;
using fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes;
using fidt17.UnityValidationModule.Runtime.Attributes.MethodAttributes;
using Microsoft.SqlServer.Server;
using NUnit.Framework;
using UnityEngine;
using Object = System.Object;

namespace fidt17.Tests.Editor.ReflectionTests
{
    public class AttributesSearchTests
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
        
        [Test]
        public void TestGetAttributesOfTypeOnField()
        {
            var field = typeof(TestClass).GetField(nameof(TestClass.FieldA));
            Assert.That(field.GetAttributesOfType<FieldValidationAttribute>().Count() == 1);
        }

        [Test]
        public void TestGetAttributesOfTypeOfMethod()
        {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.Method));
            Assert.That(method.GetAttributesOfType<ValidationMethodAttribute>().Count() == 1);
        }

        [Test]
        public void TestGetDerivativeAttributesOfType()
        {
            var field = typeof(TestClass).GetField(nameof(TestClass.FieldB));
            Assert.That(field.GetAttributesOfType<FieldValidationAttribute>().Count() == 1);
        }
    }
}