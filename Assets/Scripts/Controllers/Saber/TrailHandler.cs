using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class TrailSection
{
    public Vector3 point;
    public Vector3 upDir;
    public float time;
    public TrailSection()
    {

    }
    public TrailSection(Vector3 p, float t)
    {
        point = p;
        time = t;
    }
}
[RequireComponent(typeof(MeshFilter))]
public class TrailHandler : MonoBehaviour
{

    public GameObject pointStart;
    public GameObject pointEnd;
    public float height;
    public float time = 0.2f;
    public bool alwaysUp = false;
    public float minDistance = 0f;
    public float timeTransitionSpeed = 0f;
    public float desiredTime = 0f;
    public Color startColor = Color.white;
    public Color endColor = new Color(1, 1, 1, 0);
    public Material mat;
    Vector3 position;
    float now = 0;
    TrailSection currentSection;
    Matrix4x4 localSpaceTransform;
    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] colors;
    private Vector2[] uv;
    private MeshRenderer meshRenderer;
    private List<TrailSection> sections = new List<TrailSection>();
    bool loaded = false;
    public void Onload()
    {

        MeshFilter meshF = GetComponent(typeof(MeshFilter)) as MeshFilter;
        mesh = meshF.mesh;
        meshRenderer = GetComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshRenderer.material = mat;

        height = Vector3.Distance(pointStart.transform.position, pointEnd.transform.position);
        loaded = true;
    }

    public void Iterate(float itterateTime)
    {

        position = transform.position;
        now = itterateTime;

        // Add a new trail section
        if (sections.Count == 0 || (sections[0].point - position).sqrMagnitude > minDistance * minDistance)
        {
            TrailSection section = new TrailSection();
            section.point = position;
            if (alwaysUp)
                section.upDir = Vector3.up;
            else
                section.upDir = transform.TransformDirection(Vector3.up);

            section.time = now;
            sections.Insert(0, section);

        }
    }
    public void UpdateTrail(float currentTime, float deltaTime)
    { // ** call once a frame **

        // Rebuild the mesh	
        mesh.Clear();
        //
        // Remove old sections
        while (sections.Count > 0 && currentTime > sections[sections.Count - 1].time + time)
        {
            sections.RemoveAt(sections.Count - 1);
        }
        // We need at least 2 sections to create the line
        if (sections.Count < 2)
            return;
        //
        vertices = new Vector3[sections.Count * 2];
        colors = new Color[sections.Count * 2];
        uv = new Vector2[sections.Count * 2];
        //
        currentSection = sections[0];
        //
        // Use matrix instead of transform.TransformPoint for performance reasons
        localSpaceTransform = transform.worldToLocalMatrix;

        // Generate vertex, uv and colors
        for (var i = 0; i < sections.Count; i++)
        {
            //
            currentSection = sections[i];
            // Calculate u for texture uv and color interpolation
            float u = 0.0f;
            if (i != 0)
                u = Mathf.Clamp01((currentTime - currentSection.time) / time);
            //
            // Calculate upwards direction
            Vector3 upDir = currentSection.upDir;

            // Generate vertices
            vertices[i * 2 + 0] = localSpaceTransform.MultiplyPoint(currentSection.point);
            vertices[i * 2 + 1] = localSpaceTransform.MultiplyPoint(currentSection.point + upDir * height);

            uv[i * 2 + 0] = new Vector2(1, u);
            uv[i * 2 + 1] = new Vector2(0, u);

            // fade colors out over time
            Color interpolatedColor = Color.Lerp(startColor, endColor, u);
            colors[i * 2 + 0] = interpolatedColor;
            colors[i * 2 + 1] = interpolatedColor;
        }

        // Generate triangles indices
        int[] triangles = new int[(sections.Count - 1) * 2 * 3];
        for (int i = 0; i < triangles.Length / 6; i++)
        {
            triangles[i * 6 + 0] = i * 2;
            triangles[i * 6 + 1] = i * 2 + 1;
            triangles[i * 6 + 2] = i * 2 + 2;

            triangles[i * 6 + 3] = i * 2 + 2;
            triangles[i * 6 + 4] = i * 2 + 1;
            triangles[i * 6 + 5] = i * 2 + 3;
        }

        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.uv = uv;
        mesh.triangles = triangles;
        //
        // Tween to the desired time
        //
        if (time > desiredTime)
        {
            time -= deltaTime * timeTransitionSpeed;
            if (time <= desiredTime) time = desiredTime;
        }
        else if (time < desiredTime)
        {
            time += deltaTime * timeTransitionSpeed;
            if (time >= desiredTime) time = desiredTime;
        }
    }

    public void ClearTrail()
    {
        desiredTime = 0;
        time = 0;
        if (mesh != null)
        {
            mesh.Clear();
            sections.Clear();
        }
    }

    void Update()
    {
        if (loaded)
        {
            Iterate(Time.time);
            UpdateTrail(Time.time, 0f);
        }
    }
}

