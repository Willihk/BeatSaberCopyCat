using UnityEngine;
using System.Collections;

public class MeshLaserController : LaserControllerBase
{
    [SerializeField]
    MeshRenderer renderer;

    private void Start()
    {
        if (renderer == null)
            TryGetComponent(out renderer);


        startRotation = transform.rotation;

        material = renderer.sharedMaterial;
    }

    void Update()
    {
        if (rotationSpeed != 0)
        {
            transform.Rotate(new Vector3(rotationSpeed * 2 * Time.deltaTime, 0, 0), Space.Self);
        }
    }

    public override void SetMaterial(Material material)
    {
        base.SetMaterial(material);
        renderer.material = material;
    }
}
