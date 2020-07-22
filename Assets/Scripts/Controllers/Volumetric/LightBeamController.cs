using UnityEngine;
using System.Collections;

public class LightBeamController : VolumetricControllerBase
{
    [SerializeField]
    Renderer renderer;

    private void Awake()
    {
        if (renderer == null)
            TryGetComponent(out renderer);

        material = renderer.sharedMaterial;
    }

    public override void SetMaterial(Material material)
    {
        base.SetMaterial(material);
        renderer.material = material;
    }
}
