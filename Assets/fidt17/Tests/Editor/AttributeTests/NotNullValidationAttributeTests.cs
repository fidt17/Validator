using System.Reflection;
using fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes;
using NUnit.Framework;
using Object = System.Object;

namespace fidt17.UnityValidationModule.Tests.Editor.AttributeTests
{
    public class NotNullValidationAttributeTests
    {
        private class TestClass
        {
            [NotNullValidation] public Object PublicFailField;
            [NotNullValidation] public Object PublicPassField = new object();
        }
        
        [Test]
        public void TestPublicFailField()
        {
            var instance = new TestClass();
            var f = instance.GetType().GetField(nameof(TestClass.PublicFailField));
            Assert.That(() =>
            {
                var attribute = (NotNullValidationAttribute) f.GetCustomAttribute(typeof(NotNullValidationAttribute));
                var result = attribute.ValidateField(f, instance);
                return result.Result == false;
            });
        }

        [Test]
        public void TestPublicPassField()
        {
            var instance = new TestClass();
            var f = instance.GetType().GetField(nameof(TestClass.PublicPassField));
            Assert.That(() =>
            {
                var attribute = (NotNullValidationAttribute) f.GetCustomAttribute(typeof(NotNullValidationAttribute));
                var result = attribute.ValidateField(f, instance);
                return result.Result == true;
            });
        }
    }
}