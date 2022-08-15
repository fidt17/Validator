using UnityEngine;

namespace fidt17.UnityValidationModule.Editor.Helpers
{
    public static class ValidatorEditorUtils
    {
        /// <summary>
        /// Generates a GUIStyle with background color of provided value.
        /// </summary>
        public static GUIStyle GetBackgroundStyle(Color color)
        {
            var guiStyle = new GUIStyle
            {
                normal =
                {
                    background = GetMonoColorTexture(color)
                }
            };
            return guiStyle;
        }
        
        /// <summary>
        /// Converts provided color to Texture2D with size of (1, 1)
        /// </summary>
        public static Texture2D GetMonoColorTexture(Color color, TextureFormat textureFormat = TextureFormat.RGBA32)
        {
            var monoColorTexture = new Texture2D(1, 1, textureFormat, false);
            monoColorTexture.SetPixel(0, 0, color);
            monoColorTexture.Apply();
            return monoColorTexture;
        }

        /// <summary>
        /// Splits provided string by '\n' in such way, that it's width would not go over certain maxWidth.
        /// Splits string by words rather that by characters.
        /// </summary>
        public static string SplitStringInLinesToFit(string str, int fontSize, float maxWidth)
        {
            if (str == null) return "";
            var maxCharactersPerLine = (int) (maxWidth / fontSize * 2f);
            var result = "";

            var charCount = 0;
            for (var i = 0; i < str.Length; i++)
            {
                result += str[i];
                if (str[i] != '\n')
                {
                    charCount++;
                }
                else
                {
                    charCount = 0;
                }
                
                if (charCount == maxCharactersPerLine)
                {
                    result += "\n";
                    charCount = 0;
                }
            }

            return result;
        }
    }
}