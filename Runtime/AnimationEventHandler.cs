using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODExtensions;

public class AnimationEventHandler : MonoBehaviour
{
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] FMODUnity.EventReference Footstep;
    [SerializeField] FMODUnity.EventReference Bodyfall;
    [SerializeField] FMODUnity.EventReference Land;
    [SerializeField] FMODUnity.EventReference Jump;
    [SerializeField] FMODUnity.EventReference FoliageMovement;

    public GameObject SurfaceObject { get; protected set; }


    public int numMeadowGrassVolumes = 0;

    Mouth mouth;
    private void Start()
    {
        mouth = GetComponent<Mouth>();
    }
    void PlayFootstep()
    {
        if (!Footstep.IsNull)
        {
            float materialIndex = GetMaterialBelow();
            FMOD.Studio.EventInstance footstep = FMODUnity.RuntimeManager.CreateInstance(Footstep);
            footstep.setParameterByName("material", materialIndex);
            footstep.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            footstep.start();
            footstep.release();

            mouth.EffortSpeech();
        }
    }

    void PlayBodyfall()
    {
        if (!Bodyfall.IsNull)
        {
            float materialIndex = GetMaterialBelow();
            FMOD.Studio.EventInstance bodyfall = FMODUnity.RuntimeManager.CreateInstance(Bodyfall);
            bodyfall.setParameterByName("material", materialIndex);
            bodyfall.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            bodyfall.start();
            bodyfall.release();
        }
    }

    void PlayLand()
    {
        if (!Land.IsNull)
        {
            float materialIndex = GetMaterialBelow();
            FMOD.Studio.EventInstance land = FMODUnity.RuntimeManager.CreateInstance(Land);
            land.setParameterByName("material", materialIndex);
            land.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            land.start();
            land.release();
        }
    }

    void PlayJump()
    {
        if (!Jump.IsNull)
        {
            float materialIndex = GetMaterialBelow();
            FMOD.Studio.EventInstance jump = FMODUnity.RuntimeManager.CreateInstance(Jump);
            jump.setParameterByName("material", materialIndex);
            jump.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            jump.start();
            jump.release();

            mouth.EffortSpeech();
        }
    }

    void StartStep()
    {
        if (!FoliageMovement.IsNull)
        {
            FMOD.Studio.EventInstance footstep = FMODUnity.RuntimeManager.CreateInstance(FoliageMovement);
            if (numMeadowGrassVolumes > 0)
            {
                footstep.setParameterByName("foliageType", 0);
            }
            else
            {
                footstep.setParameterByName("foliageType", 1);
            }
            footstep.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            footstep.start();
            footstep.release();
        }
        
    }

    float GetMaterialBelow()
    {

        //Create a variable to hold raycast result

        RaycastHit hit;

        //fire of a raycast directly under the player and look for the ground layer
        bool bHasHit = Physics.Raycast(transform.position + Vector3.up * 1.0f, Vector3.down, out hit, Mathf.Infinity, _groundMask);

        if (bHasHit)
        {
            SurfaceObject = hit.collider.gameObject;
            return GetSurfaceMaterial(SurfaceObject);
        }
        return 0f;
       
    }

    public float GetSurfaceMaterial(GameObject surfaceObject)
    {
        if (surfaceObject != null && surfaceObject.TryGetComponent(out FMODSurface surface) && surface.SurfaceMaterial != null) return surface.SurfaceMaterial.SurfaceValue;
        return 0f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "MeadowGrass")
        {
            numMeadowGrassVolumes++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "MeadowGrass")
        {
            numMeadowGrassVolumes--;
        }
    }
}

