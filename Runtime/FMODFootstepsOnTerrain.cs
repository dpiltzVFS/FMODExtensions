using System.Collections.Generic;
using UnityEngine;

namespace FMODExtensions
{
    public class FMODFootstepsOnTerrain : FMODFootsteps
    {
        [SerializeField] Terrain thisTerrain;
        TerrainData ThisTerrainData => thisTerrain.terrainData;
        float[,,] _cachedTerrainAlphamapData;

        public FMODStringToIDMap _TerrianLayerNameToFMODMaterialIDMap;

        private void Start()
        {
            _cachedTerrainAlphamapData = ThisTerrainData.GetAlphamaps(0, 0, ThisTerrainData.alphamapWidth, ThisTerrainData.alphamapHeight);
        }

        public override void PlayFootstep(GameObject floorObject, bool overTerrain, float speed)
        {
            if(!_footstepEvent.IsNull)
            {
                float FMODMaterialIndex = 0;

                if (overTerrain)
                {
                    FMODMaterialIndex = GetDominantLayerIndex();
                }
                else
                {
                    FMODMaterialIndex = GetSurfaceMaterial(floorObject);
                }
                FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(_footstepEvent);
                instance.setParameterByName("Material", FMODMaterialIndex);
                instance.setParameterByName("Speed", speed);
                instance.start();
                instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
                instance.release();
            }
            
        }

        public override void PlayLand(GameObject floorObject, bool overTerrain, float speed)
        {
            if (!_landEvent.IsNull)
            {
                float FMODMaterialID = 0;

                if (overTerrain)
                {
                    FMODMaterialID = GetDominantLayerIndex();
                }
                else
                {
                    FMODMaterialID = GetSurfaceMaterial(floorObject);
                }
                FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(_landEvent);
                instance.setParameterByName("Material", FMODMaterialID);
                instance.setParameterByName("Speed", speed);
                instance.start();
                instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
                instance.release();
            }
        }

        public float GetDominantLayerIndex()
        {
            Vector2Int alphamapCoordinates = ConvertToAlphamapCoordinates(transform.position);

            int mostDominantLayerIndex = 0;
            float greatestLayerWeight = float.MinValue;
            int LayerCount = _cachedTerrainAlphamapData.GetLength(2);

            for (int LayerIndex = 0; LayerIndex < LayerCount; LayerIndex++)
            {
                float LayerWeight = _cachedTerrainAlphamapData[alphamapCoordinates.y, alphamapCoordinates.x, LayerIndex];

                if (LayerWeight > greatestLayerWeight)
                {
                    greatestLayerWeight = LayerWeight;
                    mostDominantLayerIndex = LayerIndex;
                }
            }
            string textureName = ThisTerrainData.terrainLayers[mostDominantLayerIndex].name;
            _TerrianLayerNameToFMODMaterialIDMap.map.TryGetValue(textureName, out float FMODMaterialID);
            return FMODMaterialID;
        }

        private Vector2Int ConvertToAlphamapCoordinates(Vector3 worldPosition)
        {
            Vector3 relativePosition = worldPosition - thisTerrain.gameObject.transform.position;

            return new Vector2Int
            (
                x: Mathf.RoundToInt((relativePosition.x / ThisTerrainData.size.x) * ThisTerrainData.alphamapWidth),
                y: Mathf.RoundToInt((relativePosition.z / ThisTerrainData.size.z) * ThisTerrainData.alphamapHeight)
            );
        }
    }
}

