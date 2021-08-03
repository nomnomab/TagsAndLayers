using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Nomnom.TagsAndLayers.Editor {
	public sealed class LayerGenerator {
		private const string PATH_TO_FILE = "/Plugins/Nomnom's Tags And Layers/GeneratedLayers.cs";
		private string finalPath => Application.dataPath + $"/{PATH_TO_FILE}";
		
		private string[] _internalLayers;

		public LayerGenerator() {
			// check if the generated file exists
			CheckForDirectories();

			Type ty = Type.GetType($"Nomnom.TagsAndLayers.UnityLayer");
			if (ty == null) {
				_internalLayers = new string[0];
				return;
			}
			
			// type exists, grab enum
			_internalLayers = Enum.GetNames(ty);
		}

		public bool RequiresRepaint(string[] values) {
			bool sameLength = values.Length == _internalLayers.Length;

			if (!sameLength) {
				return true;
			}
			
			// skip layers 0, 1, 2, 4, and 5
			for (int i = 3; i < values.Length; i++) {
				if (i == 4 || i == 5) {
					continue;
				}

				string value = values[i];
				string cachedValue = _internalLayers[i];

				if (value == cachedValue) {
					continue;
				}
				
				return true;
			}

			return false;
		}

		public void Generate(string[] values) {
			// properly copy over values
			if (values.Length != _internalLayers.Length) {
				Array.Resize(ref _internalLayers, values.Length);
			}
			
			Array.Copy(values, _internalLayers, values.Length);

			string[] validNames = new string[values.Length];

			for (int i = 0; i < values.Length; i++) {
				validNames[i] = string.IsNullOrEmpty(values[i]) ? "_" : values[i].Replace(" ", "_");
			}
			
			// generate string-front for the new script
			StringBuilder content = new StringBuilder();
			
			// usings
			content.AppendLine("// This script is generated when the project is saved.");
			content.AppendLine("// Any changes will be reverted.");
			content.AppendLine(string.Empty);
			
			content.AppendLine("using System;");
			content.AppendLine("using System.Collections.Generic;");
			content.AppendLine(string.Empty);
			
			// namespace
			content.AppendLine("namespace Nomnom.TagsAndLayers {");
			
			// UnityLayer enum
			content.AppendLine("\tpublic enum UnityLayer {");

			for (int i = 0; i < values.Length; i++) {
				string validName = validNames[i];

				if (validName == "_") {
					continue;
				}
				
				content.AppendLine($"\t\t{validName} = {i}{(i < values.Length - 1 ? "," : string.Empty)}");
			}

			content.AppendLine("\t}");
			content.AppendLine(string.Empty);
			
			// UnityLayerMask enum
			content.AppendLine("\t[Flags]");
			content.AppendLine("\tpublic enum UnityLayerMask {");
			
			for (int i = 0; i < values.Length; i++) {
				string validName = validNames[i];

				if (validName == "_") {
					continue;
				}

				content.AppendLine($"\t\t{validName} = 1 << UnityLayer.{validName}{(i < values.Length - 1 ? "," : string.Empty)}");
			}

			content.AppendLine("\t}");
			content.AppendLine(string.Empty);
			
			// UnityLayerName class
			content.AppendLine("\tpublic static class UnityLayerName {");
			
			for (int i = 0; i < values.Length; i++) {
				string validName = validNames[i];

				if (validName == "_") {
					continue;
				}

				content.AppendLine($"\t\tpublic const string {validName} = \"{values[i]}\";");
			}

			content.AppendLine(string.Empty);
			content.AppendLine("\t\tprivate static readonly Dictionary<UnityLayer, string> _namesLayer = new Dictionary<UnityLayer, string> {");
			
			for (int i = 0; i < values.Length; i++) {
				string validName = validNames[i];

				if (validName == "_") {
					continue;
				}

				content.AppendLine("\t\t\t{ " + $"UnityLayer.{validName}, {validName}" + " }" + (i < values.Length - 1 ? "," : string.Empty));
			}
			
			content.AppendLine("\t\t};");
			
			content.AppendLine(string.Empty);
			content.AppendLine("\t\tprivate static readonly Dictionary<UnityLayerMask, string> _namesMask = new Dictionary<UnityLayerMask, string> {");
			
			for (int i = 0; i < values.Length; i++) {
				string validName = validNames[i];

				if (validName == "_") {
					continue;
				}

				content.AppendLine("\t\t\t{ " + $"UnityLayerMask.{validName}, {validName}" + " }" + (i < values.Length - 1 ? "," : string.Empty));
			}
			
			content.AppendLine("\t\t};");
			
			content.AppendLine(string.Empty);
			content.AppendLine("\t\tpublic static string Get(UnityLayer layer) => _namesLayer[layer];");

			content.AppendLine("\t}");
			content.AppendLine("}");

			string srcCode = content.ToString();

			CheckForDirectories();
			
			File.WriteAllText(finalPath, srcCode);
			
			AssetDatabase.Refresh();
		}

		private void CheckForDirectories() {
			string pluginsDirectory = Application.dataPath + "/Plugins";
			string packageDirectory = pluginsDirectory + "/Nomnom's Tags and Layers";

			if (!Directory.Exists(pluginsDirectory)) {
				Directory.CreateDirectory(pluginsDirectory);
			}
			
			if (!Directory.Exists(packageDirectory)) {
				Directory.CreateDirectory(packageDirectory);
			}
			
			AssetDatabase.Refresh();
		}
	}
}