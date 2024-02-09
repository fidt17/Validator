using System.Collections.Generic;

namespace fidt17.UnityValidationModule.Runtime
{
    public static class ValidationSession
    {
        public static readonly Dictionary<string, object> Data = new Dictionary<string, object>();

        public static void Reset()
        {
            Data.Clear();
        }
    }
}