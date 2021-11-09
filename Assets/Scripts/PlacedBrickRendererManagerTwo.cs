using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Normal.Realtime.Utility;
using UnityEngine;
using UnityEngine.Rendering;


public class PlacedBrickRendererManagerTwo : MonoBehaviour
{
    private static PlacedBrickRendererManagerTwo _instance;
    public static PlacedBrickRendererManagerTwo GetInstance()
    {
        if (_instance == null)
        {
            _instance = GameObject.FindGameObjectWithTag("PlacedBrickRendererManager")
                ?.GetComponent<PlacedBrickRendererManagerTwo>();
        }

        return _instance;
    }
    public Material brickMaterial;

    // TODO: Some of these Dictionary caches probably don't get cleared, causing memory leaks.
    private RendererBufferRecycler _rendererBufferRecycler = new RendererBufferRecycler();
    private Dictionary<Mesh, MeshRendererQueue> _meshToRendererQueue = new Dictionary<Mesh, MeshRendererQueue>();
    private Dictionary<int, MeshRendererQueue> _objectHashCodesToRendererQueues = new Dictionary<int, MeshRendererQueue>();
    private Dictionary<MeshRendererQueue, Dictionary<int, bool>> _objectsToBeRemoved = new Dictionary<MeshRendererQueue, Dictionary<int, bool>>();
    private readonly Vector3 _brickScale = Vector3.one * 7;
    private static readonly int ColorKey = Shader.PropertyToID("_Color");

    private const int MaximumBatchSize = 1023;

    public bool rendering;

    public void AddBrick(int objectInstanceId, Transform t, Color color, Mesh mesh)
    {
        if (HasObjectWithInstanceId(objectInstanceId))
        {
            Debug.LogError("Skipping, brick already exists");
            return;
        }

        if (!_meshToRendererQueue.ContainsKey(mesh))
        {
            _meshToRendererQueue.Add(mesh, new MeshRendererQueue(mesh, brickMaterial, _rendererBufferRecycler));
            _objectsToBeRemoved.Add(_meshToRendererQueue[mesh], new Dictionary<int, bool>());
        }

        _meshToRendererQueue[mesh].AddMeshToRender(
            Matrix4x4.TRS(t.position, t.rotation, _brickScale),
            color.linear,
            objectInstanceId);

        _objectHashCodesToRendererQueues[objectInstanceId] = _meshToRendererQueue[mesh];
    }

    public void RemoveBrick(int objectInstanceId)
    {
        if (!HasObjectWithInstanceId(objectInstanceId))
        {
            // Debug.LogError("Skipping, brick does not exist");
            return;
        }

        MeshRendererQueue renderQueue = _objectHashCodesToRendererQueues[objectInstanceId];
        if (!_objectsToBeRemoved.ContainsKey(renderQueue))
            _objectsToBeRemoved[renderQueue] = new Dictionary<int, bool>();

        _objectsToBeRemoved[_objectHashCodesToRendererQueues[objectInstanceId]].Add(objectInstanceId, true);
        _objectHashCodesToRendererQueues.Remove(objectInstanceId);
    }

    public void RemoveBrickSynchronously(int objectInstanceId)
    {
        if (!HasObjectWithInstanceId(objectInstanceId))
            return;

        MeshRendererQueue renderQueue = _objectHashCodesToRendererQueues[objectInstanceId];
        renderQueue.RemoveObjectFromQueue(objectInstanceId);

        _objectsToBeRemoved[renderQueue].Clear();
        _objectHashCodesToRendererQueues.Remove(objectInstanceId);
    }

    public bool HasObjectWithInstanceId(int instanceId)
    {
        return _objectHashCodesToRendererQueues.ContainsKey(instanceId);
    }

    // Update is called once per frame
    void Update()
    {
        if (!rendering)
            return;

        // Debug.Log("Frame");

        foreach ((Mesh mesh, MeshRendererQueue renderQueue) in _meshToRendererQueue)
        {
            renderQueue.Render(_objectsToBeRemoved[renderQueue]);
            if(_objectsToBeRemoved[renderQueue].Count > 0)
                _objectsToBeRemoved[renderQueue].Clear();
        }
    }

    private class MeshRendererQueue
    {
        public Mesh Mesh;

        private readonly List<RenderQueueGroup> _renderQueueGroups = new List<RenderQueueGroup>();
        private readonly MaterialPropertyBlock _materialPropertyBlock = new MaterialPropertyBlock();
        private readonly Material _material;
        private readonly RendererBufferRecycler _rendererBufferRecycler;

        public MeshRendererQueue(Mesh mesh, Material material, RendererBufferRecycler rendererBufferRecycler)
        {
            Mesh = mesh;
            _material = material;
            _renderQueueGroups.Add(RenderQueueGroup.CreateNewRenderQueueGroup());
            _rendererBufferRecycler = rendererBufferRecycler;
        }

        public void AddMeshToRender(Matrix4x4 transformationMatrix, Vector4 color, int objectHashCode)
        {
            RenderQueueGroup lastRenderQueueGroup = _renderQueueGroups[_renderQueueGroups.Count - 1];
            if (lastRenderQueueGroup.HasMoreCapacity())
            {
                if(!lastRenderQueueGroup.AddObjectToRender(transformationMatrix, color, objectHashCode))
                    Debug.LogError($"Failed to add {objectHashCode} to render queue");
            }
            else
            {
                RenderQueueGroup newRenderQueueGroup = _rendererBufferRecycler.RentRenderQueueGroup();
                _renderQueueGroups.Add(newRenderQueueGroup);

                if(!newRenderQueueGroup.AddObjectToRender(transformationMatrix, color, objectHashCode))
                    Debug.LogError($"Failed to add {objectHashCode} to render queue");
            }
        }

        public void Render(Dictionary<int, bool> objectsToRemove)
        {
            if (objectsToRemove.Count > 0)
                CleanRenderQueue(objectsToRemove);

            foreach (RenderQueueGroup renderQueueGroup in _renderQueueGroups)
            {
                if(renderQueueGroup.Length > 0)
                    RenderNthGroup(renderQueueGroup);
            }
        }

        private void RenderNthGroup(RenderQueueGroup renderQueueGroup)
        {
            _materialPropertyBlock.SetVectorArray(ColorKey, renderQueueGroup.Colors);

            // Debug.Log($"Rendering {renderQueueGroup.Length} meshes");

            Graphics.DrawMeshInstanced(
                Mesh,
                0,
                _material,
                renderQueueGroup.TransformationMatrices,
                renderQueueGroup.Length,
                _materialPropertyBlock,
                ShadowCastingMode.Off,
                false,
                12,
                null,
                LightProbeUsage.BlendProbes);
        }

        public void RemoveObjectFromQueue(int objectInstanceId)
        {
            CleanRenderQueue(new Dictionary<int, bool>()
            {
                {objectInstanceId, true}
            });
        }

        private void CleanRenderQueue(Dictionary<int, bool> objectsToRemove)
        {
            RenderQueueGroup lastRenderQueueGroup = _renderQueueGroups.Last();

            for(int j = 0; j < _renderQueueGroups.Count; j++)
            {
                RenderQueueGroup renderQueueGroup = _renderQueueGroups[j];
                for (int i = 0; i < renderQueueGroup.Length; i++)
                {
                    if (!objectsToRemove.ContainsKey(renderQueueGroup.ObjectHashCodes[i]))
                    {
                        continue;
                    }

                    if (lastRenderQueueGroup.Length == 0)
                        return;

                    (Matrix4x4 matrix, Vector4 color, int objectHashCode) = lastRenderQueueGroup.Pop();
                    // Debug.Log("Removed a mesh");

                    while (objectsToRemove.ContainsKey(objectHashCode))
                    {
                        if (objectHashCode == renderQueueGroup.ObjectHashCodes[i])
                        {
                            return;
                        }

                        if (lastRenderQueueGroup.Length == 0)
                            return;

                        (matrix, color, objectHashCode) = lastRenderQueueGroup.Pop();
                    }

                    // Replace row with mesh from the end of the queue
                    renderQueueGroup.TransformationMatrices[i] = matrix;
                    renderQueueGroup.Colors[i] = color;
                    renderQueueGroup.ObjectHashCodes[i] = objectHashCode;

                    if (lastRenderQueueGroup.Length == 0)
                    {
                        _renderQueueGroups.Remove(lastRenderQueueGroup);
                        _rendererBufferRecycler.ReturnRenderQueueGroupToPool(lastRenderQueueGroup);
                        if (_renderQueueGroups.Count == 0) return;

                        lastRenderQueueGroup = _renderQueueGroups.Last();
                    }
                }
            }
        }
    }

    private class RendererBufferRecycler
    {
        private readonly List<RenderQueueGroup> _availableRenderQueueGroups = new List<RenderQueueGroup>();

        public RendererBufferRecycler()
        {
            _availableRenderQueueGroups.Add(RenderQueueGroup.CreateNewRenderQueueGroup());
        }

        public RenderQueueGroup RentRenderQueueGroup()
        {
            if (_availableRenderQueueGroups.Count > 0)
            {
                RenderQueueGroup renderQueueGroup = _availableRenderQueueGroups[_availableRenderQueueGroups.Count - 1];

                _availableRenderQueueGroups.RemoveAt(_availableRenderQueueGroups.Count - 1);

                return renderQueueGroup;
            }

            return RenderQueueGroup.CreateNewRenderQueueGroup();
        }

        public void ReturnRenderQueueGroupToPool(RenderQueueGroup renderQueueGroup)
        {
            renderQueueGroup.Length = 0;
            _availableRenderQueueGroups.Add(renderQueueGroup);
        }
    }

    private class RenderQueueGroup
    {
        public Matrix4x4[] TransformationMatrices;
        public Vector4[] Colors;
        public int[] ObjectHashCodes;
        public int Length;

        public bool AddObjectToRender(Matrix4x4 transformationMatrix, Vector4 color, int objectHashCode)
        {
            if (!HasMoreCapacity())
                return false;

            TransformationMatrices[Length] = transformationMatrix;
            Colors[Length] = color;
            ObjectHashCodes[Length] = objectHashCode;

            Length += 1;

            return true;
        }

        public bool HasMoreCapacity()
        {
            return Length < MaximumBatchSize;
        }

        public (Matrix4x4, Vector4, int) Pop()
        {
            (Matrix4x4, Vector4, int) values = (TransformationMatrices[Length-1], Colors[Length-1], ObjectHashCodes[Length-1]);
            Length -= 1;

            return values;
        }

        public static RenderQueueGroup CreateNewRenderQueueGroup()
        {
            return new RenderQueueGroup
            {
                TransformationMatrices = new Matrix4x4[MaximumBatchSize],
                Colors = new Vector4[MaximumBatchSize],
                ObjectHashCodes = new int[MaximumBatchSize],
                Length = 0
            };
        }
    }
}
