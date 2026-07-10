namespace Tornadoally.NoteBoard
{
    using System; using System.Linq; using System.Collections.Generic;
    using UnityEditor; using UnityEngine;
    public class NoteBoardWindow : EditorWindow
    {
        private string searchQuery = ""; private int requiredFlags = 0;
        private List<CachedNote> cachedNotes = new(); private Texture2D stripeTex; private static GUIStyle categoryLabel; private Vector2 topScroll, bottomScroll;
        private double lastCacheTime; private const double cacheCooldown = 0.25; private bool needsRefresh = true;
        private CachedNote dragNote; private bool isDragging; private Vector2 dragRectSize; private Vector2 dragMouseOffset;
        private int hoverStatus = -1; private int hoverIndex = -1; private bool hoverDropBefore = true;
        private float bottomHeight = 300f; private bool isDraggingSplitter = false; private const float collapsedSize = 105f;
        bool maxHeightFound = false; float topMaxCol = -1f; bool maxHeight2Found = false; float topMaxCol2 = -1f;
        [MenuItem("Window/Note Board")] [MenuItem("Assets/Note Board")] [MenuItem("GameObject/Note Board")]
        public static void Open() { GetWindow<NoteBoardWindow>("Note Board").titleContent = new GUIContent("Note Board", Resources.Load<Texture>("NoteBoardIcon")); }
        private void OnEnable()
        {
            EditorApplication.projectChanged += MarkDirty; EditorApplication.hierarchyChanged += MarkDirty; Undo.undoRedoPerformed += MarkDirty;
            stripeTex = Resources.Load<Texture2D>("StripePattern"); needsRefresh = true;
        }
        private void OnDisable() { EditorApplication.projectChanged -= MarkDirty; EditorApplication.hierarchyChanged -= MarkDirty; Undo.undoRedoPerformed -= MarkDirty; }
        public void MarkDirty() { needsRefresh = true; Repaint(); }
        private void OnGUI()
        {
            if (categoryLabel == null) { categoryLabel = new GUIStyle(EditorStyles.boldLabel) { font = Resources.Load<Font>("Consolas"), fontSize = 20, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter }; }
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), new Color(0.05f, 0.05f, 0.05f, 0.5f));
            DrawToolbar(); RefreshCacheIfNeeded();
            var statusDefs = NoteBoardOptions.Instance.columns; int[] statusCounts = new int[statusDefs.Count];
            foreach (var n in cachedNotes)
            {
                int si = n.record.data.statusIndex;
                if (si >= 0 && si < statusCounts.Length) statusCounts[si]++;
            }
            int totalWithStatus = statusCounts.Sum(); float progressBarHeight = 0f;
            if (totalWithStatus > 0) { progressBarHeight = 28f; DrawProgressBar(statusCounts, statusDefs, totalWithStatus); }
            float toolbarHeight = EditorStyles.toolbar.fixedHeight; float availableHeight = position.height - toolbarHeight - progressBarHeight;
            if (!isDraggingSplitter) { float top = position.height - toolbarHeight - progressBarHeight; bottomHeight = Mathf.Clamp(bottomHeight, collapsedSize, top - collapsedSize); }
            float topHeight = availableHeight - bottomHeight;
            Rect topRect = new Rect(0, toolbarHeight + progressBarHeight, position.width, topHeight); Rect botRect = new Rect(0, topRect.yMax, position.width, bottomHeight);
            GUI.backgroundColor = Color.white; GUILayout.BeginArea(topRect);
            topScroll = EditorGUILayout.BeginScrollView(topScroll, false, false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            float minWidth = NoteBoardOptions.Instance.columns.Count * 135f + 3f;
            DrawResultsTop(position.width < minWidth ? topHeight - 18f : topHeight - 4f); EditorGUILayout.EndScrollView(); GUILayout.EndArea();
            GUI.backgroundColor = Color.white; GUILayout.BeginArea(botRect);
            bottomScroll = EditorGUILayout.BeginScrollView(bottomScroll, false, false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            DrawResultsBottom(); EditorGUILayout.EndScrollView(); GUILayout.EndArea();
            HandleSplitter(topRect.yMax, progressBarHeight);
            if (isDragging) DrawGhost();
            HandleDragRelease();
        }
        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar); EditorGUI.BeginChangeCheck();
            searchQuery = GUILayout.TextField(searchQuery, EditorStyles.toolbarSearchField, GUILayout.ExpandWidth(true));
            var flagDefs = NoteBoardOptions.Instance.flags; string[] flagNames = flagDefs.Select(f => f.DisplayLabel).ToArray();
            int raw = EditorGUILayout.MaskField(requiredFlags, flagNames, EditorStyles.toolbarPopup, GUILayout.Width(220));
            int validBits = flagDefs.Count > 0 ? (1 << flagDefs.Count) - 1 : 0;
            requiredFlags = raw < 0 ? validBits : (raw & validBits);
            if (GUILayout.Button("↻", EditorStyles.toolbarButton, GUILayout.Width(20))) needsRefresh = true;
            if (EditorGUI.EndChangeCheck()) Repaint();
            EditorGUILayout.EndHorizontal();
        }
        private void DrawProgressBar(int[] counts, List<NoteStatusDef> defs, int total)
        {
            float barH = 24f; float top = EditorStyles.toolbar.fixedHeight + 1f; float width = position.width - 10f; Rect full = new Rect(5f, top, width, barH);
            float[] suffixW = new float[defs.Count]; int tail = 0;
            for (int i = defs.Count - 1; i >= 0; i--) { tail += counts[i]; suffixW[i] = width * tail / total; }
            for (int i = 0; i < defs.Count; i++)
            {
                if (suffixW[i] <= 0f) continue;
                Color c = defs[i].color; Rect pr = new Rect(full.xMax - suffixW[i], full.y, suffixW[i], barH);
                DrawPill(new Rect(pr.x - 2f, pr.y - 2f, pr.width + 4f, pr.height + 4f), new Color(c.r * 0.25f, c.g * 0.25f, c.b * 0.25f), offset: new Vector2(0, 1)); DrawPillWithPattern(pr, new Color(c.r * 0.5f, c.g * 0.5f, c.b * 0.5f), true);
            }
            var centered = new GUIStyle(categoryLabel) { alignment = TextAnchor.MiddleCenter, fontSize = 20 };
            for (int i = defs.Count - 1; i >= 0; i--)
            {
                float segW = width * counts[i] / total; float xOff = i < defs.Count - 1 ? suffixW[i + 1] : 0f;
                DrawBarLabel(full, segW, $"{defs[i].name} {(float)counts[i] / total * 100f:0}%", defs[i].color, centered, xOff);
            }
            void DrawBarLabel(Rect bar, float seg, string text, Color tint, GUIStyle style, float xOff = 0f)
            {
                if (seg <= 0f) return;
                style.fontSize = seg < 75f ? 8 : seg < 150f ? 12 : seg < 300f ? 15 : 20;
                Rect lr = new Rect(bar.xMax - xOff - seg, bar.y, seg, bar.height); float rw = style.CalcSize(new GUIContent(text)).x + 4f;
                if (seg >= rw) DrawPill(new Rect(lr.x + (lr.width - rw) / 2f, lr.y + (lr.height - style.fontSize) / 2f, rw, style.fontSize), new Color(tint.r * 0.5f, tint.g * 0.5f, tint.b * 0.5f));
                GUI.Label(lr, text, style);
            }
            void DrawPillWithPattern(Rect r, Color tint, bool darken = false)
            {
                DrawPill(r, tint);
                if (stripeTex != null && r.width > 0f)
                {
                    Color prev = GUI.color;
                    GUI.color = darken ? new Color(tint.r * 0.5f, tint.g * 0.5f, tint.b * 0.5f) : tint * 1.5f; Rect sr = new Rect(r.x + 12f, r.y, r.width - 24f, r.height);
                    if (sr.width > 0f) { GUI.DrawTextureWithTexCoords(sr, stripeTex, new Rect(0f, 0f, (int)(r.width / stripeTex.width), 1f)); }
                    GUI.color = prev;
                }
            }
        }
        private void DrawResultsTop(float topHeight)
        {
            var statusDefs = NoteBoardOptions.Instance.columns; int colCount = statusDefs.Count; var statusLists = new List<List<CachedNote>>(colCount);
            for (int i = 0; i < colCount; i++) statusLists.Add(new List<CachedNote>());
            foreach (var n in cachedNotes)
            {
                if (!MatchesFilter(n.record.data) || !MatchesSearch(n.record.name, n.record.data)) continue;
                int si = n.record.data.statusIndex;
                if (si >= 0 && si < colCount) statusLists[si].Add(n);
            }
            for (int i = 0; i < colCount; i++) statusLists[i] = SortByOrder(statusLists[i]);
            GUILayout.BeginVertical(GUILayout.ExpandHeight(true)); GUILayout.BeginHorizontal();
            maxHeightFound = false;
            for (int i = 0; i < colCount; i++) DrawColumn(statusDefs[i].DisplayLabel, statusLists[i], i, topHeight);
            if (!maxHeightFound) topMaxCol = -1f;
            GUILayout.EndHorizontal(); GUILayout.EndVertical();
        }
        private void DrawResultsBottom()
        {
            var noneStatus = cachedNotes.Where(n => (n.record.data.statusIndex == -1 || n.record.data.statusIndex >= NoteBoardOptions.Instance.columns.Count) && MatchesFilter(n.record.data) && MatchesSearch(n.record.name, n.record.data)).OrderBy(n => n.record.order).ToList();
            var flagOnly = noneStatus.Where(n => n.record.data.flagMask != 0).ToList(); var noteOnly = noneStatus.Where(n => n.record.data.flagMask == 0).ToList();
            float actualH = position.width < 273f ? bottomHeight - 18f : bottomHeight - 4f; float scrollOffset = topMaxCol2 > 0f ? 11f : 5f;
            maxHeight2Found = false;
            GUILayout.BeginVertical(GUILayout.Height(actualH)); GUILayout.BeginHorizontal();
            GUI.color = GUI.backgroundColor = NoteColors.Flagged;
            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(Mathf.Max(132f, position.width / 2f - scrollOffset)), GUILayout.Height(actualH));
            GUI.color = Color.white;
            Rect flagHdr = GUILayoutUtility.GetRect(0, 2);
            if (isDragging && hoverStatus == -1 && dragNote.record.data.flagMask != 0) EditorGUI.DrawRect(flagHdr, new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b, 0.5f));
            GUILayout.Label("🚩Flags", categoryLabel);
            GUILayout.Space(4f);
            for (int i = 0; i < flagOnly.Count; i++) { DrawNote(flagOnly[i], -1, i); GUILayout.Space(2f); }
            Rect flagEnd = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (isDragging && dragNote.record.data.flagMask != 0 && flagEnd.Contains(Event.current.mousePosition))
            {
                hoverStatus = -1; hoverIndex = flagOnly.Count; hoverDropBefore = true;
                EditorGUI.DrawRect(new Rect(flagEnd.x, flagEnd.y - 1f, flagEnd.width, 1), new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b, 1f));
            } GUILayout.EndVertical();
            TrackColumnHeight(ref topMaxCol2, ref maxHeight2Found, bottomHeight - 4f);
            GUI.color = GUI.backgroundColor = NoteColors.Note;
            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(Mathf.Max(132f, position.width / 2f - scrollOffset)), GUILayout.Height(Mathf.Max(actualH, topMaxCol2)));
            GUI.color = Color.white;
            Rect noteHdr = GUILayoutUtility.GetRect(0, 2);
            if (isDragging && hoverStatus == -1 && dragNote.record.data.flagMask == 0) EditorGUI.DrawRect(noteHdr, new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b, 0.5f));
            GUILayout.Label("✍️Notes", categoryLabel);
            GUILayout.Space(4f);
            for (int i = 0; i < noteOnly.Count; i++)
            {
                DrawNote(noteOnly[i], -1, flagOnly.Count + i, isNote: true);
                GUILayout.Space(2f);
            }
            Rect noteEnd = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (isDragging && dragNote.record.data.flagMask == 0 && !string.IsNullOrWhiteSpace(dragNote.record.data.notes) && noteEnd.Contains(Event.current.mousePosition))
            {
                hoverStatus = -1; hoverIndex = noneStatus.Count; hoverDropBefore = true;
                EditorGUI.DrawRect(new Rect(noteEnd.x, noteEnd.y - 1f, noteEnd.width, 1), new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b, 1f));
            } GUILayout.EndVertical();
            TrackColumnHeight(ref topMaxCol2, ref maxHeight2Found, bottomHeight - 4f);
            if (!maxHeight2Found) topMaxCol2 = -1f;
            GUILayout.EndHorizontal(); GUILayout.EndVertical();
        }
        private void TrackColumnHeight(ref float maxCol, ref bool found, float threshold)
        {
            float lastY = GUILayoutUtility.GetLastRect().yMax - 2f;
            if (lastY > threshold && lastY > maxCol) { found = true; maxCol = lastY; Repaint(); }
            else if (lastY == maxCol) found = true;
        }
        private void HandleSplitter(float yPos, float progressBar)
        {
            Rect splitter = new Rect(0, yPos - 4f, position.width, 8f); EditorGUIUtility.AddCursorRect(splitter, MouseCursor.ResizeVertical);
            if (Event.current.type == EventType.MouseDown && splitter.Contains(Event.current.mousePosition)) { isDraggingSplitter = true; Event.current.Use(); }
            if (Event.current.type == EventType.MouseDrag && isDraggingSplitter)
            {
                float top = position.height - EditorStyles.toolbar.fixedHeight - progressBar;
                bottomHeight = Mathf.Clamp(position.height - Event.current.mousePosition.y, collapsedSize, top - collapsedSize);
                Event.current.Use(); Repaint();
            }
            if (Event.current.type == EventType.MouseUp) isDraggingSplitter = false;
        }
        private void DrawColumn(string title, List<CachedNote> entries, int statusIndex, float topHeight)
        {
            var statusDefs = NoteBoardOptions.Instance.columns;
            Color col = statusIndex >= 0 && statusIndex < statusDefs.Count ? statusDefs[statusIndex].color : Color.white;
            GUI.color = GUI.backgroundColor = col;
            int colCount = Mathf.Max(1, statusDefs.Count); float scrollOff = topMaxCol > 0f ? 8.5f : 4f;
            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(Mathf.Max(132f, position.width / colCount - scrollOff)), GUILayout.Height(Mathf.Max(topHeight, topMaxCol)));
            GUI.color = Color.white;
            Rect hdr = GUILayoutUtility.GetRect(0, 2);
            if (isDragging && hoverStatus == statusIndex) EditorGUI.DrawRect(hdr, new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b, 0.5f));
            GUILayout.Label(title, categoryLabel);
            GUILayout.Space(4f);
            for (int i = 0; i < entries.Count; i++) { DrawNote(entries[i], statusIndex, i); GUILayout.Space(2f); }
            Rect end = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (isDragging && end.Contains(Event.current.mousePosition))
            {
                hoverStatus = statusIndex; hoverIndex = entries.Count; hoverDropBefore = true;
                EditorGUI.DrawRect(new Rect(end.x, end.y - 1f, end.width, 1), new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b, 1f));
            }
            GUILayout.EndVertical();
            float lastY = GUILayoutUtility.GetLastRect().yMax - 2f;
            if (lastY > topHeight && lastY > topMaxCol) { maxHeightFound = true; topMaxCol = lastY; Repaint(); }
            else if (lastY == topMaxCol) maxHeightFound = true;
        }
        private Rect DrawNote(CachedNote entry, int columnStatus, int index, bool isNote = false)
        {
            const float pad = 10f; const float titleH = 18f; const float flagsH = 17f; const float vSpace = 4f;
            bool hasFlags = entry.record.data.flagMask != 0; bool hasNotes = !string.IsNullOrEmpty(entry.record.data.notes);
            float topOff = topMaxCol > 0f ? 36.5f : 32f; float botOff = topMaxCol2 > 0f ? 39f : 33f;
            int colCount = Mathf.Max(1, NoteBoardOptions.Instance.columns.Count);
            float estW = Mathf.Max(104f, columnStatus == -1 ? position.width / 2f - botOff : position.width / colCount - topOff); float contentH = titleH;
            if (Event.current.type == EventType.Layout)
            {
                if (hasFlags && entry.record.isExpanded)
                {
                    int rows = CalculateFlagRows(new Rect(0, 0, Mathf.RoundToInt(estW), flagsH), entry.record.data.flagMask);
                    float flagBlk = rows * flagsH + Mathf.Max(0, rows - 1) * vSpace;
                    contentH += hasNotes ? flagBlk : flagBlk + vSpace;
                }
                if (hasNotes && entry.record.isExpanded) contentH += EditorStyles.wordWrappedMiniLabel.CalcHeight(new GUIContent(entry.record.data.notes), Mathf.RoundToInt(estW));
            }
            Rect rect = GUILayoutUtility.GetRect(0, contentH + pad * 2f, GUILayout.ExpandWidth(true));
            Color bg = GUI.backgroundColor;
            if (rect.Contains(Event.current.mousePosition) && !isDragging) GUI.backgroundColor = bg * 1.5f;
            else if (isDragging && dragNote != null && dragNote.record == entry.record) GUI.backgroundColor = bg * 2f;
            GUI.color = bg;
            GUI.Box(rect, GUIContent.none, new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(10, 10, 10, 10), margin = new RectOffset(0, 0, 6, 6) });
            GUI.color = Color.white; GUI.backgroundColor = bg;
            if (dragNote != null && dragNote.record == entry.record)
            {
                Color oc = new Color(bg.r * 0.75f, bg.g * 0.75f, bg.b * 0.75f);
                EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1), oc); EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - 1, rect.width, 1), oc); EditorGUI.DrawRect(new Rect(rect.x, rect.y, 1, rect.height), oc); EditorGUI.DrawRect(new Rect(rect.xMax - 1, rect.y, 1, rect.height), oc);
            }
            Rect inner; float y;
            if (hasNotes || hasFlags)
            {
                inner = new Rect(rect.x + pad, rect.y + pad, rect.width - pad * 2f, rect.height - pad * 2f); y = inner.y;
                float headerOffX = 14; Rect foldoutRect = new Rect(inner.x, y + 1, 14, titleH);
                bool newExpanded = EditorGUI.Foldout(foldoutRect, entry.record.isExpanded, GUIContent.none, true);
                if (newExpanded != entry.record.isExpanded) { entry.record.isExpanded = newExpanded; NoteBoardOptions.Instance.SaveRecord(entry.record); }
                Rect nameRect = new Rect(inner.x + headerOffX + 22, y, inner.width - headerOffX - 22, titleH);
                EditorGUI.LabelField(nameRect, GetEllipsedText(entry.record.name, EditorStyles.boldLabel, nameRect.width - 16f), EditorStyles.boldLabel);
                if (entry.unityObject != null) GUI.DrawTexture(new Rect(inner.x + headerOffX, y, 18, 18), EditorGUIUtility.ObjectContent(entry.unityObject, entry.unityObject.GetType()).image, ScaleMode.ScaleToFit);
            }
            else
            {
                inner = new Rect(rect.x + pad, rect.y + pad, rect.width - pad * 2f, rect.height - pad * 2f); y = inner.y; Rect nameRect = new Rect(inner.x + 22, y, inner.width - 22, titleH);
                EditorGUI.LabelField(nameRect, GetEllipsedText(entry.record.name, EditorStyles.boldLabel, nameRect.width - 16f), EditorStyles.boldLabel);
                if (entry.unityObject != null) GUI.DrawTexture(new Rect(inner.x, y, 18, 18), EditorGUIUtility.ObjectContent(entry.unityObject, entry.unityObject.GetType()).image, ScaleMode.ScaleToFit);
            }
            y += titleH;
            if (hasFlags && entry.record.isExpanded)
            {
                y += vSpace; int rows = DrawFlagPills(new Rect(inner.x, y, inner.width, flagsH), entry.record.data.flagMask); y += flagsH * rows + vSpace * Mathf.Max(0f, rows - 1);
            }
            if (hasNotes && entry.record.isExpanded)
            {
                y += vSpace; EditorGUI.LabelField(new Rect(inner.x, y, inner.width, EditorStyles.wordWrappedMiniLabel.CalcHeight(new GUIContent(entry.record.data.notes), inner.width)), entry.record.data.notes, EditorStyles.wordWrappedMiniLabel);
            }
            if (!isDragging && GUI.Button(new Rect(rect.xMax - 16 - pad, rect.y + pad, 16, 16), EditorGUIUtility.IconContent("TreeEditor.Trash"), EditorStyles.iconButton))
            {
                cachedNotes.Remove(entry); NoteBoardOptions.Instance.RemoveRecord(entry.record);
                NormalizeList(cachedNotes.Where(n => n.record.data.statusIndex == entry.record.data.statusIndex).OrderBy(n => n.record.order).ToList(), entry.record.data.statusIndex);
            }
            if (Event.current.type == EventType.MouseUp && rect.Contains(Event.current.mousePosition) && !isDragging && dragNote != null && dragNote.record == entry.record)
            {
                PingAndSelect(entry); dragNote = null;
                Event.current.Use();
            }
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                dragNote = entry; dragRectSize = rect.size; dragMouseOffset = Event.current.mousePosition - rect.position;
                Event.current.Use();
            }
            if (Event.current.type == EventType.MouseDrag && dragNote != null && dragNote.record == entry.record)
            {
                isDragging = true;
                Event.current.Use();
            }
            if (isDragging && rect.Contains(Event.current.mousePosition))
            {
                if (columnStatus != -1 || (isNote && dragNote.record.data.flagMask == 0 && !string.IsNullOrWhiteSpace(dragNote.record.data.notes)) || (!isNote && dragNote.record.data.flagMask != 0))
                { hoverStatus = columnStatus; hoverIndex = index; hoverDropBefore = Event.current.mousePosition.y < rect.y + rect.height / 2f; }
            }
            if (isDragging && hoverStatus == columnStatus && hoverIndex == index)
            {
                if (columnStatus == -1 && ((isNote && dragNote.record.data.flagMask != 0) || (!isNote && dragNote.record.data.flagMask == 0))) return rect;
                float lineY = rect.y + (hoverDropBefore ? -1f : rect.height + 1f);
                EditorGUI.DrawRect(new Rect(rect.x, lineY, rect.width, 1), new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b, 1f));
            }
            return rect;
        }
        private void PingAndSelect(CachedNote entry)
        {
            if (entry.unityObject != null)
            {
                EditorGUIUtility.PingObject(entry.unityObject); Selection.activeObject = entry.unityObject;
                if (entry.unityObject is GameObject) { SceneView.lastActiveSceneView?.FrameSelected(); } return;
            }
            if (!GlobalObjectId.TryParse(entry.record.id, out var gid)) return;
            var obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(gid);
            if (obj == null)
            {
                string path = AssetDatabase.GUIDToAssetPath(gid.assetGUID.ToString());
                if (!string.IsNullOrEmpty(path)) { UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path); obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(gid); }
            }
            if (obj != null) { Selection.activeObject = obj; EditorGUIUtility.PingObject(obj); SceneView.lastActiveSceneView?.FrameSelected(); }
        }
        private void DrawGhost()
        {
            if (!isDragging || dragNote == null) return;
            const float pad = 10f; const float titleH = 18f; const float flagsH = 17f; const float vSpace = 4f;
            Rect r = new Rect(Event.current.mousePosition - dragMouseOffset, dragRectSize);
            var statusDefs = NoteBoardOptions.Instance.columns;
            Color bg = hoverStatus >= 0 && hoverStatus < statusDefs.Count ? statusDefs[hoverStatus].color : dragNote.record.data.flagMask == 0 ? NoteColors.Note : NoteColors.Flagged; Color oc = new Color(bg.r * 0.75f, bg.g * 0.75f, bg.b * 0.75f);
            EditorGUI.DrawRect(r, new Color(bg.r, bg.g, bg.b, 0.25f)); EditorGUI.DrawRect(new Rect(r.x, r.y, r.width, 1), oc); EditorGUI.DrawRect(new Rect(r.x, r.yMax - 1, r.width, 1), oc); EditorGUI.DrawRect(new Rect(r.x, r.y, 1, r.height), oc); EditorGUI.DrawRect(new Rect(r.xMax - 1, r.y, 1, r.height), oc);
            Rect inner = new Rect(r.x + pad, r.y + pad, r.width - pad * 2, r.height - pad * 2);
            float y = inner.y;
            if (dragNote.unityObject != null)
            {
                GUI.DrawTexture(new Rect(inner.x, y, 18, 18), EditorGUIUtility.ObjectContent(dragNote.unityObject, dragNote.unityObject.GetType()).image, ScaleMode.ScaleToFit);
                EditorGUI.LabelField(new Rect(inner.x + 22, y, inner.width - 22, titleH), dragNote.record.name, EditorStyles.boldLabel);
            }
            else EditorGUI.LabelField(new Rect(inner.x, y, inner.width, titleH), dragNote.record.name, EditorStyles.boldLabel);
            y += titleH;
            if (dragNote.record.data.flagMask != 0 && dragNote.record.isExpanded)
            { y += vSpace; int rows = DrawFlagPills(new Rect(inner.x, y, inner.width, flagsH), dragNote.record.data.flagMask); y += flagsH * rows + (rows > 1 ? vSpace * (rows - 1) : 0); }
            if (!string.IsNullOrEmpty(dragNote.record.data.notes) && dragNote.record.isExpanded) { y += vSpace; EditorGUI.LabelField(new Rect(inner.x, y, inner.width, EditorStyles.wordWrappedMiniLabel.CalcHeight(new GUIContent(dragNote.record.data.notes), inner.width)), dragNote.record.data.notes, EditorStyles.wordWrappedMiniLabel); }
        }
        private int DrawFlagPills(Rect rect, int flagMask)
        {
            var flagDefs = NoteBoardOptions.Instance.flags;
            GUI.color = Color.white;
            float x = rect.x, y = rect.y, h = rect.height; int rows = 1;
            Handles.BeginGUI();
            for (int i = 0; i < flagDefs.Count; i++)
            {
                if ((flagMask & (1 << i)) == 0) continue;
                string label = flagDefs[i].DisplayLabel; float w = GUI.skin.label.CalcSize(new GUIContent(label)).x + h;
                if (x + w > rect.xMax) { rows++; x = rect.x; y += h + 4; }
                Color color = flagDefs[i].color; Rect pill = new Rect(x, y, w, h);
                bool hovered = pill.Contains(Event.current.mousePosition); bool isSelected = requiredFlags != 0 && (requiredFlags & (1 << i)) != 0;
                float b = isSelected ? 0.7f : 0.2f;
                if (hovered) b = isSelected ? 0.9f : 0.5f;
                DrawPill(new Rect(x - 2f, y - 2f, w + 4f, h + 4f), new Color(color.r * b, color.g * b, color.b * b), offset: new Vector2(0, 1)); DrawPill(pill, !isSelected && requiredFlags != 0 ? new Color(color.r * 0.4f, color.g * 0.4f, color.b * 0.4f) : color);
                GUI.Label(pill, label, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white }, hover = { textColor = Color.white }, fontStyle = isSelected ? FontStyle.Bold : FontStyle.Normal });
                if (Event.current.type == EventType.MouseDown && hovered) { requiredFlags ^= (1 << i); Event.current.Use(); }
                x += w + 6;
            } Handles.EndGUI(); return rows;
        }
        private void DrawPill(Rect r, Color col, Vector2 offset = default)
        {
            float radius = r.height * 0.5f; r.x += offset.x; r.y += offset.y;
            EditorGUI.DrawRect(new Rect(r.x + radius - 1f, r.y, r.width - radius * 2 + 2f, r.height), col);
            Handles.color = col;
            Handles.DrawSolidArc(new Vector3(r.x + radius, r.y + radius, 0), Vector3.forward, Vector3.up, 180f, radius); Handles.DrawSolidArc(new Vector3(r.xMax - radius, r.y + radius, 0), Vector3.forward, Vector3.up, -180f, radius);
        }
        private int CalculateFlagRows(Rect rect, int flagMask)
        {
            var flagDefs = NoteBoardOptions.Instance.flags;
            float x = rect.x; int rows = 1;
            for (int i = 0; i < flagDefs.Count; i++)
            {
                if ((flagMask & (1 << i)) == 0) continue;
                float w = GUI.skin.label.CalcSize(new GUIContent(flagDefs[i].DisplayLabel)).x + rect.height;
                if (x + w > rect.xMax) { rows++; x = rect.x; }
                x += w + 6;
            } return rows;
        }
        private void HandleDragRelease()
        {
            if (Event.current.type != EventType.MouseUp) return;
            if (isDragging) ApplyDrag();
            isDragging = false; dragNote = null; hoverIndex = -1; hoverStatus = -1;
            Event.current.Use();
        }
        private void ApplyDrag()
        {
            if (dragNote == null) return;
            ReorderWithinStatus(dragNote, hoverStatus, hoverDropBefore ? hoverIndex : hoverIndex + 1);
            needsRefresh = true;
        }
        private void ReorderWithinStatus(CachedNote note, int statusIndex, int index)
        {
            Undo.RegisterCompleteObjectUndo(NoteBoardOptions.Instance, "Reorder Notes");
            if (note == null) return;
            int oldStatus = note.record.data.statusIndex;
            if (oldStatus != statusIndex) NormalizeList(cachedNotes.Where(n => n.record.data.statusIndex == oldStatus && n != note).OrderBy(n => n.record.order).ToList(), oldStatus);
            var list = cachedNotes.Where(n => n.record.data.statusIndex == statusIndex).OrderBy(n => n.record.order).ToList();
            int oldIndex = list.IndexOf(note);
            if (oldIndex != -1) { list.Remove(note); if (oldIndex < index) index--; }
            list.Insert(Mathf.Clamp(index, 0, list.Count), note);
            NormalizeList(list, statusIndex);
        }
        private void NormalizeList(List<CachedNote> list, int statusIndex)
        {
            List<CachedNote> ordered;
            if (statusIndex == -1) ordered = list.Where(n => n.record.data.flagMask != 0).Concat(list.Where(n => n.record.data.flagMask == 0)).ToList();
            else ordered = list;
            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].record.data.statusIndex = statusIndex; ordered[i].record.order = i;
                NoteBoardOptions.Instance.SaveRecord(ordered[i].record, flush: false);
            }
            if (ordered.Count > 0) NoteBoardOptions.Instance.Flush();
        }
        private List<CachedNote> SortByOrder(List<CachedNote> list) => list.OrderBy(e => e.record.order).ToList();
        private void RefreshCacheIfNeeded()
        {
            if (!needsRefresh || EditorApplication.timeSinceStartup - lastCacheTime < cacheCooldown) return;
            cachedNotes.Clear();
            CollectAssetNotes(cachedNotes); CollectSceneNotes(cachedNotes);
            needsRefresh = false; lastCacheTime = EditorApplication.timeSinceStartup;
        }
        private void CollectAssetNotes(List<CachedNote> list)
        {
            foreach (var record in NoteBoardOptions.Instance.records.ToList())
            {
                if (string.IsNullOrEmpty(record.id) || GlobalObjectId.TryParse(record.id, out _)) continue;
                string path = AssetDatabase.GUIDToAssetPath(record.id); var asset = AssetDatabase.LoadMainAssetAtPath(path);
                if (!asset) { NoteBoardOptions.Instance.RemoveAllRecordsForOwner(record.id); continue; }
                if (record.name != asset.name) record.name = asset.name;
                list.Add(new CachedNote(record, asset));
            }
        }
        private void CollectSceneNotes(List<CachedNote> list)
        {
            foreach (var record in NoteBoardOptions.Instance.records.ToList())
            {
                if (!GlobalObjectId.TryParse(record.id, out var gid)) continue;
                var obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(gid); var go = obj as GameObject;
                if (go == null)
                {
                    string scenePath = AssetDatabase.GUIDToAssetPath(gid.assetGUID.ToString());
                    for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
                    {
                        var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                        if (scene.path == scenePath) { NoteBoardOptions.Instance.RemoveRecord(record); break; }
                    } list.Add(new CachedNote(record, null)); continue;
                }
                if (record.name != go.name) record.name = go.name;
                list.Add(new CachedNote(record, go));
            }
        }
        private bool MatchesFilter(NoteData data) => requiredFlags == 0 || (data.flagMask & requiredFlags) != 0;
        private bool MatchesSearch(string name, NoteData data)
        {
            if (string.IsNullOrEmpty(searchQuery)) return true;
            return name.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0 || (!string.IsNullOrEmpty(data.notes) && data.notes.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0);
        }
        private static string GetEllipsedText(string text, GUIStyle style, float maxWidth)
        {
            if (string.IsNullOrEmpty(text) || style.CalcSize(new GUIContent(text)).x <= maxWidth) return text;
            const string e = "...";
            if (style.CalcSize(new GUIContent(e)).x > maxWidth) return e;
            int lo = 0, hi = text.Length;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                if (style.CalcSize(new GUIContent(text.Substring(0, mid) + e)).x <= maxWidth) lo = mid + 1;
                else hi = mid;
            }
            return text.Substring(0, Mathf.Clamp(lo - 1, 0, text.Length)) + e;
        }
        private class CachedNote
        {
            public NoteRecord record; public UnityEngine.Object unityObject;
            public CachedNote(NoteRecord record, UnityEngine.Object unityObject) { this.record = record; this.unityObject = unityObject; }
        }
    }
}