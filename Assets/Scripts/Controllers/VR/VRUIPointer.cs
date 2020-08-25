using UnityEngine;
using System.Collections;
using BeatGame.Logic.VR;
using UnityEngine.EventSystems;

namespace  BeatGame.Logic.VR
{
    public class VRUIPointer : MonoBehaviour
    {
        [SerializeField]
        float length = 5;
        [SerializeField]
        GameObject dot;
        [SerializeField]
        public Camera Camera;

        LineRenderer lineRenderer;
        VRInputModule inputModule;

        // Use this for initialization
        void Start()
        {
            if (lineRenderer == null)
                lineRenderer = GetComponent<LineRenderer>();

            Camera = GetComponent<Camera>();
            Camera.enabled = false;

            inputModule = EventSystem.current.gameObject.GetComponent<VRInputModule>();
        }

        // Update is called once per frame
        void Update()
        {
            // Use default or distance
            PointerEventData data = inputModule.Data;

            Physics.Raycast(new Ray((Vector3)transform.position, (Vector3)transform.forward), out RaycastHit hit, length);

            // If nothing is hit, set do default length
            float colliderDistance = hit.distance == 0 ? length : hit.distance;
            float canvasDistance = data.pointerCurrentRaycast.distance == 0 ? length : data.pointerCurrentRaycast.distance;

            // Get the closest one
            float targetLength = Mathf.Min(colliderDistance, canvasDistance);

            // Default
            Vector3 endPosition = transform.position + (transform.forward * targetLength);

            // Set position of the dot
            dot.transform.position = endPosition;

            // Set linerenderer
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, endPosition);
        }
    }
}