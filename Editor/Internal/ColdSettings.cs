#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ColdHours.Editor
{
	[Serializable]
	public class ColdSettings : ScriptableObject
	{
		[SerializeField] private string _gameObjectName = "Game Object";
		public static string GameObjectName {
			get { return Instance._gameObjectName; }
			set {
				if (Instance._gameObjectName == value) return;
				Instance._gameObjectName = value;
				Save();
			}
		}

        #region Select GameObjects
        [SerializeField] private bool _selectFirstGameObjectEnabled = false;

		public static bool SelectFirstGameObjectEnabled {
			get { return Instance._selectFirstGameObjectEnabled; }
			set {
				if (Instance._selectFirstGameObjectEnabled == value) return;
				Instance._selectFirstGameObjectEnabled = value;
				Save();
			}
		}

		[SerializeField] private bool _selectAllGameObjectsEnabled = false;

		public static bool SelectAllGameObjectsEnabled {
			get { return Instance._selectAllGameObjectsEnabled; }
			set {
				if (Instance._selectAllGameObjectsEnabled == value) return;
				Instance._selectAllGameObjectsEnabled = value;
				Save();
			}
		}
		#endregion

		#region Reset Prefabs
		[SerializeField] private bool _resetFirstPrefabEnabled = false;

		public static bool ResetFirstPrefabEnabled {
			get { return Instance._resetFirstPrefabEnabled; }
			set {
				if (Instance._resetFirstPrefabEnabled == value) return;
				Instance._resetFirstPrefabEnabled = value;
				Save();
			}
		}

		[SerializeField] private bool _resetAllPrefabsEnabled = false;

		public static bool ResetAllPrefabsEnabled {
			get { return Instance._resetAllPrefabsEnabled; }
			set {
				if (Instance._resetAllPrefabsEnabled == value) return;
				Instance._resetAllPrefabsEnabled = value;
				Save();
			}
		}
        #endregion

        #region Instance

        private static ColdSettings Instance {
			get {
				if (_instance != null) return _instance;
				_instance = LoadOrCreate();
				return _instance;
			}
		}

		private static readonly string Directory = "ProjectSettings";
		private static readonly string Path = Directory + "/ColdSettings.asset";
		private static ColdSettings _instance;

		private static void Save() {
			var instance = _instance;
			if (!System.IO.Directory.Exists(Directory)) System.IO.Directory.CreateDirectory(Directory);
			try {
				UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] { instance }, Path, true);
			}
			catch (Exception ex) {
				Debug.LogError("Unable to save ColdSettings!\n" + ex);
			}
		}

		private static ColdSettings LoadOrCreate() {
			var settings = !File.Exists(Path) ? CreateNewSettings() : LoadSettings();
			if (settings == null) {
				DeleteFile(Path);
				settings = CreateNewSettings();
			}

			settings.hideFlags = HideFlags.HideAndDontSave;

			return settings;
		}


		private static ColdSettings LoadSettings() {
			ColdSettings settingsInstance;
			try {
				settingsInstance = (ColdSettings)UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(Path)[0];
			}
			catch (Exception ex) {
				Debug.LogError("Unable to read ColdSettings, set to defaults" + ex);
				settingsInstance = null;
			}

			return settingsInstance;
		}

		private static ColdSettings CreateNewSettings() {
			_instance = CreateInstance<ColdSettings>();
			Save();

			return _instance;
		}

		private static void DeleteFile(string path) {
			if (!File.Exists(path)) return;

			var attributes = File.GetAttributes(path);
			if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);

			File.Delete(path);
		}

		#endregion
	}
}
#endif