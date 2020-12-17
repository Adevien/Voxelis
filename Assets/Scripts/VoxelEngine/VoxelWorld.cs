using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Voxelis.Constants;
using Voxelis.Components;
using Voxelis.Jobs;
using Unity.Mathematics;

namespace Voxelis
{
    public class VoxelWorld : MonoBehaviour
    {
        public int Seed = 0;

        public Material TextureData;

        public Blocks[] BlocksData;

        private ChunkData[,] Chunks = new ChunkData[VoxelData.WorldSizeChunks, VoxelData.WorldSizeChunks];

        private ChunkAllocatorJob CreateWorldChunksJob;

        private JobHandle JobHandler;

        protected void Start()
        {
            UnityEngine.Random.InitState(Seed);
            QualitySettings.vSyncCount = 0;
            InitializeWorldGrid();
        }

        void InitializeWorldGrid()
        {
            for (int WorldX = 0; WorldX < VoxelData.WorldSizeChunks; WorldX++)
            for (int WorldZ = 0; WorldZ < VoxelData.WorldSizeChunks; WorldZ++)
                CreateChunk(new int2(WorldX, WorldZ));
        }

        void CreateChunk(int2 coord)
        {
            CreateWorldChunksJob = new ChunkAllocatorJob()
            {
                GridPosition = coord,
                WorldPosition = new float3(coord.x * VoxelData.ChunkSize, 0, coord.y * VoxelData.ChunkSize),
                BlockTypes = new NativeArray<Blocks>(BlocksData, Allocator.TempJob),
                Voxels = new NativeArray<int>(VoxelData.ChunkSize * VoxelData.ChunkSize * VoxelData.ChunkHeight, Allocator.TempJob),
                Triangles = new NativeList<int>(Allocator.TempJob),
                Vertices = new NativeList<Vector3>(Allocator.TempJob),
                Uvs = new NativeList<Vector2>(Allocator.TempJob)
            };

            JobHandler = CreateWorldChunksJob.Schedule();
            JobHandler.Complete();

            Chunks[coord.x, coord.y] = new ChunkData
            {
                GridPosition = CreateWorldChunksJob.GridPosition,
                WorldPosition = CreateWorldChunksJob.WorldPosition,
                Voxels = CreateWorldChunksJob.Voxels.ToArray(),
                Vertices = CreateWorldChunksJob.Vertices.ToArray(),
                Triangles = CreateWorldChunksJob.Triangles.ToArray(),
                Uv = CreateWorldChunksJob.Uvs.ToArray()
            };

            CreateWorldChunksJob.BlockTypes.Dispose();
            CreateWorldChunksJob.Voxels.Dispose();
            CreateWorldChunksJob.Vertices.Dispose();
            CreateWorldChunksJob.Triangles.Dispose();
            CreateWorldChunksJob.Uvs.Dispose();

            CreateMesh(coord, Chunks[coord.x, coord.y].WorldPosition);
        }

        public void CreateMesh(int2 coords, float3 worldPos)
        {      
            GameObject Chunk = new GameObject($"{coords.x}-{coords.y}");
            Chunk.transform.position = worldPos;

            Mesh stockMesh = new Mesh
            {
                vertices = Chunks[coords.x, coords.y].Vertices,
                triangles = Chunks[coords.x, coords.y].Triangles,
                uv = Chunks[coords.x, coords.y].Uv
            };

            stockMesh.RecalculateNormals();
            stockMesh.Optimize();
            stockMesh.OptimizeIndexBuffers();
            stockMesh.OptimizeReorderVertexBuffer();
            stockMesh.MarkDynamic();

            Chunk.AddComponent<MeshRenderer>().material = TextureData;
            Chunk.AddComponent<MeshFilter>().mesh = stockMesh;
        }
    }
}