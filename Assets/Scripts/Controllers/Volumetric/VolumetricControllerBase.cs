using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumetricControllerBase : MonoBehaviour
{
    protected Material material;

    public virtual void TurnOff() { }

    public virtual void TurnOn() { }

    public virtual void SetMaterial(Material material)
    {
        this.material = material;
    }
}
