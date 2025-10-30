using UnityEngine;

namespace FMODExtensions
{
    public class FMODFootstepsOnTerrain : FMODFootsteps
    {
        [SerializeField] Terrain thisTerrain;
        TerrainData ThisTerrainData => thisTerrain.terrainData;
        float[,,] _cachedTerrainAlphamapData;

        


        private void Start()
        {
            _cachedTerrainAlphamapData = ThisTerrainData.GetAlphamaps(0, 0, ThisTerrainData.alphamapWidth, ThisTerrainData.alphamapHeight);
        }

        public override void PlayFootstep(RaycastHit hit, float speed)
        {
            float fmodSurfaceMaterialID = 0;

            if (hit.collider.GeometryHolder.Type == UnityEngine.LowLevelPhysics.GeometryType.Terrain)
            {
                fmodSurfaceMaterialID = GetDominantLayerIndex();
            }
            else
            {
                fmodSurfaceMaterialID = GetSurfaceMaterial(hit);
            }
            FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(_footstepEvent);
            instance.setParameterByName("Material", fmodSurfaceMaterialID);
            instance.setParameterByName("Speed", speed);
            instance.start();
            instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
            instance.release();
        }

        public int GetDominantLayerIndex()
        {
            Vector2Int alphamapCoordinates = ConvertToAlphamapCoordinates(transform.position);

            int mostDominantTextureIndex = 0;
            float greatestTextureWeight = float.MinValue;
            int textureCount = _cachedTerrainAlphamapData.GetLength(2);

            for (int textureIndex = 0; textureIndex < textureCount; textureIndex++)
            {
                float textureWeight = _cachedTerrainAlphamapData[alphamapCoordinates.y, alphamapCoordinates.x, textureIndex];

                if (textureWeight > greatestTextureWeight)
                {
                    greatestTextureWeight = textureWeight;
                    mostDominantTextureIndex = textureIndex;
                }
            }
            return mostDominantTextureIndex;
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

