using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MixamoAssetPreprocessor
{
    public class MixamoAnimationRenamePostProcessor : AssetPostprocessor
    {
        void OnPreprocessAnimation()
        {
            if (!assetPath.StartsWith("Assets/Animation/Animations/Synty/Mixamo") || Path.GetExtension(assetPath) != ".fbx")
            {
                return;
            }

            ModelImporter modelImporter = assetImporter as ModelImporter;

            if (modelImporter == null)
            {
                return;
            }

            // Don't overwrite already imported models/animations
            if (modelImporter.clipAnimations.Length > 0)
            {
                return;
            }

            foreach (var clip in modelImporter.defaultClipAnimations)
            {
                if (clip.name == "mixamo.com")
                {
                    clip.name = SplitAtCaptialLetter(Path.GetFileNameWithoutExtension(assetPath));
                    modelImporter.clipAnimations = new[] { clip };
                    break;
                }
            }
        }

        private static String SplitAtCaptialLetter(string input)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in input)
            {
                if (Char.IsUpper(c) && builder.Length > 0)
                {
                    builder.Append(' ');
                }

                builder.Append(c);
            }

            return builder.ToString();
        }
    }
}