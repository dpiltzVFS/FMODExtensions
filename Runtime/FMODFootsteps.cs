using UnityEngine;

namespace FMODExtensions
{
    public class FMODFootsteps : MonoBehaviour
    {
        [SerializeField] public FMODUnity.EventReference _footstepEvent;
        [SerializeField] public FMODUnity.EventReference _jumpEvent;
        [SerializeField] public FMODUnity.EventReference _landEvent;

        public virtual void PlayFootstep(RaycastHit hit, float speed)
        {
            if (!_footstepEvent.IsNull)
            {
                FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(_footstepEvent);
                instance.setParameterByName("Material", GetSurfaceMaterial(hit));
                instance.setParameterByName("Speed", speed);
                instance.start();
                instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
                instance.release();
            }
        }

        public virtual void PlayJump(RaycastHit hit, float speed)
        {
            if (!_jumpEvent.IsNull)
            {

                FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(_jumpEvent);
                instance.setParameterByName("Material", GetSurfaceMaterial(hit));
                instance.setParameterByName("Speed", speed);
                instance.start();
                instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
                instance.release();
            }
        }

        public virtual void PlayLand(RaycastHit hit, float speed)
        {
            if (!_landEvent.IsNull)
            {
                FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(_landEvent);
                instance.setParameterByName("Material", GetSurfaceMaterial(hit));
                instance.setParameterByName("Speed", speed);
                instance.start();
                instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
                instance.release();
            }
        }

        // get surface material value from GameObject
        public float GetSurfaceMaterial(RaycastHit hit)
        {
            if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out FMODSurface surface) && surface.SurfaceMaterial != null) return surface.SurfaceMaterial.SurfaceValue;
            return 0f;
        }


    }
}