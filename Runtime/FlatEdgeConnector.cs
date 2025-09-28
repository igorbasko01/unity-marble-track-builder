using UnityEditor;
using UnityEngine;

namespace MarbleTrackBuilder
{
    [ExecuteAlways]
    public class FlatEdgeConnector : MonoBehaviour, IEditorMovementListener
    {
        private Vector3 connectionCenter = Vector3.zero;
        private Vector3 connectionNormal = Vector3.forward;

        [Header("Connection Reference")]
        public FlatEdgeConnector connectedTo = null;

        private float snapDistance = 0.4f;
        private float snapAngleTolerance = 10.0f;

        public Vector3 WorldCenter => transform.TransformPoint(connectionCenter);
        public Vector3 WorldNormal => transform.TransformDirection(connectionNormal);

        public bool IsConnected => connectedTo != null;

        public void OnEditorObjectMoved()
        {
            CheckForSnapping();
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

        private void CheckForSnapping()
        {
            if (IsConnected)
            {
                CheckForAutoDisconnect();
                return;
            }

            FlatEdgeConnector[] allConnectors = FindObjectsByType<FlatEdgeConnector>(FindObjectsSortMode.None);
            FlatEdgeConnector bestCandidate = null;
            float closestDistance = float.MaxValue;

            foreach (var other in allConnectors)
            {
                if (CanSnapTo(other, out float distance))
                {
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        bestCandidate = other;
                    }
                }
            }

            if (bestCandidate != null && closestDistance <= snapDistance)
            {
                SnapTo(bestCandidate);
            }
        }

        private void SnapTo(FlatEdgeConnector other)
        {
            Debug.Log($"Snapping {transform.parent.name} to {other.transform.parent.name} distance {Vector3.Distance(WorldCenter, other.WorldCenter):F2}");

            Undo.RecordObject(transform.parent, "Snap Track Pieces");
            Undo.RecordObject(other.transform.parent, "Snap Track Pieces");
            Undo.RecordObject(this, "Snap Track Pieces");
            Undo.RecordObject(other, "Snap Track Pieces");

            Vector3 thisTrackPieceOffset = WorldCenter - transform.parent.position;
            Vector3 targetPosition = other.WorldCenter - thisTrackPieceOffset;

            transform.parent.position = targetPosition;

            connectedTo = other;
            other.connectedTo = this;

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(other);
            EditorUtility.SetDirty(transform.parent.gameObject);
            EditorUtility.SetDirty(other.transform.parent.gameObject);

            Debug.Log($"Snapped {transform.parent.name} to {other.transform.parent.name}");
        }

        private bool CanSnapTo(FlatEdgeConnector other, out float distance)
        {
            distance = float.MaxValue;

            if (other == this) return false;

            // Can't snap to connector on same track piece
            if (other.transform.parent == this.transform.parent) return false;

            if (other.IsConnected) return false;

            distance = Vector3.Distance(WorldCenter, other.WorldCenter);
            if (distance > snapDistance) return false;

            float normalDot = Vector3.Dot(WorldNormal, other.WorldNormal);
            float angleError = Mathf.Acos(Mathf.Clamp(-normalDot, -1f, 1f)) * Mathf.Rad2Deg;

            return angleError < snapAngleTolerance;
        }

        private void CheckForAutoDisconnect()
        {
            if (IsConnected && connectedTo != null)
            {
                float distance = Vector3.Distance(WorldCenter, connectedTo.WorldCenter);
                Debug.Log($"Distance to connected piece: {distance:F2}");
                if (distance > snapDistance * 2)
                {
                    Debug.Log($"Auto-disconnecting {name} - distance {distance:F2} > {snapDistance * 2:F2}");
                    Disconnect();
                }
            }
        }

        private void Disconnect()
        {
            if (IsConnected)
            {
                Undo.RecordObject(this, "Disconnect Track Pieces");
                Undo.RecordObject(connectedTo, "Disconnect Track Pieces");

                FlatEdgeConnector other = connectedTo;
                connectedTo = null;
                other.connectedTo = null;

                EditorUtility.SetDirty(this);
                EditorUtility.SetDirty(other);

                Debug.Log($"Disconnected {transform.parent.name} from {other.transform.parent.name}");
            }
        }
    }
}
