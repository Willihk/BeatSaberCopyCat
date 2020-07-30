using UnityEngine;
using System.Collections;

namespace BeatGame.Logic.Lasers
{
    public class LaserControllerBase : MonoBehaviour
    {
        [SerializeField]
        protected float laserIntensity = 15;
        [SerializeField]
        protected float laserFlashIntensity = 30;

        protected Quaternion startRotation;
        protected float rotationSpeed;

        protected Material material;

        public virtual void TurnOff() { }

        public virtual void TurnOn() { }

        public virtual void SetMaterial(Material material)
        {
            this.material = material;
        }

        public virtual void SetRotation(float value)
        {
            if (value == 0)
            {
                transform.rotation = startRotation;
                // Reset to default
            }

            rotationSpeed = value;
        }
    }
}