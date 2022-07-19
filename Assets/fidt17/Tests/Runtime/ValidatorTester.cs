using System;
using System.Collections.Generic;
using fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes;
using fidt17.UnityValidationModule.Runtime.Attributes.MethodAttributes;
using UnityEngine;

namespace fidt17.UnityValidationModule.Tests.Runtime
{
    public class ValidatorTester : MonoBehaviour
    {
        public GameObject NonPrefabInstance => _nonPrefabInstance;
        public GameObject PrefabInstance => _prefabInstance;
        public GameObject PrefabAsset => _prefabAsset;

        [SerializeField] private GameObject _nonPrefabInstance;
        [SerializeField] private GameObject _prefabInstance;
        [SerializeField] private GameObject _prefabAsset;

        [NotNullValidation] public GameObject GameObject;
        [NotNullValidation] public List<TestNestedClass> TestList;

        [NotNullValidation] public TestScriptableObject _TestScriptableObject;
        
        [Serializable]
        public class TestNestedClass
        {
            [NotNullValidation] public GameObject RequiredGameObject;
        }

        [NotNullValidation] public TestStruct TestStructA;
        
        [Serializable]
        public struct TestStruct
        {
            [NotNullValidation] public GameObject GameObject;
        }

        [ValidationMethod]
        private bool M()
        {
            return false;
        }
    }
}
