using System;
using System.IO;
using System.Text;
using UnityEditor;

namespace MixamoAssetPreprocessor
{
    public class MixamoAnimationRenamePostProcessor : AssetPostprocessor
    {
        void OnPreprocessAnimation()
        {
            if (!assetPath.StartsWith("Assets/Animation/Animations/Mixamo") || Path.GetExtension(assetPath) != ".fbx")
            {
                return;
            }

            ModelImporter modelImporter = assetImporter as ModelImporter;

            if (modelImporter == null)
            {
                return;
            }

            foreach (var clip in modelImporter.defaultClipAnimations)
            {
                if (clip.name == "mixamo.com")
                {
                    clip.name = SplitAtCaptialLetter(Path.GetFileNameWithoutExtension(assetPath));
                    modelImporter.clipAnimations = new[] { ConfigureMixamo(clip) };
                }
            }
        }

        private static ModelImporterClipAnimation ConfigureMixamo(ModelImporterClipAnimation clip)
        {
            clip.loopTime = true;
            clip.lockRootRotation = true;
            clip.lockRootHeightY = true;
            clip.lockRootPositionXZ = true;
            return clip;
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