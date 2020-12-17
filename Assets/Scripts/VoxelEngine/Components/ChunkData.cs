using Unity.Mathematics;
using UnityEngine;

namespace Voxelis.Components
{
    public struct ChunkData
    {
        public int2 GridPosition;

        public float3 WorldPosition;

        public int[] Voxels;

        public Vector3[] Vertices;

        public int[] Triangles;

        public Vector2[] Uv;
    }
}
