using System;
using fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes;
using fidt17.UnityValidationModule.Runtime.Attributes.MethodAttributes;
using UnityEngine;

namespace fidt17.UnityValidationModule.Tests.Runtime
{
    //[CreateAssetMenu(menuName = "TestScriptableObject")]
    public class TestScriptableObject : TestScriptableObjectParent
    {
        [NotNullValidation] public GameObject FieldA;
        [NotNullValidation] private GameObject FieldB;
        [SerializeField] [NotNullValidation] private GameObject FieldC;
        [NotNullValidation] public ClassFromSo CFS;
        [NotNullValidation] public ClassFromSo CFS1;
        [NotNullValidation] public ClassFromSo CFS2;
        [NotNullValidation] public ClassFromSo CFS3;
        [NotNullValidation] public ClassFromSo CFS4;
        [NotNullValidation] public ClassFromSo CFS5;
        [NotNullValidation] public ClassFromSo CFS511;
        [NotNullValidation] public ClassFromSo CFS6;
        [NotNullValidation] public ClassFromSo CFS7;
        [NotNullValidation] public ClassFromSo CFS8;
        [NotNullValidation] public ClassFromSo CFS9;

        
        [Serializable]
        public class ClassFromSo
        {
            
        }

        [ValidationMethod]
        public bool MMethod()
        {
            return true;
        }

        [ValidationMethod("MMethodThatFails failed")]
        public bool MMethodThatFails() => false;
    }

    public class TestScriptableObjectParent : ScriptableObject
    {
        [NotNullValidation] public GameObject FieldD;
    }
}