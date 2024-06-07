using Unity.Mathematics;
using UnityEngine;

namespace Prototype
{
    [System.Serializable]
    public struct QuaternionS
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public QuaternionS(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;

        }

        public static implicit operator quaternion(QuaternionS input)
        {
            return new quaternion(input.x, input.y, input.z, input.w);
        }

        public static implicit operator Quaternion(QuaternionS input)
        {
            return new Quaternion(input.x, input.y, input.z, input.w);
        }

        public static implicit operator QuaternionS(quaternion input)
        {
            return new QuaternionS(input.value.x, input.value.y, input.value.z, input.value.w);
        }

        public static implicit operator QuaternionS(Quaternion input)
        {
            return new QuaternionS(input.x, input.y, input.z, input.w);
        }
    }
}