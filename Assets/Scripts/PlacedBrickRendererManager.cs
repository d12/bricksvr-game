using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;
using System.Linq;

// Note that the PlacedBrickRendererManager does not render pegs automatically.
// Pegs must be passed in using AddBrick, and removed using RemoveBrick.
public class PlacedBrickRendererManager : MonoBehaviour
{
    private static PlacedBrickRendererManager _instance;
    public static PlacedBrickRendererManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = GameObject.FindGameObjectWithTag("PlacedBrickRendererManager")
                ?.GetComponent<PlacedBrickRendererManager>();
        }

        return _instance;
    }

    public Material brickMaterial;

    private readonly Dictionary<Mesh, MeshRenderBucket> _meshToRenderBuckets = new Dictionary<Mesh, MeshRenderBucket>();
    private readonly Dictionary<string, LinkedListNode<MeshToRender>> _brickUuidToNodes = new Dictionary<string, LinkedListNode<MeshToRender>>();
    private readonly Dictionary<string, Mesh> _brickUuidToMesh = new Dictionary<string, Mesh>();
    private readonly Dictionary<Mesh, MeshToRender[][]> _groupedBrickBucketCache = new Dictionary<Mesh, MeshToRender[][]>();

    private MaterialPropertyBlock _materialPropertyBlock;
    private static readonly int ColorsKey = Shader.PropertyToID("_Color");
    private readonly Vector3 _brickScale = Vector3.one * 7;
    private const int MaximumBatchSize = 1023;

    public bool renderingEnabled;

    private void Awake()
    {
        _materialPropertyBlock = new MaterialPropertyBlock();
    }

    // Adding to the render system is a few steps:
    // 1. Build the Brick class which includes the transformation matrix. We depend on placed bricks not moving.
    // 2. If we haven't made a render bucket for the mesh yet, create an empty bucket
    // 3. Add the Brick class to the bucket
    // 4. Add a mapping to the linked list node in the render bucket to _brickUuidToNode, to allow us to lookup bucket nodes in constant time for deletion.
    public void AddBrick(string uuid, Transform t, Color color, Mesh mesh)
    {
        AddBrick(uuid, t.position, t.rotation, color, mesh);
    }

    public void AddBrick(string uuid, Vector3 pos, Quaternion rot, Color color, Mesh mesh)
    {
        MeshToRender meshToRender = new MeshToRender
        (
            Matrix4x4.TRS(pos, rot, _brickScale),
            color
        );

        if (!_meshToRenderBuckets.ContainsKey(mesh))
            _meshToRenderBuckets.Add(mesh, new MeshRenderBucket(mesh));

        LinkedListNode<MeshToRender> brickNode = _meshToRenderBuckets[mesh].AddMeshToRender(meshToRender);
        _brickUuidToNodes[uuid] = brickNode;
        _brickUuidToMesh[uuid] = mesh;

        _groupedBrickBucketCache.Remove(mesh);
    }

    // To remove a node:
    // 1. Lookup node in _brickUuidToNode dict
    // 2. Delete node from render bucket linked list
    // 3. Remove dict entry in _brickUuidToNode
    public void RemoveBrick(string uuid)
    {
        if (!_brickUuidToNodes.TryGetValue(uuid, out LinkedListNode<MeshToRender> brickNode))
            return;

        _groupedBrickBucketCache.Remove(_brickUuidToMesh[uuid]);

        brickNode.List.Remove(brickNode);
        _brickUuidToNodes.Remove(uuid);
        _brickUuidToMesh.Remove(uuid);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!renderingEnabled) return;

        foreach ((Mesh key, MeshRenderBucket renderBucket) in _meshToRenderBuckets.Select(x => (x.Key, x.Value)))
        {
            PerformBucketRender(key, renderBucket);
        }
    }

    // Renders one bucket for one frame.
    // Breaks a bucket into groups of 1024 first, since that's the cap for one DrawMeshInstanced call
    private void PerformBucketRender(Mesh mesh, MeshRenderBucket renderBucket)
    {
        if (renderBucket.Count == 0) return;

        foreach (MeshRenderBucketGroup meshRenderBucketGroup in renderBucket.MeshRenderBucketGroups())
        {
            RenderBucketGroup(meshRenderBucketGroup);
        }
    }

    // Warning: We don't clear these buffers, so make sure you pass in the correct count to DrawMeshInstanced.
    // private readonly Matrix4x4[] _transformationMatrices = new Matrix4x4[MaximumBatchSize];
    private void RenderBucketGroup(MeshRenderBucketGroup renderBucketGroup)
    {
        _materialPropertyBlock.SetVectorArray(ColorsKey, renderBucketGroup.Colors);

        Graphics.DrawMeshInstanced(
            renderBucketGroup.Bucket.Mesh,
            0,
            brickMaterial,
            renderBucketGroup.TransformationMatrices,
            renderBucketGroup.Count,
            _materialPropertyBlock,
            ShadowCastingMode.Off,
            false,
            12,
            null,
            LightProbeUsage.BlendProbes);
    }

    private class MeshToRender
    {
        public MeshToRender(Matrix4x4 transformationMatrix, Color color)
        {
            TransformationMatrix = transformationMatrix;
            Color = color.linear;
        }

        // Vector4 must be in linear srgb color space, otherwise colors will be off
        public MeshToRender(Matrix4x4 transformationMatrix, Vector4 color)
        {
            TransformationMatrix = transformationMatrix;
            Color = color;
        }

        public readonly Matrix4x4 TransformationMatrix;
        public readonly Vector4 Color;
    }

    private class MeshRenderBucket
    {
        public Mesh Mesh;
        private LinkedList<MeshToRender> MeshesToRender;
        private MeshRenderBucketGroup[] _meshRenderBucketGroups;
        private bool _dirty = true;
        public MeshRenderBucket(Mesh mesh)
        {
            MeshesToRender = new LinkedList<MeshToRender>();
            Mesh = mesh;
        }

        public int Count => MeshesToRender.Count;

        public LinkedListNode<MeshToRender> AddMeshToRender(MeshToRender meshToRender)
        {
            _dirty = true;
            return MeshesToRender.AddFirst(meshToRender);
        }

        public MeshRenderBucketGroup[] MeshRenderBucketGroups()
        {
            if (_dirty)
                CalculateMeshRenderBucketGroups(this);

            _dirty = false;
            return _meshRenderBucketGroups;
        }

        private void CalculateMeshRenderBucketGroups(MeshRenderBucket bucket)
        {
            int bucketGroupSizes = Mathf.Min(MaximumBatchSize, MeshesToRender.Count);
            int numberOfGroups = (MeshesToRender.Count - 1) / bucketGroupSizes + 1;

            MeshRenderBucketGroup[] brickBucketGroups = new MeshRenderBucketGroup[numberOfGroups];
            for (int i = 0; i < brickBucketGroups.Length; i++)
            {
                brickBucketGroups[i] = new MeshRenderBucketGroup(this);
            }

            LinkedListNode<MeshToRender> current = MeshesToRender.First;
            int j = 0;
            while (current != null)
            {
                brickBucketGroups[j / bucketGroupSizes].AddMeshToRender(current.Value);

                j += 1;
                current = current.Next;
            }
        }
    }

    private class MeshRenderBucketGroup
    {
        public readonly MeshRenderBucket Bucket;
        public readonly Matrix4x4[] TransformationMatrices;
        public readonly Vector4[] Colors;
        private int _index = 0;

        public int Count => _index;

        public MeshRenderBucketGroup(MeshRenderBucket bucket)
        {
            _index = 0;
            Bucket = bucket;
            TransformationMatrices = new Matrix4x4[MaximumBatchSize];
            Colors = new Vector4[MaximumBatchSize];
        }

        public void AddMeshToRender(MeshToRender meshToRender)
        {
            TransformationMatrices[_index] = meshToRender.TransformationMatrix;
            Colors[_index] = meshToRender.Color;
            _index += 1;
        }
    }
}
