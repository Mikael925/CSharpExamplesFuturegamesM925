using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EntitySettings))]
public class EntitySettingsEditor : Editor {
    private EntitySettings entitySettings;
    private GUIStyle greenField;
    private GUIStyle redField;

    public List<Texture2D> textures = new ();
    private Texture2D greenTexture;
    private Texture2D redTexture;
    private bool initialized;

    private void OnEnable() {
        entitySettings = (EntitySettings)target;

        // create styles
        greenTexture = CreateTexture(Color.green);
        textures.Add(greenTexture);
        redTexture = CreateTexture(Color.red);
        textures.Add(redTexture);
    }

    private Texture2D CreateTexture(Color color) {
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        texture.hideFlags = HideFlags.HideAndDontSave;
        return texture;
    }

    public override void OnInspectorGUI() {
        CreateStyles();

        EditorStyles.largeLabel.alignment = TextAnchor.MiddleCenter;
        EditorGUILayout.LabelField("Entity Settings", EditorStyles.largeLabel);
        EditorGUILayout.Space(20);

        //DrawDefaultInspector();
        DrawMyCustomInspector();

        EditorGUILayout.Space(20);

        if(GUILayout.Button("SaveAsset")) {
            EditorUtility.SetDirty(entitySettings);
            AssetDatabase.SaveAssets();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void CreateStyles() {
        greenField = new GUIStyle(EditorStyles.numberField) {
            normal = {
                background = greenTexture,
                textColor = Color.black
            }
        };
        redField = new GUIStyle(EditorStyles.numberField) {
            normal = {
                background = redTexture,
                textColor = Color.white
            }
        };
    }

    private void DrawMyCustomInspector() {
        EditorGUILayout.LabelField("Base Stats", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        entitySettings.entityName = EditorGUILayout.TextField("Entity Name", entitySettings.entityName, String.IsNullOrEmpty(entitySettings.entityName) ? redField : greenField);
        entitySettings.health = EditorGUILayout.IntField("Entity Health", entitySettings.health, entitySettings.health <= 0 ? redField : greenField);
        entitySettings.mana = EditorGUILayout.IntField("Entity Mana", entitySettings.mana, entitySettings.mana <= 0 ? redField : greenField);
        entitySettings.strength = EditorGUILayout.IntField("Entity Strength", entitySettings.strength);
        entitySettings.dexterity = EditorGUILayout.IntField("Entity Dexterity", entitySettings.dexterity);
        entitySettings.intelligence = EditorGUILayout.IntField("Entity Intelligence", entitySettings.intelligence);
        entitySettings.vitality = EditorGUILayout.IntField("Entity Vitality", entitySettings.vitality);
        entitySettings.luck = EditorGUILayout.IntField("Entity Luck", entitySettings.luck);
        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField("Inventory & Equipment", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        entitySettings.currentInventory = (Inventory)EditorGUILayout.ObjectField("Current Inventory", entitySettings.currentInventory, typeof(Inventory), false);
    }

    private void OnDisable() {
        foreach(Texture2D texture in textures) {
            DestroyImmediate(texture);
        }
    }
}
