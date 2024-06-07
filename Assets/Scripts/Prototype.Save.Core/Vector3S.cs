using Unity.Mathematics;
using UnityEngine;

namespace Prototype
{
    [System.Serializable]
    public struct Vector3S
    {
        public float x;
        public float y;
        public float z;

        public Vector3S(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator float3(Vector3S input)
        {
            return new float3(input.x, input.y, input.z);
        }

        public static implicit operator Vector3(Vector3S input)
        {
            return new Vector3(input.x, input.y, input.z);
        }

        public static implicit operator Vector3S(float3 input)
        {
            return new Vector3S(input.x, input.y, input.z);
        }

        public static implicit operator Vector3S(Vector3 input)
        {
            return new Vector3S(input.x, input.y, input.z);
        }
    }
}