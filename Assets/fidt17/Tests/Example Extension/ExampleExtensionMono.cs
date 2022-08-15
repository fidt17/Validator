using UnityEngine;

namespace fidt17.Tests.Example_Extension
{
    public class ExampleExtensionMono : MonoBehaviour
    {
        [GreenColorValidation] public Color MyColor = Color.white;
    }
}
