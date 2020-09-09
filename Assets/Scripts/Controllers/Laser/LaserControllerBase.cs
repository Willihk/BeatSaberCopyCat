using UnityEngine;

namespace BeatGame.Logic.Lasers
{
    public class LaserControllerBase : MonoBehaviour
    {
        public virtual void TurnOff() { }

        public virtual void TurnOn() { }

        public virtual void SetMaterial(Material material) { }

        public virtual void SetRotation(float value) { }
    }
}