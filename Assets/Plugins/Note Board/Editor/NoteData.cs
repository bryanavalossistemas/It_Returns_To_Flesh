namespace Tornadoally.NoteBoard
{
    using System;
    using UnityEngine;
    [Serializable] public class NoteStatusDef
    {
        public string name; public string emoji; public Color color = new Color(0.5f, 0.5f, 0.5f, 1f);
        public NoteStatusDef(string name, string emoji, Color color) { this.name = name; this.emoji = emoji; this.color = color; }
        public string DisplayLabel => emoji + name;
    }
    [Serializable] public class NoteFlagDef
    {
        public string name; public string emoji; public Color color = new Color(1f, 0f, 1f, 1f);
        public NoteFlagDef(string name, string emoji, Color color) { this.name = name; this.emoji = emoji; this.color = color; }
        public string DisplayLabel => emoji + name;
    }
    [Serializable] public class NoteData
    {
        public int statusIndex = -1; public int flagMask = 0; public string notes = string.Empty;
        public NoteData() { statusIndex = -1; flagMask = 0; notes = string.Empty; }
        public bool IsEmpty() => string.IsNullOrWhiteSpace(notes) && statusIndex < 0 && flagMask == 0;
    }
    public static class NoteColors
    {
        public static readonly Color TODO = new Color(156f / 255f, 60f / 255f, 48f / 255f, 1f);
        public static readonly Color WIP = new Color(1f, 47f / 51f, 4f / 255f, 1f);
        public static readonly Color Done = new Color(43f / 255f, 173f / 255f, 43f / 255f, 1f);
        public static readonly Color Flagged = new Color(1f, 0f, 1f, 1f);
        public static readonly Color Note = new Color(0.5f, 0.5f, 0.5f, 1f);
        public static readonly Color Empty = new Color(0.8274511f, 0.8274511f, 0.8274511f, 1f);
        public static readonly Color Stable = new Color(200f / 255f, 100f / 255f, 50f / 255f);
        public static readonly Color Refactor = new Color(230f / 255f, 110f / 255f, 130f / 255f);
        public static readonly Color Experiment = new Color(0f / 255f, 150f / 255f, 125f / 255f);
        public static readonly Color Bug = new Color(136f / 255f, 108f / 255f, 228f / 255f);
        public static readonly Color Placeholder = new Color(150f / 255f, 140f / 255f, 40f / 255f);
    }
}