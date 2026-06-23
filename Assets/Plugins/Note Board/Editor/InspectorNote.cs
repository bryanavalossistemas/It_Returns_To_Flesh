namespace Tornadoally.NoteBoard
{
    using System; using System.Collections.Generic; using System.Linq;
    using UnityEditor; using UnityEditor.SceneManagement; using UnityEngine;
    using Object = UnityEngine.Object;
    [InitializeOnLoad] public static class InspectorNote
    {
        static InspectorNote() { Editor.finishedDefaultHeaderGUI += DrawNotes; }
        private static Object lastSelection;
        private static string currentOwnerId; private static string currentOwnerName;
        private static readonly Dictionary<string, string> pendingEdits = new(); private static NoteRecord pendingNewNote;
        private static bool flagChange; private static GUIStyle noteLabel;
        private static void DrawNotes(Editor editor)
        {
            Object active = Selection.activeObject;
            if (active == null) return;
            if (editor.target == active || active is Texture2D)
            {
                EditorGUILayout.Space(4);
                if (AssetDatabase.Contains(editor.target)) DrawForAsset(editor.target);
                else DrawForSceneObject(editor.target);
            }
        }
        private static void DrawForAsset(Object asset)
        {
            ResetSelectionIfNeeded();
            currentOwnerId = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset)); currentOwnerName = asset.name;
            var records = NoteBoardOptions.Instance.GetRecordsForOwner(currentOwnerId);
            SyncNames(records, currentOwnerName);
            DrawAllNotes(records);
        }
        private static void DrawForSceneObject(Object obj)
        {
            ResetSelectionIfNeeded();
            GameObject go = obj as GameObject ?? (obj as Component)?.gameObject; if (go == null) return;
            var prefabStage = PrefabStageUtility.GetPrefabStage(go);
            if (prefabStage != null) { currentOwnerId = AssetDatabase.AssetPathToGUID(prefabStage.assetPath); }
            else
            {
                var prefabType = PrefabUtility.GetPrefabAssetType(go); var prefabStatus = PrefabUtility.GetPrefabInstanceStatus(go);
                if (prefabType != PrefabAssetType.NotAPrefab && prefabStatus != PrefabInstanceStatus.NotAPrefab)
                {
                    GameObject prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(go);
                    currentOwnerId = prefabAsset != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(prefabAsset)) : GlobalObjectId.GetGlobalObjectIdSlow(go).ToString();
                }
                else { currentOwnerId = GlobalObjectId.GetGlobalObjectIdSlow(go).ToString(); }
            }
            currentOwnerName = go.name;
            var records = NoteBoardOptions.Instance.GetRecordsForOwner(currentOwnerId);
            SyncNames(records, currentOwnerName);
            if (records.Count > 0) EditorGUIUtility.SetIconForObject(go, GetIconForData(records.Where(r => r.data.statusIndex == records.GroupBy(r => r.data.statusIndex).OrderByDescending(g => g.Count()).ThenBy(g => g.Min(r => r.order)).First().Key).OrderBy(r => r.order).First().data));
            DrawAllNotes(records);
        }
        private static void DrawAllNotes(List<NoteRecord> savedRecords)
        {
            foreach (var record in savedRecords.OrderBy(r => r.order)) { DrawCollapsibleNote(record, isPending: false); EditorGUILayout.Space(2); }
            if (pendingNewNote != null && pendingNewNote.id == currentOwnerId)
            { DrawCollapsibleNote(pendingNewNote, isPending: true); EditorGUILayout.Space(2); }
            else if (GUILayout.Button("＋ Add Note", GUILayout.Height(22))) AddPendingNote(savedRecords.Count);
        }
        private static void DrawCollapsibleNote(NoteRecord record, bool isPending)
        {
            if (noteLabel == null) { noteLabel = new GUIStyle(EditorStyles.boldLabel) { font = Resources.Load<Font>("Consolas"), fontSize = 20, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter }; }
            string tag = record.noteTag; string draft = pendingEdits.TryGetValue(tag, out var d) ? d : (record.data.notes ?? string.Empty); bool textUnsaved = isPending || draft != (record.data.notes ?? string.Empty);
            string headerTitle = textUnsaved ? "Note*" : "Note"; bool deleted = false;
            Color old = GUI.backgroundColor; GUI.color = GUI.backgroundColor = GetStatusColor(record.data);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox); GUILayout.BeginHorizontal();
            var arrowStyle = new GUIStyle(EditorStyles.label) { fontSize = 11 };
            if (GUILayout.Button(record.isExpanded ? "▼" : "▶", arrowStyle, GUILayout.Width(16), GUILayout.Height(16)))
            {
                record.isExpanded = !record.isExpanded; NoteBoardOptions.Instance.SaveRecord(record);
            }
            GUI.color = Color.white;
            GUILayout.Label(headerTitle, noteLabel);
            if (!record.isExpanded) GUILayout.FlexibleSpace();
            if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), EditorStyles.iconButton, GUILayout.Width(18), GUILayout.Height(18))) deleted = true;
            GUILayout.EndHorizontal();
            if (!deleted && record.isExpanded) { DrawNoteBody(record, tag, isPending); }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = old;
            if (deleted)
            {
                AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(record.id));
                if (isPending) pendingNewNote = null;
                else NoteBoardOptions.Instance.RemoveRecord(record);
                pendingEdits.Remove(tag);
                MarkBrowserDirty();
            }
        }
        private static void DrawNoteBody(NoteRecord record, string tag, bool isPending)
        {
            int prevStatus = record.data.statusIndex;
            EditorGUI.BeginChangeCheck(); DrawStatusButtons(ref record.data);
            if (EditorGUI.EndChangeCheck()) { HandleOrdering(record, prevStatus); PersistRecord(record, isPending); }
            flagChange = false;
            GUILayout.BeginVertical(EditorStyles.helpBox); DrawFlagButtons(ref record.data); GUILayout.EndVertical();
            if (record.data.statusIndex == -1 && record.data.flagMask != 0) DrawOutline(GUILayoutUtility.GetLastRect(), NoteColors.Flagged);
            if (flagChange) PersistRecord(record, isPending);
            if (!pendingEdits.ContainsKey(tag)) pendingEdits[tag] = record.data.notes ?? string.Empty;
            GUILayout.BeginVertical(EditorStyles.helpBox);
            string draft = pendingEdits[tag];
            EditorGUILayout.LabelField(record.data.notes != draft ? "✍️Note *" : "✍️Note", string.IsNullOrEmpty(record.data.notes) ? EditorStyles.label : EditorStyles.boldLabel);
            string newDraft = EditorGUILayout.TextArea(draft, GUILayout.MinHeight(50));
            if (newDraft != draft) pendingEdits[tag] = newDraft;
            bool textDirty = pendingEdits[tag] != (record.data.notes ?? string.Empty);
            if (textDirty && GUILayout.Button("Save Note")) { record.data.notes = pendingEdits[tag]; PersistRecord(record, isPending); }
            GUILayout.EndVertical();
            if (record.data.statusIndex == -1 && record.data.flagMask == 0 && !string.IsNullOrEmpty(record.data.notes)) DrawOutline(GUILayoutUtility.GetLastRect(), NoteColors.Note);
        }
        private static void PersistRecord(NoteRecord record, bool isPending)
        {
            NoteBoardOptions.Instance.SaveRecord(record);
            if (isPending) pendingNewNote = null;
            MarkBrowserDirty();
        }
        private static void AddPendingNote(int nextOrder) { pendingNewNote = new NoteRecord { id = currentOwnerId, noteTag = Guid.NewGuid().ToString("N"), name = currentOwnerName, data = new NoteData(),  order = nextOrder, isExpanded = true }; }
        private static void ResetSelectionIfNeeded()
        {
            if (Selection.activeObject == lastSelection) return;
            pendingNewNote = null; pendingEdits.Clear(); lastSelection = Selection.activeObject;
        }
        private static void SyncNames(List<NoteRecord> records, string name)
        {
            bool any = false;
            foreach (var r in records.Where(r => r.name != name)) { r.name = name; NoteBoardOptions.Instance.SaveRecord(r, flush: false); any = true; }
            if (any) { NoteBoardOptions.Instance.Flush(); MarkBrowserDirty(); }
        }
        private static Color GetStatusColor(NoteData data)
        {
            var defs = NoteBoardOptions.Instance.columns;
            if (data.statusIndex >= 0 && data.statusIndex < defs.Count) return defs[data.statusIndex].color;
            return data.flagMask != 0 ? NoteColors.Flagged : string.IsNullOrEmpty(data.notes) ? NoteColors.Empty : NoteColors.Note;
        }
        private static Texture2D GetIconForData(NoteData data)
        {
            if (data.statusIndex >= 0) return EditorGUIUtility.IconContent($"sv_label_{GetClosestIndex(NoteBoardOptions.Instance.columns[data.statusIndex].color)}").image as Texture2D;
            if (data.flagMask != 0) return EditorGUIUtility.IconContent("sv_label_7").image as Texture2D;
            if (!string.IsNullOrEmpty(data.notes)) return EditorGUIUtility.IconContent("sv_label_0").image as Texture2D;
            return null;
            int GetClosestIndex(Color input)
            {
                int closestIndex = 0;
                float minDistance = float.MaxValue;
                for (int i = 0; i < labelColors.Length; i++)
                {
                    float r = input.r - labelColors[i].r;
                    float g = input.g - labelColors[i].g;
                    float bDiff = input.b - labelColors[i].b;
                    float dist = r * r + g * g + bDiff * bDiff;
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closestIndex = i;
                    }
                }
                return closestIndex;
            }
        }
        static readonly Color[] labelColors = { Color.gray1, Color.blue, new Color(0f, 0.5f, 0.5f), Color.green, Color.yellow, new Color(1f, 0.5f, 0f), Color.red, Color.magenta };
        private static void HandleOrdering(NoteRecord record, int prevStatus)
        {
            if (record.data.statusIndex == prevStatus && !record.data.IsEmpty()) return;
            NormalizeStatus(prevStatus, record);
            if (record.data.statusIndex == -1)
            {
                var none = NoteBoardOptions.Instance.records.Where(r => r.data.statusIndex == -1 && r != record).ToList();
                int flagged = none.Count(r => r.data.flagMask != 0); int notes = none.Count(r => r.data.flagMask == 0);
                record.order = record.data.flagMask != 0 ? flagged : flagged + notes;
            }
            else { record.order = NoteBoardOptions.Instance.records.Count(r => r.data.statusIndex == record.data.statusIndex && r != record); }
        }
        private static void MarkBrowserDirty() { if (EditorWindow.HasOpenInstances<NoteBoardWindow>()) EditorWindow.GetWindow<NoteBoardWindow>(false, null, false).MarkDirty(); }
        private static void NormalizeStatus(int statusIndex, NoteRecord exclude)
        {
            var list = NoteBoardOptions.Instance.records.Where(r => r.data.statusIndex == statusIndex && r != exclude).OrderBy(r => r.order).ToList();
            if (statusIndex == -1) list = list.Where(r => r.data.flagMask != 0).Concat(list.Where(r => r.data.flagMask == 0)).ToList();
            for (int i = 0; i < list.Count; i++) { list[i].order = i; NoteBoardOptions.Instance.SaveRecord(list[i], flush: false); }
            if (list.Count > 0) NoteBoardOptions.Instance.Flush();
        }
        private static void DrawOutline(Rect r, Color color)
        {
            float size = 2f; Color c = new Color(color.r * 1.5f, color.g * 1.5f, color.b * 1.5f);
            EditorGUI.DrawRect(new Rect(r.x, r.y, r.width, size), c); EditorGUI.DrawRect(new Rect(r.x, r.yMax - size, r.width, size), c); EditorGUI.DrawRect(new Rect(r.x, r.y, size, r.height), c); EditorGUI.DrawRect(new Rect(r.xMax - size, r.y, size, r.height), c);
        }
        private static void DrawStatusButtons(ref NoteData data)
        {
            var defs = NoteBoardOptions.Instance.columns;
            EditorGUILayout.BeginHorizontal();
            Color old = GUI.backgroundColor;
            var style = new GUIStyle(GUI.skin.button);
            for (int i = 0; i < defs.Count; i++) DrawStatusButton(ref data, i, defs[i].DisplayLabel, defs[i].color, 2f, style);
            GUI.backgroundColor = old;
            EditorGUILayout.EndHorizontal();
        }
        private static void DrawStatusButton(ref NoteData data, int statusIndex, string label, Color color, float activeMult, GUIStyle style)
        {
            bool active = data.statusIndex == statusIndex;
            GUI.backgroundColor = active ? color * activeMult : color;
            style.fontStyle = active ? FontStyle.Bold : FontStyle.Normal;
            Rect r = GUILayoutUtility.GetRect(new GUIContent(label), style);
            if (GUI.Button(r, label, style)) data.statusIndex = active ? -1 : statusIndex;
            if (active) DrawOutline(r, color);
        }
        private static void DrawFlagButtons(ref NoteData data)
        {
            var flagDefs = NoteBoardOptions.Instance.flags;
            EditorGUILayout.LabelField("🚩Flags", data.flagMask == 0 ? EditorStyles.label : EditorStyles.boldLabel);
            if (flagDefs.Count == 0) return;
            float spacing = 6f; float height = EditorGUIUtility.singleLineHeight + 5f;
            float minWidth = 120f; float viewWidth = EditorGUIUtility.currentViewWidth - 40f; int perRow = Mathf.Max(1, Mathf.FloorToInt((viewWidth + spacing) / (minWidth + spacing)));
            int rows = Mathf.CeilToInt((float)flagDefs.Count / perRow); float totalH = rows * height + (rows - 1) * spacing + spacing;
            Rect rect = GUILayoutUtility.GetRect(0f, totalH, GUILayout.ExpandWidth(true));
            DrawFlagPillRows(rect, ref data.flagMask, flagDefs, perRow, spacing, height);
        }
        private static void DrawFlagPillRows(Rect rect, ref int flagMask, List<NoteFlagDef> flagDefs, int perRow, float spacing, float height)
        {
            Handles.BeginGUI(); int index = 0, row = 0;
            while (index < flagDefs.Count)
            {
                int count = Mathf.Min(perRow, flagDefs.Count - index); float pillW = (rect.width - spacing * (count + 1)) / count;
                for (int col = 0; col < count; col++) { FlagToggle(index, flagDefs[index], rect.x + spacing + col * (pillW + spacing), rect.y + row * (height + spacing), pillW, height, ref flagMask); index++; }
                row++;
            } Handles.EndGUI();
        }
        private static void FlagToggle(int flagIndex, NoteFlagDef def, float x, float y, float width, float height, ref int flagMask)
        {
            int bit = 1 << flagIndex;
            Rect pill = new Rect(x, y, width, height); Rect shadow = new Rect(x - 2f, y - 2f, width + 4f, height + 4f);
            Color color = def.color;
            bool hovered = pill.Contains(Event.current.mousePosition); bool selected = (flagMask & bit) != 0; float b = selected ? 0.8f : 0.3f;
            if (hovered) b = selected ? 1f : 0.5f;
            color = new Color(color.r * b, color.g * b, color.b * b);
            float sm = selected ? 1.5f : 0.4f; Color shade = new Color(color.r * sm, color.g * sm, color.b * sm);
            DrawPill(shadow, shade, new Vector2(0, 1)); DrawPill(pill, color);
            var centered = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white }, hover = { textColor = Color.white }, fontStyle = selected ? FontStyle.Bold : FontStyle.Normal };
            GUI.Label(pill, def.DisplayLabel, centered);
            if (Event.current.type == EventType.MouseDown && hovered)
            {
                flagMask ^= bit; flagChange = true;
                Event.current.Use();
            }
        }
        private static void DrawPill(Rect r, Color col, Vector2 offset = default)
        {
            float radius = r.height * 0.5f; r.x += offset.x; r.y += offset.y;
            EditorGUI.DrawRect(new Rect(r.x + radius - 1f, r.y, r.width - radius * 2 + 2f, r.height), col);
            Handles.color = col;
            Handles.DrawSolidArc(new Vector3(r.x + radius, r.y + radius, 0), Vector3.forward, Vector3.up, 180f, radius); Handles.DrawSolidArc(new Vector3(r.xMax - radius, r.y + radius, 0), Vector3.forward, Vector3.up, -180f, radius);
        }
    }
}