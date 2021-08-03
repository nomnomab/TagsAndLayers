using System;
using UnityEditorInternal;

namespace Nomnom.TagsAndLayers.Editor {
	/// <summary>
	/// Watches the project and will fire an event when the project attempts to save changes.
	/// </summary>
	public sealed class AssetWatcher : AssetModificationProcessor {
		private static readonly TagGenerator _tagGenerator;
		private static readonly LayerGenerator _layerGenerator;

		static AssetWatcher() {
			_tagGenerator = new TagGenerator();
			_layerGenerator = new LayerGenerator();
		}
		
		private static string[] OnWillSaveAssets(string[] paths) {
			string[] tags = InternalEditorUtility.tags;
			string[] layers = InternalEditorUtility.layers;
			
			// resize layers to reflect Unity's decision to un-restrict User Layer 3
			Array.Resize(ref layers, layers.Length + 1);

			for (int i = layers.Length - 1; i > 3; i--) {
				layers[i] = layers[i - 1];
			}

			layers[3] = string.Empty;
			
			// check for generation requirements
			bool tagsDirty = _tagGenerator.RequiresRepaint(tags);
			bool layersDirty = _layerGenerator.RequiresRepaint(layers);

			if (tagsDirty) {
				_tagGenerator.Generate(tags);
			}
			
			if (layersDirty) {
				_layerGenerator.Generate(layers);
			}

			return paths;
		}
	}
}