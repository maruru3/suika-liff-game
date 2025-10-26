using UnityEngine;
using UnityEditor;

/// <summary>
/// 必要なタグを自動追加するエディタスクリプト
/// </summary>
[InitializeOnLoad]
public class TagSetup
{
    static TagSetup()
    {
        AddTag("Floor");
        AddTag("Wall");
    }

    static void AddTag(string tag)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        // タグが既に存在するかチェック
        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(tag))
            {
                found = true;
                break;
            }
        }

        // タグが存在しない場合は追加
        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = tag;
            tagManager.ApplyModifiedProperties();
            Debug.Log("Tag added: " + tag);
        }
    }
}
