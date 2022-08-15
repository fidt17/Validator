using System;
using fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes;
using fidt17.UnityValidationModule.Runtime.Attributes.MethodAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace fidt17.Tests.Runtime
{
    public class UnityEventValidationTester : MonoBehaviour
    {
        [UnityEventValidation] public int A;
        [UnityEventValidation] public UnityEvent UnityEventA;

        public void MethodA()
        {
            
        }

        [ValidationMethod]
        private bool ThrowValidationMethod()
        {
            throw new Exception("ThrowValidationMethod Exception");
        }
    }
}