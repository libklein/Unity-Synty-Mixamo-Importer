using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MixamoAssetPreprocessor
{
    public class MixamoAnimationRenamePostProcessor : AssetPostprocessor
    {
        void OnPreprocessModel()
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


            var animations = modelImporter.defaultClipAnimations;
            
            if (modelImporter.clipAnimations.Length > 0)
            {
                animations = modelImporter.clipAnimations;
            }
            
            foreach (var clip in animations)
            {
                if (clip.name == "mixamo.com")
                {
                    clip.name = SplitAtCaptialLetter(Path.GetFileNameWithoutExtension(assetPath));
                    modelImporter.clipAnimations = new[] { clip };
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