#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


namespace ColdHours.Editor
{
	[InitializeOnLoad]
	public class ColdWindow : EditorWindow
	{
		private static EditorWindow _windowInstance;


		[MenuItem("Tools/Cold Tools/Cold Window", priority = 1)]
		private static void OpenColdWindow() {
			_windowInstance = GetWindow<ColdWindow>();
			_windowInstance.titleContent = new GUIContent("Cold Window");
			Rect windowSize = _windowInstance.position;
			windowSize.height = 300;
			windowSize.width = 400;
			windowSize.center = new Vector2(Screen.height / 2, Screen.width / 2);
			_windowInstance.position = windowSize;
		}

		private void OnEnable() {
			_windowInstance = this;
		}

		private void OnGUI() {
			//To center any labels and other stuffs
			var centeredStyleLabel = GUI.skin.GetStyle("Label");
			var centeredStyleTextField = GUI.skin.GetStyle("TextField");
			centeredStyleLabel.alignment = TextAnchor.UpperCenter;
			centeredStyleTextField.alignment = TextAnchor.UpperCenter;
			EditorGUILayout.Space();
			GUI.contentColor = Color.cyan;
			EditorGUILayout.LabelField("You are using the Cold Window!", centeredStyleLabel);
			EditorGUILayout.Space();
			GUI.contentColor = Color.white;
			EditorGUILayout.LabelField("Name of the Object that the Tools will use:", centeredStyleLabel);
			ColdFeatures.GameObjectName = EditorGUILayout.TextField(ColdFeatures.GameObjectName, centeredStyleTextField);
			EditorGUILayout.Space(10);
			using (new EditorGUILayout.HorizontalScope()) {
				if (GUILayout.Button("Find Object", EditorStyles.miniButton)) {
					SelectGameObject.FindObject();
				}
				if (GUILayout.Button("Find All Objects", EditorStyles.miniButton)) {
					SelectGameObject.FindAllObjects();
				}
			}
			using (new EditorGUILayout.HorizontalScope()) {
				if (GUILayout.Button("Reset Prefab", EditorStyles.miniButton)) {
					ResetPrefabObject.ResetObject();
				}

				if (GUILayout.Button("Reset All Prefabs", EditorStyles.miniButton)) {
					ResetPrefabObject.ResetAllObjects();
				}
			}
			if (GUILayout.Button("Reset Selected Prefabs", EditorStyles.miniButton)) {
				ResetPrefabObject.ResetSelected();
			}
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			GUI.contentColor = Color.cyan;
			EditorGUILayout.LabelField("More Functionality soon...", centeredStyleLabel);

			centeredStyleLabel.alignment = TextAnchor.UpperLeft;
			centeredStyleTextField.alignment = TextAnchor.UpperLeft;
		}
	}
}
#endif