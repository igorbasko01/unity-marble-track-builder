using UnityEngine;

namespace MarbleTrackBuilder
{
    [ExecuteAlways]
    public class FlatEdgeConnector : MonoBehaviour, IEditorMovementListener
    {
        private Vector3 connectionCenter = Vector3.zero;
        private Vector3 connectionNormal = Vector3.forward;

        public FlatEdgeConnector connectedTo = null;

        public Vector3 WorldCenter => transform.TransformPoint(connectionCenter);
        public Vector3 WorldNormal => transform.TransformDirection(connectionNormal);

        public bool IsConnected => connectedTo != null;

        public void OnEditorObjectMoved()
        {
            Debug.Log($"FlatEdgeConnector '{name}' detected movement.");
        }

        private void OnDrawGizmos()
        {
            DrawConnectorGizmo(false);
        }

        private void OnDrawGizmosSelected()
        {
            DrawConnectorGizmo(true);
        }

        private void DrawConnectorGizmo(bool isSelected)
        {
            Vector3 worldCenter = WorldCenter;
            Vector3 worldNormal = WorldNormal;

            Vector3 gizmoPosition = worldCenter + worldNormal * 0.2f;

            if (IsConnected)
            {
                Gizmos.color = Color.red;
            }
            else if (isSelected)
            {
                Gizmos.color = Color.yellow;
            }
            else
            {
                Gizmos.color = Color.green;
            }

            Gizmos.DrawWireSphere(gizmoPosition, 0.1f);
        }
    }
}
