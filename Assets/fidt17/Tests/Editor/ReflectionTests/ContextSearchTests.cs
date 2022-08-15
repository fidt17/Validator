using System;
using fidt17.UnityValidationModule.Editor.ContextParsers;
using fidt17.UnityValidationModule.Editor.Helpers;
using fidt17.UnityValidationModule.Editor.ValidationResultDrawers;
using fidt17.UnityValidationModule.Runtime.ValidationResults;
using NUnit.Framework;

namespace fidt17.Tests.Editor.ReflectionTests
{
    public class ContextSearchTests
    {
        private class TypeA { }

        private class TypeB : TypeA { }

        private class TypeC : TypeB { }

        private class TypeD : TypeC { }

        private class TypeF : TypeD { }
        
        private class TypeE : TypeD { }
        
        [ValidationResultParser(typeof(TypeA))]
        private class TypeAParser : BaseResultParser
        {
            public TypeAParser(ValidationResult vl) : base(vl)
            {
            }

            public override IValidationResultDrawer GetDrawer() => throw new NotImplementedException();
            public override bool IsContextValid(object obj) => true;
        }

        [ValidationResultParser(typeof(TypeB))]
        private class TypeBParser : BaseResultParser
        {
            public TypeBParser(ValidationResult vl) : base(vl)
            {
            }

            public override IValidationResultDrawer GetDrawer() => throw new NotImplementedException();
            public override bool IsContextValid(object obj) => true;
        }
        
        [ValidationResultParser(typeof(TypeC))]
        private class TypeCParser : BaseResultParser
        {
            public TypeCParser(ValidationResult vl) : base(vl)
            {
            }

            public override IValidationResultDrawer GetDrawer() => throw new NotImplementedException();
            public override bool IsContextValid(object obj) => true;
        }

        [ValidationResultParser(typeof(TypeF))]
        private class TypeFParserWithFailCondition : BaseResultParser
        {
            public TypeFParserWithFailCondition(ValidationResult vl) : base(vl)
            {
            }

            public override IValidationResultDrawer GetDrawer() => null;
            public override bool IsContextValid(object obj) => false;
        }
        
        [ValidationResultParser(typeof(TypeE))]
        private class TypeEParserWithPassCondition : BaseResultParser
        {
            public TypeEParserWithPassCondition(ValidationResult vl) : base(vl)
            {
            }

            public override IValidationResultDrawer GetDrawer() => null;
            public override bool IsContextValid(object obj) => true;
        }

        [Test]
        public void TestContextParserSearchOnNullValidationResult()
        {
            Assert.Throws<ArgumentNullException>(() => ReflectionExtensions.GetContextParserFor(null));
        }
        
        [Test]
        public void TestContextParserSearchOnEmptyContext()
        {
            Assert.That(() =>
            {
                var parser = ReflectionExtensions.GetContextParserFor(new PassResult(targetContext: null));
                return parser.GetType() == typeof(DefaultResultParser);
            });
        }

        [Test]
        public void TestContextParserTestA()
        {
            Assert.That(() =>
            {
                var parser = ReflectionExtensions.GetContextParserFor(new PassResult(targetContext: new TypeA()));
                return parser.GetType() == typeof(TypeAParser);
            });
        }
        
        [Test]
        public void TestContextParserTestBFromTypeA()
        {
            Assert.That(() =>
            {
                var parser = ReflectionExtensions.GetContextParserFor(new PassResult(targetContext: new TypeB()));
                return parser.GetType() == typeof(TypeBParser);
            });
        }
        
        [Test]
        public void TestContextParserTestCFromTypeB()
        {
            Assert.That(() =>
            {
                var parser = ReflectionExtensions.GetContextParserFor(new PassResult(targetContext: new TypeC()));
                return parser.GetType() == typeof(TypeCParser);
            });
        }
        
        [Test]
        public void TestContextParserTestDFromTypeC()
        {
            Assert.That(() =>
            {
                var parser = ReflectionExtensions.GetContextParserFor(new PassResult(targetContext: new TypeD()));
                return parser.GetType() == typeof(TypeCParser);
            });
        }

        [Test]
        public void TestContextParserTestFFailFromTypeD()
        {
            Assert.That(() =>
            {
                var parser = ReflectionExtensions.GetContextParserFor(new PassResult(targetContext: new TypeF()));
                return parser.GetType() == typeof(TypeCParser);
            });
        }
        
        [Test]
        public void TestContextParserTestFPassFromTypeD()
        {
            Assert.That(() =>
            {
                var parser = ReflectionExtensions.GetContextParserFor(new PassResult(targetContext: new TypeE()));
                return parser.GetType() == typeof(TypeEParserWithPassCondition);
            });
        }
    }
}