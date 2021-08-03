# TagsAndLayers
This asset allows users to use tags and layers without resorting to using strings for value lookups.

## Known Issues
- Having a layer with empty slots before it, after index 5, will cause the layers to no longer be synced to their respective index.
  - e.g. Index 8 is filled, but indicies 6 and 7 are empty.
  - Index 3 is fine when left empty, this is considered and dealt with internally.

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