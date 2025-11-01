using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TextureAtlas;

namespace FMODExtensions
{
    public class FMODFootstepsOnTerrain : FMODFootsteps
    {
        [SerializeField] Terrain thisTerrain;
        TerrainData ThisTerrainData => thisTerrain.terrainData;
        float[,,] _cachedTerrainAlphamapData;

        Dictionary<string, float> LayerNameToFMODMaterialIDMap = new Dictionary<string, float>();

        float FMODMaterialID;

        


        private void Start()
        {
            _cachedTerrainAlphamapData = ThisTerrainData.GetAlphamaps(0, 0, ThisTerrainData.alphamapWidth, ThisTerrainData.alphamapHeight);
            LayerNameToFMODMaterialIDMap.Add("FancyGrass", 6f);
            LayerNameToFMODMaterialIDMap.Add("Dirt", 8f);
            LayerNameToFMODMaterialIDMap.Add("2_Moss", 7f);
        }

        public override void PlayFootstep(RaycastHit hit, float speed)
        {
            float FMODMaterialID = 0;

            if (hit.collider.GeometryHolder.Type == UnityEngine.LowLevelPhysics.GeometryType.Terrain)
            {
                FMODMaterialID = GetDominantLayerIndex();
            }
            else
            {
                FMODMaterialID = GetSurfaceMaterial(hit);
            }
            FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(_footstepEvent);
            instance.setParameterByName("Material", FMODMaterialID);
            instance.setParameterByName("Speed", speed);
            instance.start();
            instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
            instance.release();
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
            LayerNameToFMODMaterialIDMap.TryGetValue(textureName, out float FMODMaterialID);
            Debug.Log("dominate layer = " + textureName);

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

