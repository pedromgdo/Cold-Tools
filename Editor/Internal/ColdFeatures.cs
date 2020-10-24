#if UNITY_EDITOR
using UnityEditor;

namespace ColdHours.Editor
{
	[InitializeOnLoad]
	public class ColdFeatures
	{
		private const string SelectFirstObjectMenuItem = "Tools/Cold Tools/Select First Object";
		private const string SelectAllObjectsMenuItem  = "Tools/Cold Tools/Select All Objects";
		private const string ResetFirstPrefabMenuItem  = "Tools/Cold Tools/Reset First Prefab";
		private const string ResetAllPrefabsMenuItem = "Tools/Cold Tools/Reset All Prefabs";

		static ColdFeatures() {
			SelectFirstObjectEnabled = SelectFirstObjectEnabled;
			SelectAllObjectsEnabled = SelectAllObjectsEnabled;
			ResetFirstPrefabEnabled = ResetFirstPrefabEnabled;
			ResetAllPrefabsEnabled = ResetAllPrefabsEnabled;
			GameObjectName = GameObjectName;
		}

		//Generic
		public static string GameObjectName {
			get { return ColdSettings.GameObjectName; }
			set { ColdSettings.GameObjectName = value; }
		}

		#region Select First Object

		private static bool SelectFirstObjectEnabled {
			get => ColdSettings.SelectFirstGameObjectEnabled;
			set {
				{
					ColdSettings.SelectFirstGameObjectEnabled = value;
					SelectGameObject.IsOnFirst = value;
				}
			}
		}

		[MenuItem(SelectFirstObjectMenuItem, true)]
		static bool ValidadeFirstObject() {
			Menu.SetChecked(SelectFirstObjectMenuItem, SelectFirstObjectEnabled);
			return true;
		}
		[MenuItem(SelectFirstObjectMenuItem, priority = 100)]
		static void SelectFirstObject() {
			SelectFirstObjectEnabled = !SelectFirstObjectEnabled;
		}

		#endregion

		#region Select All Objects
		private static bool SelectAllObjectsEnabled {
			get => ColdSettings.SelectAllGameObjectsEnabled;
			set {
				{
					ColdSettings.SelectAllGameObjectsEnabled = value;
					SelectGameObject.IsOnAll = value;
				}
			}
		}

		[MenuItem(SelectAllObjectsMenuItem, true)]
		static bool ValidadeAllObjects() {
			Menu.SetChecked(SelectAllObjectsMenuItem, SelectAllObjectsEnabled);
			return true;
		}
		[MenuItem(SelectAllObjectsMenuItem, priority = 100)]
		static void SelectAllObjects() {
			SelectAllObjectsEnabled = !SelectAllObjectsEnabled;
		}

		#endregion

		#region Reset First Prefab
		private static bool ResetFirstPrefabEnabled {
			get => ColdSettings.ResetFirstPrefabEnabled;
			set {
				{
					ColdSettings.ResetFirstPrefabEnabled = value;
					ResetPrefabObject.IsOnFirst = value;
				}
			}
		}

		[MenuItem(ResetFirstPrefabMenuItem, true)]
		static bool ValidadeFirstPrefab() {
			Menu.SetChecked(ResetFirstPrefabMenuItem, ResetFirstPrefabEnabled);
			return true;
		}
		[MenuItem(ResetFirstPrefabMenuItem, priority = 100)]
		static void ResetFirstPrefab() {
			ResetFirstPrefabEnabled = !ResetFirstPrefabEnabled;
		}
		#endregion

		#region Reset All Prefabs

		private static bool ResetAllPrefabsEnabled {
			get => ColdSettings.ResetAllPrefabsEnabled;
			set {
				{
					ColdSettings.ResetAllPrefabsEnabled = value;
					ResetPrefabObject.IsOnAll = value;
				}
			}
		}

		[MenuItem(ResetAllPrefabsMenuItem, true)]
		static bool ValidadeAllPrefabs() {
			Menu.SetChecked(ResetAllPrefabsMenuItem, ResetAllPrefabsEnabled);
			return true;
		}
		[MenuItem(ResetAllPrefabsMenuItem, priority = 100)]
		static void SelectAllPrefabs() {
			ResetAllPrefabsEnabled = !ResetAllPrefabsEnabled;
		}

		#endregion

	}
}
#endif