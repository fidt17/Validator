﻿using System;
using System.Reflection;
using fidt17.UnityValidationModule.Runtime.Attributes;
using fidt17.UnityValidationModule.Runtime.Attributes.MethodAttributes;
using NUnit.Framework;

namespace fidt17.UnityValidationModule.Tests.Editor.AttributeTests
{
    public class ValidationMethodAttributeTests
    {
        private class TestClass
        {
            [ValidationMethod]
            public bool PassValidationMethod() => true;
            
            [ValidationMethod]
            public bool FailValidationMethod() => false;

            [ValidationMethod]
            public void WrongReturnTypeValidationMethod()
            {
            }

            [ValidationMethod]
            public bool WrongArgumentsValidationMethod(string[] args) => true;
        }
        
        [Test]
        public void TestPassValidationMethod()
        {
            var instance = new TestClass();
            var m = instance.GetType().GetMethod(nameof(TestClass.PassValidationMethod));
            Assert.That(() =>
            {
                var attribute = (ValidationMethodAttribute) m.GetCustomAttribute(typeof(ValidationMethodAttribute));
                var result = attribute.ValidateMethod(m, instance);
                return result.Result == true;
            });
        }
        
        [Test]
        public void TestFailValidationMethod()
        {
            var instance = new TestClass();
            var m = instance.GetType().GetMethod(nameof(TestClass.FailValidationMethod));
            Assert.That(() =>
            {
                var attribute = (ValidationMethodAttribute) m.GetCustomAttribute(typeof(ValidationMethodAttribute));
                var result = attribute.ValidateMethod(m, instance);
                return result.Result == false;
            });
        }
        
        [Test]
        public void TestWrongReturnTypeValidationMethod()
        {
            var instance = new TestClass();
            var m = instance.GetType().GetMethod(nameof(TestClass.WrongReturnTypeValidationMethod));
            Assert.Throws<Exception>(() =>
            {
                var attribute = (ValidationMethodAttribute) m.GetCustomAttribute(typeof(ValidationMethodAttribute));
                attribute.ValidateMethod(m, instance);
            });
        }
        
        [Test]
        public void TestWrongArgumentsValidationMethod()
        {
            var instance = new TestClass();
            var m = instance.GetType().GetMethod(nameof(TestClass.WrongArgumentsValidationMethod));
            Assert.Throws<Exception>(() =>
            {
                var attribute = (ValidationMethodAttribute) m.GetCustomAttribute(typeof(ValidationMethodAttribute));
                attribute.ValidateMethod(m, instance);
            });
        }
    }
}