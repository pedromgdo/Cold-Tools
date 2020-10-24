#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace ColdHours.Editor
{
    [InitializeOnLoad]
    public class SelectGameObject
    {
        public static bool IsOnFirst = false;
        public static bool IsOnAll = false;

        static SelectGameObject() {
            EditorSceneManager.sceneOpened += FindObject;
            EditorSceneManager.sceneOpened += FindAllObjects;
        }
        public static void FindObject() {
            Debug.Log("Setting active Object");
            var obj = GameObject.Find(ColdFeatures.GameObjectName);
            Selection.activeGameObject = obj;
        }
        private static void FindObject(Scene scene, OpenSceneMode mode) {
            if (!Application.isPlaying && IsOnFirst) {
                Debug.Log("Setting active Object");
                var obj = GameObject.Find(ColdFeatures.GameObjectName);
                Selection.activeGameObject = obj;
            }
        }
        public static void FindAllObjects() {
            Debug.Log("Setting active Object");
            List<Object> objs = new List<Object>();
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
                if (go.name.Equals(ColdFeatures.GameObjectName)) objs.Add(go);
            Selection.objects = objs.ToArray();
        }
        private static void FindAllObjects(Scene scene, OpenSceneMode mode) {
            if (!Application.isPlaying && IsOnAll) {
                Debug.Log("Setting active Object");
                List<Object> objs = new List<Object>();
                foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
                    if (go.name.Equals(ColdFeatures.GameObjectName)) objs.Add(go);
                Selection.objects = objs.ToArray();
            }
        }
    }
}
#endif