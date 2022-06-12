using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MixamoAssetPreprocessor
{
    public class SyntyMixamoRigger : AssetPostprocessor
    {
        private static readonly HumanBone[] DefaultBones = {
            new() { boneName = "Hips", limit = new HumanLimit(), humanName = "Hips" },
            new() { boneName = "UpperLeg_L", limit = new HumanLimit(), humanName = "LeftUpperLeg" },
            new() { boneName = "UpperLeg_R", limit = new HumanLimit(), humanName = "RightUpperLeg" },
            new() { boneName = "LowerLeg_L", limit = new HumanLimit(), humanName = "LeftLowerLeg" },
            new() { boneName = "LowerLeg_R", limit = new HumanLimit(), humanName = "RightLowerLeg" },
            new() { boneName = "Ankle_L", limit = new HumanLimit(), humanName = "LeftFoot" },
            new() { boneName = "Ankle_R", limit = new HumanLimit(), humanName = "RightFoot" },
            new() { boneName = "Spine_01", limit = new HumanLimit(), humanName = "Spine" },
            new() { boneName = "Spine_02", limit = new HumanLimit(), humanName = "Chest" },
            new() { boneName = "Neck", limit = new HumanLimit(), humanName = "Neck" },
            new() { boneName = "Head", limit = new HumanLimit(), humanName = "Head" },
            new() { boneName = "Clavicle_L", limit = new HumanLimit(), humanName = "LeftShoulder" },
            new() { boneName = "Clavicle_R", limit = new HumanLimit(), humanName = "RightShoulder" },
            new() { boneName = "Shoulder_L", limit = new HumanLimit(), humanName = "LeftUpperArm" },
            new() { boneName = "Shoulder_R", limit = new HumanLimit(), humanName = "RightUpperArm" },
            new() { boneName = "Elbow_L", limit = new HumanLimit(), humanName = "LeftLowerArm" },
            new() { boneName = "Elbow_R", limit = new HumanLimit(), humanName = "RightLowerArm" },
            new() { boneName = "Hand_L", limit = new HumanLimit(), humanName = "LeftHand" },
            new() { boneName = "Hand_R", limit = new HumanLimit(), humanName = "RightHand" },
            new() { boneName = "Toes_L", limit = new HumanLimit(), humanName = "LeftToes" },
            new() { boneName = "Toes_R", limit = new HumanLimit(), humanName = "RightToes" },
            new() { boneName = "Eyes", limit = new HumanLimit(), humanName = "LeftEye" },
            new() { boneName = "IndexFinger_01", limit = new HumanLimit(), humanName = "Left Index Proximal" },
            new() { boneName = "IndexFinger_02", limit = new HumanLimit(), humanName = "Left Index Intermediate" },
            new() { boneName = "IndexFinger_03", limit = new HumanLimit(), humanName = "Left Index Distal" },
            new() { boneName = "Finger_01", limit = new HumanLimit(), humanName = "Left Middle Proximal" },
            new() { boneName = "Finger_02", limit = new HumanLimit(), humanName = "Left Middle Intermediate" },
            new() { boneName = "Finger_03", limit = new HumanLimit(), humanName = "Left Middle Distal" },
            new() { boneName = "Thumb_011", limit = new HumanLimit(), humanName = "Right Thumb Proximal" },
            new() { boneName = "Thumb_021", limit = new HumanLimit(), humanName = "Right Thumb Intermediate" },
            new() { boneName = "Thumb_031", limit = new HumanLimit(), humanName = "Right Thumb Distal" },
            new() { boneName = "IndexFinger_011", limit = new HumanLimit(), humanName = "Right Index Proximal" },
            new() { boneName = "IndexFinger_021", limit = new HumanLimit(), humanName = "Right Index Intermediate" },
            new() { boneName = "IndexFinger_031", limit = new HumanLimit(), humanName = "Right Index Distal" },
            new() { boneName = "Spine_03", limit = new HumanLimit(), humanName = "UpperChest" },
            new() { boneName = "Thumb_01", limit = new HumanLimit(), humanName = "Left Thumb Proximal" },
            new() { boneName = "Thumb_02", limit = new HumanLimit(), humanName = "Left Thumb Intermediate" },
            new() { boneName = "Thumb_03", limit = new HumanLimit(), humanName = "Left Thumb Distal" },
            new() { boneName = "Finger_011", limit = new HumanLimit(), humanName = "Right Middle Proximal" },
            new() { boneName = "Finger_021", limit = new HumanLimit(), humanName = "Right Middle Intermediate" },
            new() { boneName = "Finger_031", limit = new HumanLimit(), humanName = "Right Middle Distal" }
        };
        public void OnPreprocessModel()
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

            modelImporter.animationType = ModelImporterAnimationType.Human;
            modelImporter.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
        }

        public void OnPostprocessModel(GameObject g)
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

            var humanDescription = modelImporter.humanDescription;
            humanDescription.human = DefaultBones;
            modelImporter.humanDescription = humanDescription;
        }
    }
}