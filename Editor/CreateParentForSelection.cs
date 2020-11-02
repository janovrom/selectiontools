using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Janovrom.SelectionTools
{

    public static class CreateParentForSelection
    {
        
        [MenuItem("GameObject/Group selection #%g", false, 0)]
        public static void GroupSelection()
        {
            if (Selection.transforms.Length == 0)
                return;

            // Compute center. This will be the position of the new group transform
            Vector3 center = Vector3.zero;
            for (int i = 0; i < Selection.transforms.Length; ++i)
                center += Selection.transforms[i].position;
            center /= Selection.transforms.Length;

            // Create undo group
            int group = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Group");

            // Create new game object (group parent) as an undoable opertation
            var go = new GameObject(Selection.activeTransform.name);
            go.transform.position = center;
            Undo.RegisterCreatedObjectUndo(go, "Create group parent");

            for (int i = 0; i < Selection.transforms.Length; ++i)
                Undo.SetTransformParent(Selection.transforms[i], go.transform, "Attach to group");

            Undo.CollapseUndoOperations(group);
        }

        [MenuItem("GameObject/Dissolve group %DEL", false, 0)]
        public static void DissolveGroup()
        {
            if (Selection.transforms.Length == 0)
                return;

            int group = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Dissolve parent");

            for (int i = 0; i < Selection.transforms.Length; ++i)
            {
                // Grab parent, current node and its children
                var parent = Selection.transforms[i].parent;
                var current = Selection.transforms[i];
                for (int j = current.childCount - 1; j >= 0; --j)
                {
                    Undo.SetTransformParent(transform: current.GetChild(j), newParent: parent, "Remove from parent");
                }
                Undo.DestroyObjectImmediate(current.gameObject);
            }

            Undo.CollapseUndoOperations(group);
        }

    }

}
