namespace Tornadoally.NoteBoard
{
    using System.Collections.Generic;
    using UnityEditor; using UnityEngine;
    [CustomEditor(typeof(NoteBoardOptions))] public class NoteBoardEditor : Editor
    {
        private SerializedProperty _columns; private SerializedProperty _flags;
        private bool _columnsFoldout = true; private bool _flagsFoldout = true;
        private Vector2 _columnsScroll; private Vector2 _flagsScroll; private const float MaxSectionHeight = 210f;
        private void OnEnable() { _columns = serializedObject.FindProperty("columns"); _flags = serializedObject.FindProperty("flags"); }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUILayout.Space(-4); GUILayout.BeginHorizontal(); GUILayout.Space(-18);
            DrawSection("📝 Columns", _columns, new Color(32f / 255f, 32f / 255f, 32f / 255f), ref _columnsFoldout, ref _columnsScroll);
            GUILayout.Space(-4); GUILayout.EndHorizontal(); GUILayout.Space(-2); GUILayout.BeginHorizontal(); GUILayout.Space(-18);
            DrawSection("🚩 Flags", _flags, new Color(47f / 255f, 27f / 255f, 47f / 255f), ref _flagsFoldout, ref _flagsScroll, true);
            GUILayout.Space(-4); GUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
        private void DrawSection(string title, SerializedProperty list, Color headerColour, ref bool foldout, ref Vector2 scroll, bool isFlag = false)
        {
            GUIStyle outerBox = new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(0, 0, 0, 0) };
            GUI.backgroundColor = isFlag ? NoteColors.Flagged : NoteColors.Note;
            EditorGUILayout.BeginVertical(outerBox);
            Rect headerRect = GUILayoutUtility.GetRect(0, 28, GUILayout.ExpandWidth(true)); Rect foldoutRect = new Rect(headerRect.x + 20, headerRect.y + 5, headerRect.width - 70, 18);
            GUIStyle headerLabel = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold, fontSize = 16};
            GUI.backgroundColor = Color.white;
            foldout = EditorGUI.Foldout(foldoutRect, foldout, $"{title} ({list.arraySize})", true, headerLabel);
            if (foldout)
            {
                GUILayout.Space(-2);
                GUIStyle bodyBox = new GUIStyle { margin = new RectOffset(0, 0, 0, 0) };
                float contentHeight = CalculateSectionHeight(list); float clampedHeight = Mathf.Min(contentHeight, MaxSectionHeight);
                scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(clampedHeight)); EditorGUILayout.BeginVertical(bodyBox);
                for (int i = 0; i < list.arraySize; i++) { DrawElement(list, i, list.arraySize > 1); GUILayout.Space(-2); }
                EditorGUILayout.EndVertical(); EditorGUILayout.EndScrollView();
                GUI.backgroundColor = isFlag ? NoteColors.Flagged : NoteColors.Note;
                if (GUILayout.Button("+ Add", new GUIStyle(EditorStyles.miniButton) { fontStyle = FontStyle.Bold, fontSize = 14 })) list.InsertArrayElementAtIndex(list.arraySize);
                GUI.backgroundColor = Color.white;
            } EditorGUILayout.EndVertical();
        }
        private static void DrawElement(SerializedProperty list, int index, bool canDelete)
        {
            SerializedProperty element = list.GetArrayElementAtIndex(index); element.isExpanded = true;
            List<SerializedProperty> children = new List<SerializedProperty>();
            SerializedProperty prop = element.Copy(); SerializedProperty end = prop.GetEndProperty(); bool enterChildren = true;
            while (prop.NextVisible(enterChildren) && !SerializedProperty.EqualContents(prop, end)) { children.Add(prop.Copy()); enterChildren = false; }
            float rowHeight = EditorGUIUtility.singleLineHeight; float spacing = 4; float buttonWidth = 14; float totalHeight = rowHeight + 8;
            Rect bgRect = EditorGUILayout.GetControlRect(false, totalHeight); bgRect.xMin -= 2; bgRect.xMax += 2;
            SerializedProperty col = element.FindPropertyRelative("color");
            Color? bgColor = col != null ? col.colorValue : null;
            if (bgColor.HasValue) EditorGUI.DrawRect(bgRect, bgColor.Value);
            Rect contentRect = new Rect(bgRect.x + 6, bgRect.y + 4, bgRect.width - 12, rowHeight);
            int count = children.Count;
            float totalSpacing = spacing * (count - 1); float usableWidth = contentRect.width - buttonWidth - spacing; float fieldWidth = (usableWidth - totalSpacing) / count; float x = contentRect.x;
            for (int i = 0; i < count; i++)
            {
                Rect r = new Rect(x, contentRect.y, fieldWidth, rowHeight);
                if (children[i].propertyType == SerializedPropertyType.Color && children[i].name == "color")
                { children[i].colorValue = EditorGUI.ColorField(r, GUIContent.none, children[i].colorValue, showEyedropper: true, showAlpha: false, hdr: false); }
                else EditorGUI.PropertyField(r, children[i], GUIContent.none);
                x += fieldWidth + spacing;
            }
            Rect buttonRect = new Rect(contentRect.xMax - buttonWidth, contentRect.y, buttonWidth, rowHeight);
            GUIStyle removeBtn = new GUIStyle(EditorStyles.miniButton) { alignment = TextAnchor.MiddleCenter };
            GUI.enabled = canDelete;
            if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("TreeEditor.Trash"), EditorStyles.iconButton))
            {
                list.DeleteArrayElementAtIndex(index);
                if (index < list.arraySize && list.GetArrayElementAtIndex(index).propertyType == SerializedPropertyType.ObjectReference && list.GetArrayElementAtIndex(index).objectReferenceValue == null)
                { list.DeleteArrayElementAtIndex(index); }
            } GUI.enabled = true;
        }
        private static float CalculateSectionHeight(SerializedProperty list)
        {
            if (list.arraySize == 0) return 32f;
            return list.arraySize * (EditorGUIUtility.singleLineHeight + 8f) + 2f;
        }
    }
}