using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public class OnDragDropAttribute : StackableDecoratorAttribute
    {
        public int button = 0;
        public bool autoDrag = true;
        public bool autoDrop = true;
        public string drag = string.Empty;
        public string accept = string.Empty;
        public string drop = string.Empty;
        public bool use = true;
        public bool after = false;
#if UNITY_EDITOR
        private DynamicAction m_DragAction = null;
        private DynamicValue<bool> m_DropAccept = null;
        private DynamicAction m_DropAction = null;

        private static int s_HashCode = "StackableDecorator.OnDragDropAttribute".GetHashCode();
#endif
        public OnDragDropAttribute()
        {
#if UNITY_EDITOR
#endif
        }
#if UNITY_EDITOR
        private void Update()
        {
            if (m_DragAction == null)
                m_DragAction = new DynamicAction(drag, m_SerializedProperty);
            if (m_DropAccept == null)
                m_DropAccept = new DynamicValue<bool>(accept, m_SerializedProperty);
            if (m_DropAction == null)
                m_DropAction = new DynamicAction(drop, m_SerializedProperty);
            m_DragAction.Update(m_SerializedProperty);
            m_DropAccept.Update(m_SerializedProperty);
            m_DropAction.Update(m_SerializedProperty);
        }

        private void HandleEvent(Rect position)
        {
            var id = GUIUtility.GetControlID(s_HashCode, FocusType.Passive, position);
            var evt = Event.current;
            switch (evt.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (position.Contains(evt.mousePosition) && evt.button == button)
                        GUIUtility.hotControl = id;
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id && evt.button == button)
                        GUIUtility.hotControl = 0;
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id && position.Contains(evt.mousePosition) && evt.button == button)
                    {
                        if (autoDrag && m_SerializedProperty.propertyType == SerializedPropertyType.ObjectReference && m_SerializedProperty.objectReferenceValue != null)
                        {
                            DragAndDrop.PrepareStartDrag();
                            DragAndDrop.objectReferences = new[] { m_SerializedProperty.objectReferenceValue };
                            if (EditorUtility.IsPersistent(m_SerializedProperty.objectReferenceValue))
                            {
                                var path = AssetDatabase.GetAssetPath(m_SerializedProperty.objectReferenceValue);
                                DragAndDrop.paths = new[] { path };
                            }
                            DragAndDrop.StartDrag(m_SerializedProperty.displayName);
                        }
                        else
                            m_DragAction.DoAction();
                        if (use) evt.Use();
                    }
                    break;
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (position.Contains(evt.mousePosition) && evt.button == button)
                    {
                        if (autoDrop && m_SerializedProperty.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            foreach (var obj in DragAndDrop.objectReferences)
                                if (m_FieldInfo.FieldType.IsAssignableFrom(obj.GetType()))
                                {
                                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                    if (evt.type == EventType.DragPerform)
                                    {
                                        DragAndDrop.AcceptDrag();
                                        m_SerializedProperty.objectReferenceValue = obj;
                                    }
                                    break;
                                }
                        }
                        else
                        {
                            if (m_DropAccept.GetValue())
                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            if (evt.type == EventType.DragPerform)
                            {
                                DragAndDrop.AcceptDrag();
                                m_DropAction.DoAction();
                            }
                        }
                        if (use) evt.Use();
                    }
                    break;
            }
        }

        public override float GetHeight(SerializedProperty property, GUIContent label, float height)
        {
            return height;
        }

        public override bool BeforeGUI(ref Rect position, ref SerializedProperty property, ref GUIContent label, ref bool includeChildren, bool visible)
        {
            if (!IsVisible()) return visible;
            if (!visible) return false;

            if (!after)
            {
                Update();
                HandleEvent(position);
            }

            return true;
        }

        public override void AfterGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsVisible()) return;

            if (after)
            {
                Update();
                HandleEvent(position);
            }
        }
#endif
    }
}