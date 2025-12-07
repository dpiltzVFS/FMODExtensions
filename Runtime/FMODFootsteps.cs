using UnityEngine;

namespace FMODExtensions
{
    public class FMODFootsteps : MonoBehaviour
    {
        [SerializeField] public FMODUnity.EventReference _footstepEvent;
        [SerializeField] public FMODUnity.EventReference _jumpEvent;
        [SerializeField] public FMODUnity.EventReference _landEvent;

        public virtual void PlayFootstep(GameObject floorObject, bool overTerrain, float speed)
        {
            if (!_footstepEvent.IsNull)
            {
                FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(_footstepEvent);
                instance.setParameterByName("Material", GetSurfaceMaterial(floorObject));
                instance.setParameterByName("Speed", speed);
                instance.start();
                instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
                instance.release();
            }
        }

        public virtual void PlayJump(GameObject floorObject, bool overTerrain, float speed)
        {
            if (!_jumpEvent.IsNull)
            {

                FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(_jumpEvent);
                instance.setParameterByName("Material", GetSurfaceMaterial(floorObject));
                instance.setParameterByName("Speed", speed);
                instance.start();
                instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
                instance.release();
            }
        }

        public virtual void PlayLand(GameObject floorObject, bool overTerrain, float speed)
        {
            if (!_landEvent.IsNull)
            {
                FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(_landEvent);
                instance.setParameterByName("Material", GetSurfaceMaterial(floorObject));
                instance.setParameterByName("Speed", speed);
                instance.start();
                instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
                instance.release();
            }
        }

        public float GetSurfaceMaterial(GameObject floorObject)
        {
            if (floorObject.TryGetComponent(out FMODSurface surface) && surface.SurfaceMaterial != null) return surface.SurfaceMaterial.SurfaceValue;
            return 0f;
        }
    }
}