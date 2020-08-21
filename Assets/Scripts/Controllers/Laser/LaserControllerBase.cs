using UnityEngine;
using System.Collections;

namespace BeatGame.Logic.Lasers
{
    public class LaserControllerBase : MonoBehaviour
    {
        protected Quaternion startRotation;
        protected float rotationSpeed;

        public virtual void TurnOff() { }

        public virtual void TurnOn() { }

        public virtual void SetMaterial(Material material)
        {
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