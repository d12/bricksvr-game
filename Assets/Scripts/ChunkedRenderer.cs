using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Normal.Realtime.Utility;
using UnityEngine;

public class ChunkedRenderer : MonoBehaviour
{
    public Transform chunkingCenterPointTransform;
    private Vector3 _chunkingCenterPoint;

    public int renderDistance;
    private bool _uncappedRenderDistance = false;
    public float chunkSize;

    // Large players should keep more brick colliders loaded, otherwise they won't be able to place bricks
    public float playerScale = 1f;

    public Transform playerHeadTransform;

    private readonly Dictionary<Coord, Chunk> _chunkDictionary = new Dictionary<Coord, Chunk>();

    // A queue of work to perform asynchronously. This work is spread over multiple frames.
    private readonly Queue<SetConnectorStatesTask> _workQueue = new Queue<SetConnectorStatesTask>();
    private const int WorkQueueItemsToProcessPerFrame = 30;

    #region GetInstance

    private static ChunkedRenderer _instance;

    public static ChunkedRenderer GetInstance()
    {
        if (_instance == null)
            _instance = GameObject.FindGameObjectWithTag("ChunkedRenderer").GetComponent<ChunkedRenderer>();

        return _instance;
    }

    #endregion

    private void Awake()
    {
        _chunkingCenterPoint = chunkingCenterPointTransform.position;
    }

    public void RenderDistanceSet(int distance)
    {
        renderDistance = distance;
        _uncappedRenderDistance = distance == 21;
        RedoChunks();
    }

    public void AddBrickToRenderer(GameObject brickObject)
    {
        Chunk chunk = FindOrCreateChunk(GetChunkFromPosition(brickObject.transform.position));
        chunk.AddBrick(brickObject, _workQueue);
    }

    private void AddBrickToRenderer(ChunkedBrick chunkedBrick)
    {
        Chunk chunk = FindOrCreateChunk(GetChunkFromPosition(chunkedBrick.GameObject.transform.position));
        chunk.AddBrick(chunkedBrick, _workQueue);
    }

    private Chunk FindOrCreateChunk(Coord coord)
    {
        if (!_chunkDictionary.ContainsKey(coord))
            _chunkDictionary[coord] = new Chunk { Position = GetCenterOfChunk(coord) };


        return _chunkDictionary[coord];
    }

    private void Update()
    {
        Vector3 playerPosition = playerHeadTransform.position;
        playerPosition.y = 0;

        Coord currentChunk = GetChunkFromPosition(playerPosition);

        foreach ((Coord coord, Chunk chunk) in _chunkDictionary)
        {
            // Enable renderers within our render distance (which is defined in actual distance, not # of chunks)
            chunk.SetRendererEnabled(_uncappedRenderDistance || (Vector3.Distance(playerPosition, chunk.Position) < renderDistance));

            // Enable connector colliders within our chunk
            chunk.SetConnectorCollidersEnabled(coord.WithinNChunks(currentChunk, (int) Math.Ceiling(playerScale)), _workQueue);
        }

        WorkThroughWorkQueue();
    }

    private void WorkThroughWorkQueue()
    {
        for (int i = 0; i < WorkQueueItemsToProcessPerFrame; i++)
        {
            if (_workQueue.Count == 0) return;

            SetConnectorStatesTask task = _workQueue.Dequeue();

            if(task.ChunkedBrick.GameObject != null)
                foreach (LegoConnectorScript connectorScript in task.ChunkedBrick.ConnectorScripts)
                    connectorScript.SetOutOfRenderDistance(!task.Enabled);
        }
    }

    private void RedoChunks()
    {
        List<ChunkedBrick> bricks = new List<ChunkedBrick>();
        foreach (Chunk chunk in _chunkDictionary.Values)
        {
            bricks.AddRange(chunk.Bricks());
        }

        _chunkDictionary.Clear();

        foreach (ChunkedBrick chunkedBrick in bricks)
        {
            AddBrickToRenderer(chunkedBrick);
        }
    }

    #region Gizomos
    private void OnDrawGizmos()
    {
        for (int i = 0; i < renderDistance; i++)
        {
            for (int j = 0; j < renderDistance; j++)
            {
                Gizmos.DrawWireCube(GetCenterOfChunk(new Coord(i, j)), new Vector3(chunkSize, 5, chunkSize));
            }
        }
    }
    #endregion

    #region Helpers

    private Vector3 GetCenterOfChunk(Coord coord)
    {
        return new Vector3(_chunkingCenterPoint.x + ((coord.x + 0.5f) * chunkSize) - (renderDistance / 2f * chunkSize), 0,
            _chunkingCenterPoint.z + ((coord.z + 0.5f) * chunkSize) - (renderDistance / 2f * chunkSize));
    }

    private Coord GetChunkFromPosition(Vector3 position)
    {
        Vector3 relativePosition = position - _chunkingCenterPoint;
        return new Coord((int)((relativePosition.x / chunkSize) + (renderDistance / 2f)),
            (int)((relativePosition.z / chunkSize) + (renderDistance / 2f)));
    }

    private static float DistanceBetweenCoords(Coord one, Coord two)
    {
        return Mathf.Sqrt((two.x - one.x) ^ 2 + (two.z - one.z) ^ 2);
    }

    #endregion

    #region Structs
    private struct Coord
    {
        public int x;
        public int z;

        public Coord(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public bool Equals(Coord otherCoord)
        {
            return x == otherCoord.x && z == otherCoord.z;
        }

        public bool WithinNChunks(Coord otherCoord, int n)
        {
            return (Mathf.Abs(x - otherCoord.x) <= n) && (Mathf.Abs(z - otherCoord.z) <= n);
        }
    }
    #endregion

    private class Chunk
    {
        private readonly List<ChunkedBrick> _bricks = new List<ChunkedBrick>();
        private bool _renderersEnabled = true;
        private bool _connectorsEnabled = true;

        public Vector3 Position;

        public void AddBrick(GameObject brickObject, Queue<SetConnectorStatesTask> workQueue)
        {
            AddBrick(new ChunkedBrick(brickObject), workQueue);
        }

        public void AddBrick(ChunkedBrick chunkedBrick, Queue<SetConnectorStatesTask> workQueue)
        {
            _bricks.Add(chunkedBrick);

            chunkedBrick.MeshRenderer.enabled = _renderersEnabled;
            workQueue.Enqueue(new SetConnectorStatesTask(chunkedBrick, _connectorsEnabled));
        }

        // Toggling the renderers is super fast, so no need to throw these jobs on the work queue.
        public void SetRendererEnabled(bool enabled)
        {
            if (_renderersEnabled == enabled) return; // No work to do, renderers are already in the correct state
            _renderersEnabled = enabled;

            for (int i = _bricks.Count - 1; i >= 0; i--)
            {
                ChunkedBrick brick = _bricks[i];
                if (brick.GameObject == null)
                {
                    _bricks.RemoveAt(i);
                }
                else
                {
                    brick.MeshRenderer.enabled = enabled;
                }
            }
        }

        public void SetConnectorCollidersEnabled(bool enabled, Queue<SetConnectorStatesTask> workQueue)
        {
            if (_connectorsEnabled == enabled) return; // No work to do, connectors are already in the correct state
            _connectorsEnabled = enabled;

            for (int i = _bricks.Count - 1; i >= 0; i--)
            {
                if (_bricks[i].GameObject == null)
                {
                    _bricks.RemoveAt(i);
                }
                else
                {
                    workQueue.Enqueue(new SetConnectorStatesTask(_bricks[i], enabled));
                }
            }
        }

        public List<ChunkedBrick> Bricks()
        {
            return _bricks;
        }
    }
}

public readonly struct ChunkedBrick
{
    public readonly GameObject GameObject;
    public readonly MeshRenderer MeshRenderer;
    public readonly int VertexCount;
    public readonly List<LegoConnectorScript> ConnectorScripts;

    public ChunkedBrick(GameObject o)
    {
        GameObject = o;

        BrickAttach attach = o.GetComponent<BrickAttach>();
        MeshRenderer = attach.meshRenderer;
        VertexCount = attach.meshFilter.sharedMesh.vertexCount;
        ConnectorScripts = attach.maleConnectorScripts.Concat(attach.femaleConnectorScripts).ToList();
    }
}

public readonly struct SetConnectorStatesTask
{
    public readonly ChunkedBrick ChunkedBrick;
    public readonly bool Enabled;

    public SetConnectorStatesTask(ChunkedBrick brick, bool enabled)
    {
        ChunkedBrick = brick;
        Enabled = enabled;
    }
}


