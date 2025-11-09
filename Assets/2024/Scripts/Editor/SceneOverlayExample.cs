using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class SceneOverlayExample {
    static void OnSceneGUI(SceneView sceneView) {
        Handles.BeginGUI();

        Rect bgRect = new Rect(10, 10, 220, 120);
        GUI.Box(bgRect, "");

        Rect buttonRect = new Rect(20, 20, 200, 100);
        if(GUI.Button(buttonRect, "Save All Scenes")) {
            SaveAllScenes();
        }
        Handles.EndGUI();
    }

    [MenuItem("ExampleTools/Save All Open Scenes %#&s")] // Shortcut: Ctrl+Shift+Alt+S
    public static void SaveAllScenes() {
        int sceneCount = SceneManager.sceneCount;
        bool allSaved = true;

        for(int i = 0; i < sceneCount; i++) {
            Scene scene = SceneManager.GetSceneAt(i);

            if(string.IsNullOrEmpty(scene.path)) {
                string path = EditorUtility.SaveFilePanelInProject(
                    "Save New Scene",
                    scene.name,
                    "unity",
                    "Specify where to save the new scene."
                );

                if(!string.IsNullOrEmpty(path)) {
                    if(!EditorSceneManager.SaveScene(scene, path)) {
                        allSaved = false;
                        Debug.LogError("Failed to save new scene: " + scene.name);
                    }
                }
                else {
                    allSaved = false;
                    Debug.LogWarning("Save canceled for new scene: " + scene.name);
                }
            }
            else if(scene.isDirty)
            {
                if(!EditorSceneManager.SaveScene(scene)) {
                    allSaved = false;
                    Debug.LogError("Failed to save scene: " + scene.path);
                }
            }
        }

        if(allSaved) {
            Debug.Log("All open scenes saved successfully.");
        }
        else {
            Debug.LogWarning("Some scenes were not saved.");
        }
    }
}
