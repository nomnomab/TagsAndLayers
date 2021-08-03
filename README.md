# TagsAndLayers
This asset allows users to use tags and layers without resorting to using strings for value lookups.

## Usage
```cs
// getting the index of a layer
UnityLayer uiLayer = UnityLayer.UI;

// creating a layer mask
int mask = (int)(UnityLayerMask.UI | UnityLayerMask.Water);

// get the name of a layer
string uiLayerString = UnityLayerName.Get(uiLayer); // or UnityLayerName.UI

// get a tag
UnityTag playerTag = UnityTag.Player;
string playerTagString = UnityTagName.Get(playerTag); // or UnityTagName.Player
```

## Installation
#### Using Unity Package Manager
1. Open the Package Manager from `Window/Package Manager`
2. Click the '+' button in the top-left of the window
3. Click 'Add package from git URL'
4. Provide the URL of this git repository: https://github.com/nomnomab/TagsAndLayers.git
5. Click the 'add' button