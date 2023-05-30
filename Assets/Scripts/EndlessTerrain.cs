using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewDistance = 450;
    public Transform viewer;

    public static Vector2 viewerPosition;
    private int _chunkSize;
    private int _chunkVisibleInViewDistance;

    private Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    private List<TerrainChunk> _terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    private void Start()
    {
        _chunkSize = MapGenerator.mapChunkSize - 1;
        _chunkVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / _chunkSize);
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunk();
    }

    private void UpdateVisibleChunk()
    {
        for (int i = 0; i < _terrainChunksVisibleLastUpdate.Count; i++)
            _terrainChunksVisibleLastUpdate[i].SetVisible(false);

        _terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / _chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / _chunkSize);

        for(int yOffset = -_chunkVisibleInViewDistance; yOffset <= _chunkVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -_chunkVisibleInViewDistance; xOffset <= _chunkVisibleInViewDistance; xOffset++)
            {
                var viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (_terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    _terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if (_terrainChunkDictionary[viewedChunkCoord].IsVisible())
                    {
                        _terrainChunksVisibleLastUpdate.Add(_terrainChunkDictionary[viewedChunkCoord]);
                    }
                }
                else
                    _terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, _chunkSize, transform)); ;
            }
        }
    }

    public class TerrainChunk
    {
        private GameObject _meshObject;
        private Vector2 _position;
        private Bounds _bounds;

        public TerrainChunk(Vector2 coord, int size, Transform parent)
        {
            _position = coord * size;
            _bounds = new Bounds(_position, Vector2.one * size);
            var positionV3 = new Vector3(_position.x, 0, _position.y);

            _meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _meshObject.transform.position = positionV3;
            _meshObject.transform.localScale = Vector3.one * size / 10f;
            _meshObject.transform.parent = parent;

            SetVisible(false);
        }

        public void UpdateTerrainChunk()
        {
            float viewerDistanceFormNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(viewerPosition));
            bool visible = viewerDistanceFormNearestEdge <= maxViewDistance;

            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            _meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return _meshObject.activeSelf;
        }
    }
}