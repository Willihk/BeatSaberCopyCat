using UnityEngine;
using System.Collections;

public class LightBeamController : VolumetricControllerBase
{
    [SerializeField]
    Renderer renderer;

    private void OnEnable()
    {
        if (renderer == null)
            TryGetComponent(out renderer);

        if (renderer != null)
        material = renderer.sharedMaterial;
    }

    public override void SetMaterial(Material material)
    {
        base.SetMaterial(material);
        if (renderer == null)
            TryGetComponent(out renderer);

        renderer.material = material;
    }
}
