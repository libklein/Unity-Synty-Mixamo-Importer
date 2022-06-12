
# Unity Synty Mixamo Animation Importer

A editor script to automatically apply the recommended settings from Synty Studios (see https://www.youtube.com/watch?v=9H0aJhKSlEQ) to ensure that Animations imported from Mixamo work with Synty characters. It additionally (optionally) renames the animation clip according to the name of the imported asset to avoid multiple animations with the name `mixamo.com`. 

`SyntyMixamoRigger.cs` simply configures any `.fbx` added to `Assets/Animation/Animations/Synty/Mixamo/` with the recommend values. This includes character rigging. 
Then, `SyntyMixamoAnimationRenamer.cs` renames the animation clip `mixamo.com` according to the filename of the asset and removes any other clips. If you do not want this to happen, just remove `SyntyMixamoAnimationRenamer.cs`.
## Getting Started

Simply download the repository and place it's contents right into your unity project directory.
## Usage/Examples

Download an animation from Mixamo with the settings/character described in the [tutorial](https://www.youtube.com/watch?v=9H0aJhKSlEQ).
Then just place the downloaded `.fbx` into `Assets/Animation/Animations/Synty/Mixamo` and move any Mixamo animations used on Synty models into that folder/trigger a reimport.
