using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Nomnom.TagsAndLayers.Editor {
	public sealed class TagGenerator {
		private const string PATH_TO_FILE = "/Plugins/Nomnom's Tags And Layers/GeneratedTags.cs";
		private string finalPath => Application.dataPath + $"/{PATH_TO_FILE}";
		
		private string[] _internalTags;

		public TagGenerator() {
			// check if the generated file exists
			CheckForDirectories();

			Type ty = Type.GetType($"Nomnom.TagsAndLayers.UnityTag");
			if (ty == null) {
				_internalTags = new string[0];
				return;
			}
			
			// type exists, grab enum
			_internalTags = Enum.GetNames(ty);
		}
		
		public bool RequiresRepaint(string[] values) {
			bool sameLength = values.Length == _internalTags.Length;

			if (!sameLength) {
				return true;
			}
			
			for (int i = 0; i < values.Length; i++) {
				string value = values[i];
				string cachedValue = _internalTags[i];

				if (value == cachedValue) {
					continue;
				}
				
				return true;
			}

			return false;
		}

		public void Generate(string[] values) {
			// properly copy over values
			if (values.Length != _internalTags.Length) {
				Array.Resize(ref _internalTags, values.Length);
			}
			
			Array.Copy(values, _internalTags, values.Length);

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
			
			// UnityTag enum
			content.AppendLine("\tpublic enum UnityTag {");
			
			for (int i = 0; i < values.Length; i++) {
				string validName = validNames[i];

				if (validName == "_") {
					continue;
				}
				
				content.AppendLine($"\t\t{validName}{(i < values.Length - 1 ? "," : string.Empty)}");
			}

			content.AppendLine("\t}");
			content.AppendLine(string.Empty);
			
			// UnityTagName class
			content.AppendLine("\tpublic static class UnityTagName {");
			
			for (int i = 0; i < values.Length; i++) {
				string validName = validNames[i];

				if (validName == "_") {
					continue;
				}

				content.AppendLine($"\t\tpublic const string {validName} = \"{values[i]}\";");
			}
			
			content.AppendLine(string.Empty);
			content.AppendLine("\t\tprivate static readonly Dictionary<UnityTag, string> _namesTag = new Dictionary<UnityTag, string> {");
			
			for (int i = 0; i < values.Length; i++) {
				string validName = validNames[i];

				if (validName == "_") {
					continue;
				}

				content.AppendLine("\t\t\t{ " + $"UnityTag.{validName}, {validName}" + " }" + (i < values.Length - 1 ? "," : string.Empty));
			}
			
			content.AppendLine("\t\t};");
			
			content.AppendLine(string.Empty);
			content.AppendLine("\t\tpublic static string Get(UnityTag layer) => _namesTag[layer];");
			
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