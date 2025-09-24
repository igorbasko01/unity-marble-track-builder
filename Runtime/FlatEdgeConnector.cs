using UnityEngine;

namespace MarbleTrackBuilder
{
    [ExecuteAlways]
    public class FlatEdgeConnector : MonoBehaviour, IEditorMovementListener
    {
        public void OnEditorObjectMoved()
        {
            Debug.Log($"FlatEdgeConnector '{name}' detected movement.");
        }
    }
}
