using fidt17.UnityValidationModule.Runtime.Attributes.FieldAttributes;
using UnityEngine;

namespace fidt17.Tests.Runtime
{
	public class SingleFieldTest : MonoBehaviour
	{
		[SerializeField, NotNullValidation] private GameObject _reference;
	}
}