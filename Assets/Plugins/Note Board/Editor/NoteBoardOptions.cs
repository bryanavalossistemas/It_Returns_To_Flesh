namespace Tornadoally.NoteBoard
{
    using System; using System.Collections.Generic; using System.Linq;
    using UnityEditor; using UnityEngine;
    public class NoteBoardOptions : ScriptableObject
    {
        private static NoteBoardOptions _instance;
        public static NoteBoardOptions Instance
        {
            get
            {
                if (_instance != null) return _instance;
                string[] folderGuids = AssetDatabase.FindAssets("Note Board t:folder"); string folderPath = folderGuids.Length > 0 ? AssetDatabase.GUIDToAssetPath(folderGuids[0]) : "Assets/Note Board";
                if (folderGuids.Length == 0) { AssetDatabase.CreateFolder("Assets", "Note Board"); AssetDatabase.Refresh(); }
                string assetPath = $"{folderPath}/NoteBoardOptions.asset"; _instance = AssetDatabase.LoadAssetAtPath<NoteBoardOptions>(assetPath);
                if (_instance == null)
                {
                    _instance = CreateInstance<NoteBoardOptions>(); AssetDatabase.CreateAsset(_instance, assetPath); AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
                } return _instance;
            }
        }
        public List<NoteRecord> records = new List<NoteRecord>(); public List<NoteStatusDef> columns = new List<NoteStatusDef>(); public List<NoteFlagDef> flags = new List<NoteFlagDef>();
        private void OnEnable()
        {
            if (columns == null || columns.Count == 0) columns = new List<NoteStatusDef> { new NoteStatusDef("TODO", "📋", NoteColors.TODO), new NoteStatusDef("WIP", "🛠️", NoteColors.WIP), new NoteStatusDef("DONE", "✔️", NoteColors.Done) };
            if (flags == null || flags.Count == 0) flags = new List<NoteFlagDef> { new NoteFlagDef("Stable", "🧱", NoteColors.Stable), new NoteFlagDef("Refactor", "🧠", NoteColors.Refactor), new NoteFlagDef("Experiment", "🧪", NoteColors.Experiment), new NoteFlagDef("Bug", "👾", NoteColors.Bug), new NoteFlagDef("Placeholder", "🚧", NoteColors.Placeholder) };
        }
        public List<NoteRecord> GetRecordsForOwner(string id) => records.Where(r => r.id == id).ToList();
        public void SaveRecord(NoteRecord record, bool flush = true)
        {
            if (string.IsNullOrEmpty(record.noteTag)) record.noteTag = Guid.NewGuid().ToString("N");
            if (record.data.IsEmpty())
            {
                RemoveRecord(record);
                EditorUtility.SetDirty(this);
                if (flush) AssetDatabase.SaveAssets();
                return;
            }
            if (!records.Contains(record)) records.Add(record);
            EditorUtility.SetDirty(this);
            if (flush) AssetDatabase.SaveAssets();
        }
        public void RemoveRecord(NoteRecord record, bool noUndo = false)
        {
            if (!noUndo) Undo.RegisterCompleteObjectUndo(this, "Remove Note");
            records.Remove(record);
            if (GetRecordsForOwner(record.id).Count == 0 && GlobalObjectId.TryParse(record.id, out var gid))
            {
                var obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(gid);
                if (obj is GameObject go) EditorGUIUtility.SetIconForObject(go, null);
            }
            EditorUtility.SetDirty(this); AssetDatabase.SaveAssets();
        }
        public void RemoveAllRecordsForOwner(string id, bool noUndo = false)
        {
            if (!noUndo) Undo.RegisterCompleteObjectUndo(this, "Remove Notes");
            if (GlobalObjectId.TryParse(id, out var gid))
            {
                var obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(gid);
                if (obj is GameObject go) EditorGUIUtility.SetIconForObject(go, null);
            }
            records.RemoveAll(r => r.id == id);
            EditorUtility.SetDirty(this); AssetDatabase.SaveAssets();
        }
        public void Flush() => AssetDatabase.SaveAssets();
    }
    [Serializable] public class NoteRecord { public string id; public string noteTag; public string name; public NoteData data; public int order; public bool isExpanded; }
}