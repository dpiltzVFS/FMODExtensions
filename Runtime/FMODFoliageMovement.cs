//to do - support more than one layer
//to do - use parameterIDs instead of names
//to do - reduce FMODUpdate frequency
//to do - add support for on movement inside colliders
//to do - take height of details into account and stop playing sound when jumping

using FMODUnity;
using UnityEngine;
using UnityEngine.Playables;
namespace FMODExtensions
{
    public class FMODFoliageMovement : MonoBehaviour
    {
        [SerializeField] Terrain ThisTerrain;
        [SerializeField] EventReference FoliageMovement;
        
        TerrainData ThisTerrainData => ThisTerrain.terrainData;

        float[,,] _detailWeights;

        float[] _detailSurfaceMaterialIDs;

        FMOD.Studio.EventInstance _foliageMovement;

        Rigidbody _rb;

        int _amountOfDetailLayers;

        FMOD.Studio.PARAMETER_ID localPlayerSpeedParameterID;
        FMOD.Studio.PARAMETER_ID localPlayerGrassDensityParameterID;

        void Start()
        {
            _amountOfDetailLayers = ThisTerrainData.detailPrototypes.Length;
            _detailSurfaceMaterialIDs = new float[_amountOfDetailLayers];
            _detailWeights = new float[_amountOfDetailLayers, ThisTerrainData.detailWidth, ThisTerrainData.detailHeight];
            _rb = GetComponentInParent<Rigidbody>();
            GetDetailLayerData();
            GetDetailPrototypeData();
            InstancePersistantFMODEvents();
        }
        void Update()
        {
            
            GetWeightOfDetailLayers();
            UpdateFMODDetailParameters();
        }
        private void GetDetailLayerData()
        {
            int i, j, k;
            for (i = 0; i < ThisTerrainData.detailPrototypes.Length; i++)
            {
                int[,] layer = ThisTerrainData.GetDetailLayer(0, 0, ThisTerrainData.detailWidth, ThisTerrainData.detailHeight, i);

                for (j = 0; j < ThisTerrainData.detailWidth; j++)
                {
                    for (k = 0; k < ThisTerrainData.detailHeight; k++)
                    {
                        float pixelValue = layer[j, k];
                        _detailWeights[i, j, k] = pixelValue;
                    }
                }
            }
        }

        private void GetDetailPrototypeData()
        {
            for (int i = 0; i < _amountOfDetailLayers; i++)
            {
                _detailSurfaceMaterialIDs[i] = GetSurfaceMaterial(ThisTerrainData.detailPrototypes[i].prototype);
            }
        }
        public void InstancePersistantFMODEvents()
        {
            if (!FoliageMovement.IsNull)
            {
                _foliageMovement = RuntimeManager.CreateInstance(FoliageMovement);
                localPlayerSpeedParameterID = GetParameterID(_foliageMovement, "LocalPlayerSpeed");
                localPlayerGrassDensityParameterID = GetParameterID(_foliageMovement, "LocalPlayerGrassDensity");
                RuntimeManager.AttachInstanceToGameObject(_foliageMovement, gameObject, gameObject.GetComponent<Rigidbody>());
                _foliageMovement.setParameterByID(localPlayerGrassDensityParameterID, 0);
                _foliageMovement.start();
            }
        }

        public FMOD.Studio.PARAMETER_ID GetParameterID(FMOD.Studio.EventInstance FMODEvent, string parameterName)
        {
            FMOD.Studio.EventDescription eventDescription;
            FMODEvent.getDescription(out eventDescription);
            FMOD.Studio.PARAMETER_DESCRIPTION parameterDescription;
            eventDescription.getParameterDescriptionByName(parameterName, out parameterDescription);
            return parameterDescription.id;
        }

        Vector2Int ConvertToDetailmapCoordinates(Vector3 _worldPosition)
        {
            Vector3 relativePosition = _worldPosition - ThisTerrain.gameObject.transform.position;

            return new Vector2Int
            (
                x: Mathf.RoundToInt((relativePosition.x / ThisTerrainData.size.x) * ThisTerrainData.detailHeight),
                y: Mathf.RoundToInt((relativePosition.z / ThisTerrainData.size.z) * ThisTerrainData.detailWidth)
            );
        }

        void GetWeightOfDetailLayers()
        {
            int i = 0;
            float grassDensity = 0.0f;
            Vector2Int detailMapCoordinates = ConvertToDetailmapCoordinates(transform.position);
            for (i = 0; i < _amountOfDetailLayers; i++)
            {
                float layerWeight = _detailWeights[i, detailMapCoordinates.y, detailMapCoordinates.x];
                if (layerWeight > 0)
                {
                    if (_detailSurfaceMaterialIDs[i] == 0)
                    {
                        grassDensity += layerWeight;
                    }
                }

                _foliageMovement.setParameterByID(localPlayerGrassDensityParameterID, grassDensity);
                //Debug.Log("density = " + grassDensity);
            }
        }
        private void UpdateFMODDetailParameters()
        {
            _foliageMovement.setParameterByID(localPlayerSpeedParameterID, _rb.linearVelocity.magnitude);
        }

        public float GetSurfaceMaterial(GameObject surfaceObject)
        {
            if (surfaceObject != null && surfaceObject.TryGetComponent(out FMODSurface surface) && surface.SurfaceMaterial != null) return surface.SurfaceMaterial.SurfaceValue;
            return 0f;
        }

        private void OnDestroy()
        {
            _foliageMovement.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _foliageMovement.release();
        }
    }
}

