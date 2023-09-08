using UnityEngine;
using UnityEditor;

public class EditorTextDisplay : EditorWindow
{
    private string enteredText = "";

    [MenuItem("Window/Custom Text Editor")]
    public static void ShowWindow()
    {
        GetWindow<EditorTextDisplay>("Text Editor");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Enter Text:");
        enteredText = EditorGUILayout.TextField(enteredText);

        if (GUILayout.Button("Save"))
        {
            SaveText();
        }
    }

    private void SaveText()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null)
        {
            CustomTextComponent customTextComponent = selectedObject.GetComponent<CustomTextComponent>();
            if (customTextComponent != null)
            {
                customTextComponent.customText = enteredText;
                EditorUtility.SetDirty(customTextComponent);
            }
            else
            {
                customTextComponent = selectedObject.AddComponent<CustomTextComponent>();
                customTextComponent.customText = enteredText;
                EditorUtility.SetDirty(customTextComponent);
            }
        }
    }
}

[System.Serializable]
public class CustomTextComponent : MonoBehaviour
{
    public string customText;
}