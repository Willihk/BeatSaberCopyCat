using UnityEngine;

namespace BeatGame.Logic.Lasers
{
    public class RotatingMeshLaserController : LaserControllerBase
    {
        [SerializeField]
        MeshRenderer renderer;

        protected Quaternion startRotation;
        protected float rotationSpeed;

        private void OnEnable()
        {
            if (renderer == null)
                TryGetComponent(out renderer);

            startRotation = transform.rotation;
        }

        void Update()
        {
            if (rotationSpeed != 0)
            {
                transform.Rotate(new Vector3(rotationSpeed * 2 * Time.deltaTime, 0, 0), Space.Self);
            }
        }

        public override void TurnOff()
        {
            base.TurnOff();
            if (rotationSpeed != 0)
                transform.rotation = startRotation;
        }

        public override void SetMaterial(Material material)
        {
            base.SetMaterial(material);
            if (renderer != null || TryGetComponent(out renderer))
                renderer.sharedMaterial = material;
        }

        public override void SetRotation(float value)
        {
            if (value == 0)
            {
                // Reset to default
                transform.rotation = startRotation;
            }

            rotationSpeed = value;
        }
    }
}