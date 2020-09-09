using UnityEngine;
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
        }

        public override void SetMaterial(Material material)
        {
            if (renderer != null || TryGetComponent(out renderer))
                renderer.sharedMaterial = material;
        }
    }
}