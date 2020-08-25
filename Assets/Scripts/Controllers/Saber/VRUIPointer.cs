using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Controllers.Saber
{
    public class VRUIPointer : MonoBehaviour
    {
        float length = 5;
        GameObject dot;

        LineRenderer lineRenderer;

        // Use this for initialization
        void Start()
        {
            if (lineRenderer == null)
                lineRenderer = GetComponent<LineRenderer>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}