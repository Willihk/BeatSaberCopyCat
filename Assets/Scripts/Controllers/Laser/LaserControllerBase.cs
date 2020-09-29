using System.Collections;
using UnityEngine;

namespace BeatGame.Logic.Lasers
{
    public class LaserControllerBase : MonoBehaviour
    {
        Material material;

        public virtual void TurnOff()
        {
        }

        public virtual void TurnOn()
        {
        }

        public virtual void SetMaterial(Material material)
        {
            this.material = material;
        }

        public virtual void SetRotation(float value) { }
    }
}