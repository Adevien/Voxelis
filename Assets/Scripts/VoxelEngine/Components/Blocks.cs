using System;
using UnityEngine;

namespace Voxelis.Components
{
    [Serializable]
    public struct Blocks
    {
        public bool Solid;
        public int Back;
        public int Front;
        public int Top;
        public int Bottom;
        public int Left;
        public int Right;

        public int GetTexture(int faceIndex)
        {
            switch (faceIndex)
            {
                case 0: return Back;
                case 1: return Front;
                case 2: return Top;
                case 3: return Bottom;
                case 4: return Left;
                case 5: return Right;
                default: Debug.Log("Invalid face index"); return 0;
            }
        }
    }
}
