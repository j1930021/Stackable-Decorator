using StackableDecorator;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ImageGUID : EditorWindow
{
    [HorizontalGroup("Top", 1, -1, 100, spacing = 8, order = 1)]
    [Label(-1), StackableField]
    public string guid;

    [InGroup("Top", order = 1)]
    [Label(0), Slider(36, 108, showField = false)]
    public int cellSize;

    [SimpleGrid(cellSizeGetter = "#GetCellSize", columnGetter = "#GetColumn", spacing = 0, maxHeightGetter = "#GetMaxHeight")]
    public ObjectList m_ObjectList;

    private Vector2 GetCellSize() { return new Vector2(cellSize, cellSize); }
    private Vector2 GetImageSize() { return new Vector2(cellSize - 4, cellSize - 4); }
    private int GetColumn() { return Mathf.FloorToInt((position.width - 20) / (cellSize + 0)); }
    private float GetMaxHeight() { return position.height - 20; }

    private void GetGuid(SerializedProperty property)
    {
        if (property.propertyType != SerializedPropertyType.ObjectReference) return;
        if (property.objectReferenceValue == null) return;
        GUIUtility.keyboardControl = 0;
        var path = AssetDatabase.GetAssetPath(property.objectReferenceValue);
        var guid = AssetDatabase.AssetPathToGUID(path);
        this.guid = guid;
        GUIUtility.systemCopyBuffer = guid;
        Repaint();
    }

    [Serializable]
    public class ObjectList
    {
        [OnClick("GetGuid", order = 3)]
        [Box(2, 2, 2, 2, style = "ProjectBrowserTextureIconDropShadow", order = 2)]
        [AsImage(sizeGetter = "#GetImageSize")]
        public List<Texture2D> list;
    }

    [MenuItem("Tools/Image GUID")]
    public static void Init()
    {
        GetWindow<ImageGUID>("Image GUID");
    }

    private SerializedObject m_SerializedObject = null;

    void OnEnable()
    {
        m_SerializedObject = new SerializedObject(this);
        m_ObjectList = new ObjectList();
        m_ObjectList.list = new List<Texture2D>();

        var guids = AssetDatabase.FindAssets("t:Texture");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (asset != null)
                m_ObjectList.list.Add(asset);
        }

        cellSize = 36;
    }

    void OnGUI()
    {
        InlineProperty.Draw(position.Position(0, 0).Shrink(3, 3), m_SerializedObject, false);
    }
}