using UnityEditor;
using UnityEngine;

namespace MarbleTrackBuilder.Editor
{
    [InitializeOnLoad]
    public static class EditorMovementManager
    {
        private static Transform lastSelectedTransform;
        private static Vector3 lastPosition;
        private static bool wasMoving = false;

        static EditorMovementManager()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnEditorUpdate()
        {
            if (Selection.activeTransform == null)
            {
                lastSelectedTransform = null;
                return;
            }

            if (Selection.activeTransform != lastSelectedTransform)
            {
                lastSelectedTransform = Selection.activeTransform;
                lastPosition = lastSelectedTransform.position;
                wasMoving = false;
                return;
            }

            if (lastSelectedTransform.position != lastPosition)
            {
                wasMoving = true;
            }
            else if (wasMoving)
            {
                IEditorMovementListener[] listeners = lastSelectedTransform.GetComponentsInChildren<IEditorMovementListener>();
                
                foreach (var listener in listeners)
                {
                    listener.OnEditorObjectMoved();
                }

                wasMoving = false;
            }

            lastPosition = lastSelectedTransform.position;
        }
    }
}
