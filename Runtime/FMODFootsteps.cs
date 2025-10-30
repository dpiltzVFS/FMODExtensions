using UnityEngine;

namespace FMODExtensions
{
    public class FMODFootsteps : MonoBehaviour
    {
        [SerializeField] public FMODUnity.EventReference _footstepEvent;

        public virtual void PlayFootstep(RaycastHit hit, float speed)
        {
            FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(_footstepEvent);
            instance.setParameterByName("Material", GetSurfaceMaterial(hit));
            instance.setParameterByName("Speed", speed);
            instance.start();
            instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
            instance.release();
        }

        // get surface material value from GameObject
        public float GetSurfaceMaterial(RaycastHit hit)
        {
            if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out FMODSurface surface) && surface.SurfaceMaterial != null) return surface.SurfaceMaterial.SurfaceValue;
            return 0f;
        }
    }
}