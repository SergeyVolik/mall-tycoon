using Unity.Mathematics;
using UnityEngine;

namespace Prototype
{
    [System.Serializable]
    public struct Vector2S
    {
        public float x;
        public float y;

        public Vector2S(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator float2(Vector2S input)
        {
            return new float2(input.x, input.y);
        }

        public static implicit operator Vector2(Vector2S input)
        {
            return new Vector2(input.x, input.y);
        }

        public static implicit operator Vector2S(float2 input)
        {
            return new Vector2S(input.x, input.y);
        }

        public static implicit operator Vector2S(Vector2 input)
        {
            return new Vector2S(input.x, input.y);
        }
    }
}