using Unity.Mathematics;

namespace Voxelis.Constants
{
    public struct VoxelData
    {
        public const int ChunkSize = 16;

        public const int ChunkHeight = 16;

        public const int WorldSizeChunks = 20;

		public static int WorldSizeInBlocks => WorldSizeChunks * ChunkSize;

		public const int TextureAtlasSizeInBlocks = 16;

		public static readonly float3[] Vertices = new float3[8] 
		{
		    new float3(0, 0, 0),
		    new float3(1, 0, 0),
		    new float3(1, 1, 0),
		    new float3(0, 1, 0),
		    new float3(0, 0, 1),
		    new float3(1, 0, 1),
		    new float3(1, 1, 1),
		    new float3(0, 1, 1),
        	};

		public static readonly int4[] Triangles = new int4[6] 
		{
			new int4(0, 3, 1, 2),
			new int4(5, 6, 4, 7),
			new int4(3, 7, 2, 6),
			new int4(1, 5, 0, 4),
			new int4(4, 7, 0, 3),
			new int4(1, 2, 5, 6)
		};

		public static readonly int3[] Faces = new int3[6] 
		{
			new int3(0, 0, -1),
			new int3(0, 0, 1),
			new int3(0, 1, 0),
			new int3(0, -1, 0),
			new int3(-1, 0, 0),
			new int3(1, 0, 0)
		};

        	public static readonly int2[] Uvs = new int2[4] 
		{
			new int2 (0, 0),
			new int2 (0, 1),
			new int2 (1, 0),
			new int2 (1, 1)
		};
	}
}
