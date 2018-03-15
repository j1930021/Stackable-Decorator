#if UNITY_EDITOR
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StackableDecorator
{
    public class ButtonGroup
    {
        public int hOffset = 0;
        public int vOffset = 0;

        protected GUIContent[] m_Contents;

        private string m_Styles;
        private GUIStyle m_Style = null;
        private GUIStyle m_FirstStyle;
        private GUIStyle m_MidStyle;
        private GUIStyle m_LastStyle;

        public ButtonGroup(string[] contents, string styles)
        {
            Update(contents);
            m_Styles = styles;
        }

        public ButtonGroup(GUIContent[] contents, string styles)
        {
            Update(contents);
            m_Styles = styles;
        }

        public void Update(string[] contents)
        {
            m_Contents = contents.Select(text => new GUIContent(text)).ToArray();
        }

        public void Update(GUIContent[] contents)
        {
            m_Contents = contents;
        }

        public int GetCount()
        {
            return m_Contents.Length;
        }

        public Vector2 GetButtonSize(int column)
        {
            if (m_Style == null) FindStyles();

            int count = m_Contents.Length;
            if (column == -1) column = count;

            Vector2 result = Vector2.zero;
            if (count == 0)
                result = Vector2.zero;
            else if (column <= 0)
                result = Vector2.zero;
            else
            {
                int row = count / column;
                if (count % column != 0) row++;

                float width = 0, height = 0;
                for (int i = 0; i < column; i++)
                {
                    var style = i != 0 ? m_MidStyle : m_FirstStyle;
                    if (i % column == 0) style = m_FirstStyle;
                    if (i % column == column - 1) style = m_LastStyle;
                    if (i == count - 1) style = m_LastStyle;
                    if (count == 1) style = m_Style;
                    if (column == 1) style = m_Style;
                    var size = style.CalcSize(m_Contents[i]);
                    width = Mathf.Max(width, size.x);
                    height = Mathf.Max(height, size.y);
                }
                result.x = (column * width) + CalcTotalHorizSpacing(column);
                float yspace = (Mathf.Max(m_Style.margin.top, m_Style.margin.bottom) + vOffset) * (row - 1);
                result.y = (row * height) + yspace;
            }
            return result;
        }

        private void FindStyles()
        {
            var names = m_Styles.Split(',');
            if (names.Length < 3)
            {
                m_Style = names[0];
                if (m_Style == null) m_Style = GUI.skin.button;
                FindStyles("left", "mid", "right");
            }
            else
            {
                FindStyles(names[0], names[1], names[2]);
                if (m_Style == null) m_Style = m_MidStyle;
            }
        }

        private void FindStyles(string first, string mid, string last)
        {
            var name = m_Style == null ? string.Empty : m_Style.name;

            m_MidStyle = GUI.skin.FindStyle(name + mid);
            if (m_MidStyle == null) m_MidStyle = m_Style;
            if (m_MidStyle == null) m_MidStyle = m_Style = GUI.skin.button;

            m_FirstStyle = GUI.skin.FindStyle(name + first);
            if (m_FirstStyle == null) m_FirstStyle = m_MidStyle;

            m_LastStyle = GUI.skin.FindStyle(name + last);
            if (m_LastStyle == null) m_LastStyle = m_MidStyle;
        }

        public long Draw(Rect position, long selected)
        {
            return Draw(position, selected, m_Contents.Length);
        }

        public long Draw(Rect position, long selected, int column)
        {
            if (m_Style == null) FindStyles();

            int count = m_Contents.Length;
            if (column == -1) column = count;

            long result;
            if (count == 0)
                result = selected;
            else if (column <= 0)
                result = selected;
            else
            {
                int row = count / column;
                if (count % column != 0) row++;

                float xspace = CalcTotalHorizSpacing(column);
                float yspace = (Mathf.Max(m_Style.margin.top, m_Style.margin.bottom) + vOffset) * (row - 1);
                float width = (position.width - xspace) / column;
                float height = (position.height - yspace) / row;
                if (m_Style.fixedWidth != 0f) width = m_Style.fixedWidth;
                if (m_Style.fixedHeight != 0f) height = m_Style.fixedHeight;
                Rect[] array = CalcMouseRects(position, column, width, height);

                for (int i = 0; i < count; i++)
                {
                    var style = i != 0 ? m_MidStyle : m_FirstStyle;
                    if (i % column == 0) style = m_FirstStyle;
                    if (i % column == column - 1) style = m_LastStyle;
                    if (i == count - 1) style = m_LastStyle;
                    if (count == 1) style = m_Style;
                    if (column == 1) style = m_Style;
                    selected = OnDrawButton(array[i], i, selected, style);
                }
                result = selected;
            }
            return result;
        }

        protected virtual long OnDrawButton(Rect position, int current, long selected, GUIStyle style)
        {
            var value = GUI.Toggle(position, current == selected, m_Contents[current], style);
            if (value != (current == selected))
                selected = current;
            return selected;
        }

        private int CalcTotalHorizSpacing(int xCount)
        {
            int result;
            if (xCount < 2)
                result = 0;
            else if (xCount == 2)
                result = Mathf.Max(m_FirstStyle.margin.right, m_LastStyle.margin.left) + hOffset;
            else
            {
                int num = Mathf.Max(m_MidStyle.margin.left, m_MidStyle.margin.right);
                result = Mathf.Max(m_FirstStyle.margin.right, m_MidStyle.margin.left) + Mathf.Max(m_MidStyle.margin.right, m_LastStyle.margin.left) + num * (xCount - 3);
                result += hOffset * (xCount - 1);
            }
            return result;
        }

        private Rect[] CalcMouseRects(Rect position, int column, float width, float height)
        {
            int count = m_Contents.Length;
            int col = 0;
            float x = position.xMin;
            float y = position.yMin;
            var style1 = count > 1 ? m_FirstStyle : m_Style;
            Rect[] array = new Rect[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = new Rect(x, y, width, height);
                array[i].width = array[i].xMax - array[i].x;
                var style2 = i == count - 2 ? m_LastStyle : m_MidStyle;
                x += width + Mathf.Max(style1.margin.right, style2.margin.left) + hOffset;
                col++;
                if (col >= column)
                {
                    col = 0;
                    y += height + Mathf.Max(m_Style.margin.top, m_Style.margin.bottom) + vOffset;
                    x = position.xMin;
                }
            }
            return array;
        }
    }

    public class ButtonMask : ButtonGroup
    {
        private long[] m_Mask;

        public ButtonMask(string[] contents, long[] mask, bool all, string styles) : base(contents, styles)
        {
            m_Mask = mask;
            if (all) AppendAll();
        }

        public ButtonMask(GUIContent[] contents, long[] mask, bool all, string styles) : base(contents, styles)
        {
            m_Mask = mask;
            if (all) AppendAll();
        }

        private void AppendAll()
        {
            m_Contents = m_Contents.Concat(new GUIContent("All").Yield()).ToArray();
            long allmask = 0;
            foreach (var mask in m_Mask)
                allmask |= mask;
            m_Mask = m_Mask.Concat(allmask.Yield()).ToArray();
        }

        protected override long OnDrawButton(Rect position, int current, long selected, GUIStyle style)
        {
            var mask = (selected & m_Mask[current]) == m_Mask[current];
            var value = GUI.Toggle(position, mask, m_Contents[current], style);
            if (value != mask)
            {
                if (Event.current.button == 1)
                    selected = m_Mask[current];
                else
                    selected = mask ? selected & (~m_Mask[current]) : selected | m_Mask[current];
            }
            return selected;
        }
    }

    public class MaskPopupList : PopupWindowContent
    {
        private const string kCommandName = "MaskPopupList";

        private const float kItemHeight = 18;
        private const float kHeaderHeight = 20;

        private class Styles
        {
            public GUIStyle even = "ObjectPickerResultsOdd";
            public GUIStyle odd = "ObjectPickerResultsEven";
            public GUIStyle toggle = "OL Toggle";
            public GUIStyle header = "IN BigTitle";
            public GUIStyle element;
            public Styles()
            {
                element = new GUIStyle(EditorStyles.label);
                element.alignment = TextAnchor.MiddleLeft;
                element.padding.left = 10;
                element.active = element.normal;
            }
        }
        private static Styles s_Styles;
        private static MaskPopupList s_LastPopup;
        private static GUIContent s_Content = new GUIContent();

        private float m_Width;
        private float m_MaxHeight;
        private Vector2 m_ScrollPos;
        private bool m_ScrollBar;
        private float m_WidthExpand = 0;

        private long m_Selected;
        private string[] m_Names;
        private long[] m_Values;
        private bool m_SortCombined;
        private int m_Id;
        private EditorWindow m_Window;

        public static void Popup(Rect position, long selected, IEnumerable<string> names, IEnumerable<long> values, bool sort, int id)
        {
            var type = typeof(Editor).Assembly.GetType("UnityEditor.GUIView");
            var current = type.GetProperty("current", BindingFlags.Public | BindingFlags.Static);
            type = typeof(Editor).Assembly.GetType("UnityEditor.HostView");
            var actualView = type.GetProperty("actualView", BindingFlags.NonPublic | BindingFlags.Instance);
            var window = (EditorWindow)actualView.GetValue(current.GetValue(null, null), null);

            var popup = new MaskPopupList();
            popup.m_Width = position.width;
            popup.m_Selected = selected;
            popup.m_Names = names.ToArray();
            popup.m_Values = values.ToArray();
            popup.m_SortCombined = sort;
            popup.m_Id = id;
            popup.m_Window = window;
            s_LastPopup = popup;
            PopupWindow.Show(position, popup);
        }

        public static bool IsSelectionChanged(int id)
        {
            if (s_LastPopup == null || s_LastPopup.m_Id != id) return false;
            var evt = Event.current;
            if (evt.type == EventType.ExecuteCommand && evt.commandName == kCommandName)
            {
                evt.Use();
                return true;
            }
            return false;
        }

        public static long GetLastSelectedValue()
        {
            return s_LastPopup.m_Selected;
        }

        private MaskPopupList()
        {
            m_MaxHeight = Screen.height * 0.8f;
        }

        public override Vector2 GetWindowSize()
        {
            var h = kHeaderHeight + m_Names.Length * kItemHeight;
            if (h > m_MaxHeight)
            {
                h = m_MaxHeight;
                m_ScrollBar = true;
            }
            if (m_WidthExpand != 0)
            {
                m_Width += m_WidthExpand;
                m_WidthExpand = 0;
            }
            return new Vector2(m_Width, h);
        }

        public override void OnGUI(Rect rect)
        {
            if (s_Styles == null) s_Styles = new Styles();
            DrawHeader();
            DrawList();
            if (m_WidthExpand != 0)
                editorWindow.Repaint();
        }

        private void DrawHeader()
        {
            var rect = new Rect(0, 0, m_Width, kHeaderHeight);
            GUI.Label(rect, GUIContent.none, s_Styles.header);

            long allmask = 0;
            foreach (var mask in m_Values)
                allmask |= mask;
            bool all = m_Selected == allmask;
            EditorGUI.BeginChangeCheck();

            rect.yMin = rect.yMax - 18;
            GUI.Button(rect, all ? "Select None" : "Select All", s_Styles.element);

            rect = new Rect(rect.xMax - 23, rect.y + (rect.height - 17) / 2, 17, 17);
            if (m_ScrollBar)
                rect.x -= 14;
            GUI.Toggle(rect, all, GUIContent.none, s_Styles.toggle);

            if (EditorGUI.EndChangeCheck())
            {
                var value = all ? 0 : allmask;
                if (value != m_Selected)
                {
                    m_Selected = value;
                    SendCommand();
                }
            }
        }

        private void DrawList()
        {
            bool even = false;
            var rect = new Rect(0, 0, m_Width, kItemHeight);
            if (m_ScrollBar)
            {
                rect.width -= 14;
                var scroll = new Rect(0, kHeaderHeight, m_Width, m_MaxHeight - kHeaderHeight);
                var view = new Rect(0, 0, m_Width - 15, m_Names.Length * kItemHeight);
                m_ScrollPos = GUI.BeginScrollView(scroll, m_ScrollPos, view);
            }
            else
                rect.y += kHeaderHeight;

            for (int i = 0; i < m_Names.Length; i++)
                if (!m_SortCombined || m_Values[i].IsPowerOfTwo())
                {
                    GUI.Label(rect, GUIContent.none, even ? s_Styles.even : s_Styles.odd);
                    DrawItem(rect, i);
                    rect.y += kItemHeight;
                    even = !even;
                }
            if (m_SortCombined)
                for (int i = 0; i < m_Names.Length; i++)
                    if (!m_Values[i].IsPowerOfTwo())
                    {
                        GUI.Label(rect, GUIContent.none, even ? s_Styles.even : s_Styles.odd);
                        DrawItem(rect, i);
                        rect.y += kItemHeight;
                        even = !even;
                    }

            if (m_ScrollBar)
                GUI.EndScrollView();
        }

        private void DrawItem(Rect rect, int index)
        {
            rect.width -= 23;
            s_Content.text = m_Names[index];
            var width = s_Styles.element.CalcSize(s_Content).x + 8;
            if (width > rect.width)
                m_WidthExpand = Mathf.Max(m_WidthExpand, width - rect.width);
            if (GUI.Button(rect, m_Names[index], s_Styles.element))
                Toggle(index);
            rect = new Rect(rect.xMax, rect.y + (rect.height - 17) / 2, 17, 17);
            var value = GUI.Toggle(rect, IsSelected(index), GUIContent.none, s_Styles.toggle);
            if (value != IsSelected(index))
                Toggle(index);
        }

        private bool IsSelected(int index)
        {
            return (m_Selected & m_Values[index]) == m_Values[index];
        }

        private void Toggle(int index)
        {
            if (Event.current.button == 1)
                m_Selected = m_Values[index];
            else
                m_Selected = IsSelected(index) ? m_Selected & (~m_Values[index]) : m_Selected | m_Values[index];
            SendCommand();
        }

        private void SendCommand()
        {
            m_Window.SendEvent(EditorGUIUtility.CommandEvent(kCommandName));
        }
    }

    public static class InlineProperty
    {
        public static float GetHeight(SerializedObject serializedObject, List<float> propertyHeights = null)
        {
            float result = 0;
            if (propertyHeights != null) propertyHeights.Clear();
            serializedObject.Update();
            var iterator = serializedObject.GetIterator();
            bool enter = true;
            InGroupAttribute.Push();
            while (iterator.NextVisible(enter))
            {
                enter = false;
                if (iterator.propertyPath.Equals("m_Script")) continue;
                var h = EditorGUI.GetPropertyHeight(iterator, null, true);
                if (h < 0) h = 0;
                if (propertyHeights != null) propertyHeights.Add(h);
                if (result > 0 && h > 0) result += 2;
                result += h;
            }
            InGroupAttribute.Pop();
            return Mathf.Max(0, result);
        }

        public static void Draw(Rect position, SerializedObject serializedObject, bool indent, List<float> propertyHeights = null)
        {
            if (Event.current.type == EventType.Layout) return;

            var rect = new Rect(position);
            int indentLevel = EditorGUI.indentLevel;

            int index = 0;
            serializedObject.Update();
            var iterator = serializedObject.GetIterator();
            bool enter = true;
            InGroupAttribute.Push();
            while (iterator.NextVisible(enter))
            {
                enter = false;
                if (iterator.propertyPath.Equals("m_Script")) continue;
                EditorGUI.indentLevel = indentLevel + (indent ? 1 : 0);
                if (propertyHeights != null)
                    rect.height = propertyHeights[index];
                else
                    rect.height = EditorGUI.GetPropertyHeight(iterator, null, true);
                if (rect.height > 0)
                {
                    EditorGUI.PropertyField(rect, iterator, null, true);
                    rect = rect.MoveDown(2);
                }
                index++;
            }
            InGroupAttribute.Pop();
            serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indentLevel;
        }

        private static List<FieldInfo> s_FieldInfos = new List<FieldInfo>();

        public static void Draw(SerializedObject serializedObject, bool indent)
        {
            int indentLevel = EditorGUI.indentLevel;

            serializedObject.Update();
            var iterator = serializedObject.GetIterator();
            bool enter = true;
            InGroupAttribute.Push();
            while (iterator.NextVisible(enter))
            {
                enter = false;
                if (iterator.propertyPath.Equals("m_Script")) continue;
                iterator.GetFieldInfos(s_FieldInfos);
                if (s_FieldInfos[s_FieldInfos.Count - 1].GetCustomAttributes(typeof(InGroupAttribute), false).Any())
                    continue;
                EditorGUI.indentLevel = indentLevel + (indent ? 1 : 0);
                var h = EditorGUI.GetPropertyHeight(iterator, null, true);
                if (h > 0)
                {
                    var rect = EditorGUILayout.GetControlRect(true, h);
                    if (Event.current.type != EventType.Layout)
                        EditorGUI.PropertyField(rect, iterator, null, true);
                }
            }
            InGroupAttribute.Pop();
            serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indentLevel;
        }
    }
}
#endif