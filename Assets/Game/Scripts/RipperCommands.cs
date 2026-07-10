using UnityEngine;
using UnityEngine.Localization;
using System;
using System.Linq;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

#region RipperActions
public abstract class RipperAction { }
public class VomitRA : RipperAction
{
    public float duration = 5f;
    public bool cancellable = true;
}
public class SoresRA : RipperAction
{
    public Vector2 jumpForce;
}
public class ExplodeRA : RipperAction
{
    public float explosionRadius = 9.73f;
    public Vector2 explosionForce = new(43.7f, 15.4f);
}
public class FrenzyRA : RipperAction
{
    public float speedMultiplier = 2f;
    public float hungryMultiplier = 3f;
    public float visionRange = 6f;
    public float duration = 5f;
}
#endregion

[CreateAssetMenu(fileName = "RipperCommand", menuName = "Scriptable Objects/Ripper/Commands")]
public class RipperCommands : ScriptableObject
{
    public LocalizedString title, description;
    public int hpNeeded = 2;
    [SerializeReference] public RipperAction action;

    #region Editor
#if UNITY_EDITOR
    [CustomEditor(typeof(RipperCommands))]
    public class RipperCommands_Editor : Editor
    {
        private static Type[] types;
        private static string[] names;
        private VisualElement root, ripperActionVE;
        private DropdownField dropdown;
        private SerializedProperty actionProp;

        void OnEnable()
        {
            if (types != null) return;
            Type baseType = typeof(RipperAction);
            types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t)).ToArray();
            names = types.Select(t => t.Name).ToArray();
            Undo.undoRedoPerformed += OnUndoRedo;
        }
        void OnDisable() => Undo.undoRedoPerformed -= OnUndoRedo;

        private void OnUndoRedo()
        {
            if (root == null) return;
            serializedObject.Update();
            RebuildRipperAction();
        }

        public override VisualElement CreateInspectorGUI()
        {
            root = new();
            serializedObject.Update();
            SerializedProperty iterator = serializedObject.GetIterator();
            if (iterator.NextVisible(true))
            {
                do
                {
                    if (iterator.name == "m_Script" || iterator.name == "action") continue;
                    PropertyField field = new(iterator.Copy());
                    field.Bind(serializedObject);
                    root.Add(field);

                } while (iterator.NextVisible(false));
            }

            actionProp = serializedObject.FindProperty("action");
            dropdown = new()
            {
                label = "RipperAction",
                choices = names.Prepend("<Select RipperAction>").ToList(),
                index = managedReference() == null ? 0 : Array.FindIndex(types, x => x == managedReference().GetType()) + 1
            };
            dropdown.RegisterValueChangedCallback(evt =>
            {
                serializedObject.Update();
                Undo.RecordObject(target, "Change RipperAction");
                dropdown.choices.Remove("<Select RipperAction>");
                actionProp.managedReferenceValue = (RipperAction)Activator.CreateInstance(types[dropdown.index]);
                serializedObject.ApplyModifiedProperties();
                root.Bind(serializedObject);
                RebuildRipperAction();
                EditorUtility.SetDirty(target);
            });
            root.Add(dropdown);

            ripperActionVE = new VisualElement();
            root.Add(ripperActionVE);

            RebuildRipperAction();
            return root;

            object managedReference() => actionProp.managedReferenceValue;
        }

        private void RebuildRipperAction()
        {
            ripperActionVE.Clear();
            if (actionProp.managedReferenceValue == null) return;

            SerializedProperty copy = actionProp.Copy(), end = copy.GetEndProperty();
            copy.NextVisible(true);
            do
            {
                if (SerializedProperty.EqualContents(copy, end)) break;
                PropertyField field = new(copy.Copy());
                field.Bind(serializedObject);
                ripperActionVE.Add(field);
            } while (copy.NextVisible(false));
        }
    }
#endif
    #endregion
}