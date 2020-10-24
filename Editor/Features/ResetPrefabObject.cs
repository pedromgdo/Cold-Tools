#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace ColdHours.Editor
{
    [InitializeOnLoad]
    public class ResetPrefabObject
    {
        public static bool IsOnFirst = false;
        public static bool IsOnAll = false;

        static ResetPrefabObject() {
            EditorSceneManager.sceneOpened += ResetObject;
            EditorSceneManager.sceneOpened += ResetAllObjects;
        }
        public static void ResetObject() {
            var obj = GameObject.Find(ColdFeatures.GameObjectName);
            PrefabUtility.RevertObjectOverride(obj,InteractionMode.AutomatedAction);
            int a = 0;
            Dictionary<Object, bool> p = new Dictionary<Object, bool>();
            RecursiveRevertPrefabInstances(obj, ref a, ref p);
            if (a > 0) {
                EditorSceneManager.MarkAllScenesDirty();
                EditorSceneManager.SaveOpenScenes();
                Debug.Log("Reset Object");
            }
            Selection.activeGameObject = obj;
        }
        private static void ResetObject(Scene scene, OpenSceneMode mode) {
            if (!Application.isPlaying && IsOnFirst) {
                var obj = GameObject.Find(ColdFeatures.GameObjectName);
                PrefabUtility.RevertObjectOverride(obj, InteractionMode.AutomatedAction);
                int a = 0;
                Dictionary<Object, bool> p = new Dictionary<Object, bool>();
                RecursiveRevertPrefabInstances(obj, ref a, ref p);
                if (a > 0) {
                    EditorSceneManager.MarkAllScenesDirty();
                    EditorSceneManager.SaveOpenScenes();
                    Debug.Log("Reset Object");
                }
                Selection.activeGameObject = obj;
            }
        }
        public static void ResetAllObjects() {
            List<Object> objs = new List<Object>();
            int a = 0;
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>()) {
                if (go.name.Equals(ColdFeatures.GameObjectName)) {
                    objs.Add(go);
                    PrefabUtility.RevertObjectOverride(go, InteractionMode.AutomatedAction);
                    Dictionary<Object, bool> p = new Dictionary<Object, bool>();
                    RecursiveRevertPrefabInstances(go, ref a, ref p);
                }
            }
            if (a > 0) {
                EditorSceneManager.MarkAllScenesDirty();
                EditorSceneManager.SaveOpenScenes();
                Debug.Log("Reset All Objects");
            }
            Selection.objects = objs.ToArray();
        }

        private static void ResetAllObjects(Scene scene, OpenSceneMode mode) {
            if (!Application.isPlaying && IsOnAll) {
                List<Object> objs = new List<Object>();
                int a = 0;
                foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>()) {
                    if (go.name.Equals(ColdFeatures.GameObjectName)) {
                        objs.Add(go);
                        PrefabUtility.RevertObjectOverride(go, InteractionMode.AutomatedAction);
                        Dictionary<Object, bool> p = new Dictionary<Object, bool>();
                        RecursiveRevertPrefabInstances(go, ref a, ref p);
                    }
                }
                if (a > 0) {
                    EditorSceneManager.MarkAllScenesDirty();
                    EditorSceneManager.SaveOpenScenes();
                    Debug.Log("Reset All Objects");
                }
                Selection.objects = objs.ToArray();
            }
        }
        public static void ResetSelected() {
            int a = 0;
            foreach (GameObject go in Selection.gameObjects) {
                PrefabUtility.RevertObjectOverride(go, InteractionMode.AutomatedAction);
                Dictionary<Object, bool> p = new Dictionary<Object, bool>();
                RecursiveRevertPrefabInstances(go, ref a, ref p);
            }
            if (a > 0) {
                EditorSceneManager.MarkAllScenesDirty();
                EditorSceneManager.SaveOpenScenes();
                Debug.Log("Reset All Objects");
            }
        }
        /// <summary>
        /// This allows for both nested prefabs as well as simply going into object trees without having to expand the whole tree first.
        /// </summary>
        static void RecursiveRevertPrefabInstances(GameObject obj, ref int revertedCount, ref Dictionary<UnityEngine.Object, bool> prefabsAlreadyReverted) {
            if (obj == null)
                return;
            if (IsAPrefabNotYetReverted(obj, ref prefabsAlreadyReverted)) {
                revertedCount++;
                PrefabUtility.RevertPrefabInstance(obj, InteractionMode.AutomatedAction);
            }
            Transform trans = obj.transform;
            for (int i = 0; i < trans.childCount; i++)
                RecursiveRevertPrefabInstances(trans.GetChild(i).gameObject, ref revertedCount, ref prefabsAlreadyReverted);
        }

        /// <summary>
        /// This keeps us from reverting the same prefab over and over, which otherwise happens when we're doing checks for nested prefabs.
        /// </summary>
        static bool IsAPrefabNotYetReverted(GameObject obj, ref Dictionary<UnityEngine.Object, bool> prefabsAlreadyReverted) {
            bool wasValidAtEitherLevel = false;
            UnityEngine.Object prefab = PrefabUtility.GetPrefabInstanceHandle(obj);
            if (prefab != null && !prefabsAlreadyReverted.ContainsKey(prefab)) {
                wasValidAtEitherLevel = true;
                prefabsAlreadyReverted[prefab] = true;
            }
            prefab = PrefabUtility.GetPrefabInstanceHandle(obj);
            if (prefab != null && !prefabsAlreadyReverted.ContainsKey(prefab)) {
                wasValidAtEitherLevel = true;
                prefabsAlreadyReverted[prefab] = true;
            }
            return wasValidAtEitherLevel;
        }
    }
}
#endif