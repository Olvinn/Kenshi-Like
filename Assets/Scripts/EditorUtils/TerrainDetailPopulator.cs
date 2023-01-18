using UnityEngine;
using UnityEngine.Serialization;

namespace EditorUtils
{
    public class TerrainDetailPopulator : MonoBehaviour
    {
        [SerializeField] private Terrain terrainToPopulate;
        [SerializeField] private int patchDetail;
        [SerializeField] private float minHeight, maxHeight;

        public void PopulateGrass()
        {
            var resolution = terrainToPopulate.terrainData.heightmapResolution;
            terrainToPopulate.terrainData.SetDetailResolution(resolution, patchDetail);
  
            int[,] newMap = new int[resolution, resolution];

            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    float height = terrainToPopulate.transform.position.y + terrainToPopulate.terrainData.GetHeight(j, i);
                    if (height > minHeight && height < maxHeight)
                        newMap[i, j] = patchDetail;
                }
            }
            terrainToPopulate.terrainData.SetDetailLayer(0, 0, 0, newMap);
        }
    }
}
