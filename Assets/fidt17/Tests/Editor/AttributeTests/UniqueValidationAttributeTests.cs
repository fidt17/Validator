using System.Collections.Generic;
using System.Linq;
using fidt17.UnityValidationModule.Editor;
using fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes;
using NUnit.Framework;

namespace fidt17.Tests.Editor.AttributeTests
{
    public class UniqueValidationAttributeTests
    {
        private class TestCollection
        {
            [FieldValidation] public List<TestClassA> A = new List<TestClassA>();
            [FieldValidation] public List<TestClassB> B = new List<TestClassB>();
        }
        
        private class TestClassA
        {
            [UniqueValidation("groupA", notAssignedIdValue: "NONE")] public string Id;

            public TestClassA(string id)
            {
                Id = id;
            }
        }
        
        private class TestClassB
        {
            [UniqueValidation("groupB")] public string Id;

            public TestClassB(string id)
            {
                Id = id;
            }
        }

        [Test]
        public void EmptyId()
        {
            var instance = new TestClassA("");
            Assert.That(() =>
            {
                var validator = new Validator();
                var failCount = validator.Validate(instance).Count(x => !x.Result);
                return failCount == 1;
            });
        }
        
        [Test]
        public void NullId()
        {
            var instance = new TestClassA(null);
            Assert.That(() =>
            {
                var validator = new Validator();
                var failCount = validator.Validate(instance).Count(x => !x.Result);
                return failCount == 1;
            });
        }
        
        [Test]
        public void SingleId()
        {
            var instance = new TestClassA("1");
            Assert.That(() =>
            {
                var validator = new Validator();
                var failCount = validator.Validate(instance).Count(x => !x.Result);
                return failCount == 0;
            });
        }
        
        [Test]
        public void MultipleIdsPass()
        {
            var instance = new TestCollection
            {
                A = new List<TestClassA>
                {
                    new TestClassA("1"),
                    new TestClassA("2"),
                }
            };
            
            Assert.That(() =>
            {
                var validator = new Validator();
                var failCount = validator.Validate(instance).Count(x => !x.Result);
                return failCount == 0;
            });
        }
        
        [Test]
        public void MultipleIdsFail()
        {
            var instance = new TestCollection
            {
                A = new List<TestClassA>
                {
                    new TestClassA("1"),
                    new TestClassA("2"),
                    new TestClassA("2"),
                }
            };
            
            Assert.That(() =>
            {
                var validator = new Validator();
                var failCount = validator.Validate(instance).Count(x => !x.Result);
                return failCount == 1;
            });
        }
        
        [Test]
        public void MultipleIdGroupsPass()
        {
            var instance = new TestCollection
            {
                A = new List<TestClassA>
                {
                    new TestClassA("1"),
                    new TestClassA("2"),
                },
                B = new List<TestClassB>
                {
                    new TestClassB("1"),
                    new TestClassB("2"),
                }
            };
            
            Assert.That(() =>
            {
                var validator = new Validator();
                var failCount = validator.Validate(instance).Count(x => !x.Result);
                return failCount == 0;
            });
        }
        
        [Test]
        public void MultipleIdGroupsFail()
        {
            var instance = new TestCollection
            {
                A = new List<TestClassA>
                {
                    new TestClassA("1"),
                    new TestClassA("2"),
                    new TestClassA("2"),
                },
                B = new List<TestClassB>
                {
                    new TestClassB("1"),
                    new TestClassB("2"),
                    new TestClassB("2"),
                }
            };
            
            Assert.That(() =>
            {
                var validator = new Validator();
                var failCount = validator.Validate(instance).Count(x => !x.Result);
                return failCount == 2;
            });
        }

        [Test]
        public void TestNotAssignedValue()
        {
            var instance = new TestCollection
            {
                A = new List<TestClassA>
                {
                    new TestClassA("NONE"),
                    new TestClassA("NONE"),
                }
            };
            Assert.That(() =>
            {
                var validator = new Validator();
                return validator.Validate(instance).Count(x => !x.Result) == 0;
            });
        }
    }
}