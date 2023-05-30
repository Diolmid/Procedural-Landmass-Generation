using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColourMap, Mesh}
    public DrawMode drawMode;

    [Space]
    [Range(0, 6)] public int levelOfDetail;
    public float noiseScale;
    private const int _mapChunkSize = 241;

    [Space]
    public int octaves;
    [Range(0, 1)] public float persistance;
    public float lacunarity;

    [Space]
    public int seed;
    public Vector2 offset;

    [Space]
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    [Space]
    public bool autoUpdate;

    public TerrainType[] regions;

    public void GenerateMap()
    {
        var noiseMap = Noise.GenerateNoiseMap(_mapChunkSize, _mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

        var colourMap = new Color[_mapChunkSize * _mapChunkSize];
        for(int y = 0; y < _mapChunkSize; y++)
        {
            for (int x = 0; x < _mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colourMap[y * _mapChunkSize + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        else if (drawMode == DrawMode.ColourMap)
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, _mapChunkSize, _mapChunkSize));
        else if(drawMode == DrawMode.Mesh)
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(colourMap, _mapChunkSize, _mapChunkSize));
    }

    private void OnValidate()
    {
        if(lacunarity < 1)
            lacunarity = 1;
        if(octaves < 0)
            octaves = 0;
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}