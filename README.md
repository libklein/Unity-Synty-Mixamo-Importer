
# Unity Synty Mixamo Animation Importer

A editor script to automatically apply the recommended settings from Synty Studios (see https://www.youtube.com/watch?v=9H0aJhKSlEQ) to ensure that Animations imported from Mixamo work with Synty characters. It additionally (optionally) renames the animation clip according to the name of the imported asset to avoid multiple animations with the name `mixamo.com`. 

`SyntyMixamoImporter.cs` simply applies a preset (`MixamoPreset.preset`) to any `.fbx` added to `Assets/Animation/Animations/Synty/Mixamo/` which configures the animation correctly. 
Then, `SyntyMixamoAnimationRenamer.cs` renames the animation clip `mixamo.com` according to the filename of the asset and removes any other clips. If you do not want this to happen, just remove `SyntyMixamoAnimationRenamer.cs`.
## Getting Started

Simply download the repository and place it's contents right into your unity project directory.
## Usage/Examples

Download an animation from Mixamo with the settings/character described in the [tutorial](https://www.youtube.com/watch?v=9H0aJhKSlEQ).
Then just place the downloaded `.fbx` into `Assets/Animation/Animations/Synty/Mixamo` and your're good to go.
## Technical Details

When a new asset with extension `.fbx` is added to any folder in `Assets/Animation/Animations/Synty/Mixamo`, the script searches all parent folders up to and including `Assets/Animation/Animations/Synty/Mixamo` for presets which it automatically applies in order of distance:
Adding `WalkForwards.fbx` to `Assets/Animation/Animations/Synty/Mixamo/MyCharacter/Movement/` would first apply presets in the following order:
1) `Assets/Animation/Animations/Synty/Mixamo/*.preset`
2) `Assets/Animation/Animations/Synty/Mixamo/MyCharacter/*.preset`
3) `Assets/Animation/Animations/Synty/Mixamo/MyCharacter/Movement/*.preset`