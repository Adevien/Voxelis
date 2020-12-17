using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Voxelis.Components;
using Voxelis.Constants;

namespace Voxelis.Jobs
{
    [BurstCompile]
    public struct ChunkAllocatorJob : IJob
    {
        public int2 GridPosition;

        public float3 WorldPosition;

        public NativeArray<int> Voxels;

        public NativeList<Vector3> Vertices;

        public NativeList<int> Triangles;

        public NativeList<Vector2> Uvs;

        public NativeArray<Blocks> BlockTypes;

        public void Execute()
        {
            int vertexIndex = 0;

            for (int y = 0; y < VoxelData.ChunkHeight;  y++)
            for (int x = 0; x < VoxelData.ChunkSize;    x++)
            for (int z = 0; z < VoxelData.ChunkSize;    z++)
            {
                int3 index = new int3(x, y, z);
                int flatIndex = To1D(index);

                Voxels[flatIndex] = GetVoxel(index + WorldPosition);

                for (int p = 0; p < 6; p++)
                    if (!CheckVoxel(index + VoxelData.Faces[p], flatIndex))
                    {
                        Vertices.Add(index + VoxelData.Vertices[VoxelData.Triangles[p][0]]);
                        Vertices.Add(index + VoxelData.Vertices[VoxelData.Triangles[p][1]]);
                        Vertices.Add(index + VoxelData.Vertices[VoxelData.Triangles[p][2]]);
                        Vertices.Add(index + VoxelData.Vertices[VoxelData.Triangles[p][3]]);

                        Triangles.Add(vertexIndex);
                        Triangles.Add(vertexIndex + 1);
                        Triangles.Add(vertexIndex + 2);
                        Triangles.Add(vertexIndex + 2);
                        Triangles.Add(vertexIndex + 1);
                        Triangles.Add(vertexIndex + 3);

                        vertexIndex += 4;

                        int TextureId = BlockTypes[Voxels[flatIndex]].GetTexture(p);

                        float textureY = TextureId / VoxelData.TextureAtlasSizeInBlocks;
                        float textureX = TextureId - (textureY * VoxelData.TextureAtlasSizeInBlocks);

                        float normalizedTexture = 1f / VoxelData.TextureAtlasSizeInBlocks;

                        textureX *= normalizedTexture;
                        textureY *= normalizedTexture;
                        textureY = 1f - textureY - normalizedTexture;

                        Uvs.Add(new Vector2(textureX, textureY));
                        Uvs.Add(new Vector2(textureX, textureY + normalizedTexture));
                        Uvs.Add(new Vector2(textureX + normalizedTexture, textureY));
                        Uvs.Add(new Vector2(textureX + normalizedTexture, textureY + normalizedTexture));
                    }  
            }
        }

        private int GetVoxel(float3 pos)
        {
            if (pos.x < 0
                || pos.x > VoxelData.WorldSizeInBlocks - 1
                || pos.y < 0
                || pos.y > VoxelData.ChunkHeight - 1
                || pos.z < 0
                || pos.z > VoxelData.WorldSizeInBlocks - 1)
                return 0;
            return pos.y < 1 ? 1 : pos.y == VoxelData.ChunkHeight - 1 ? 3 : 2;
        }

        private bool CheckVoxel(int3 pos, int index) => !VoxelInChunk(pos) ? BlockTypes[GetVoxel(pos + WorldPosition)].Solid : BlockTypes[Voxels[index]].Solid;

        private bool VoxelInChunk(int3 pos) => pos.x >= 0 && pos.x <= VoxelData.ChunkSize - 1 && pos.y >= 0 && pos.y <= VoxelData.ChunkHeight - 1 && pos.z >= 0 && pos.z <= VoxelData.ChunkSize - 1;

        private int To1D(int3 pos) => (pos.z * VoxelData.ChunkSize * VoxelData.ChunkHeight) + (pos.y * VoxelData.ChunkSize) + pos.x;        
    }
}

