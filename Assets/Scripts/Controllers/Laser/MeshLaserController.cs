using UnityEngine;
using System.Collections;
namespace BeatGame.Logic.Lasers
{
    public class MeshLaserController : LaserControllerBase
    {
        [SerializeField]
        MeshRenderer renderer;

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

        public override void SetMaterial(Material material)
        {
            if (renderer != null || TryGetComponent(out renderer))
                renderer.sharedMaterial = material;
        }
    }
}