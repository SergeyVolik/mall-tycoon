using Unity.Collections;
using UnityEngine;

namespace Prototype
{
    public static class PhysicsHelper
    {
        public static int GetAllTargetWithoutWalls(Transform castFrom, RaycastHit[] sphereCastHits, float castRadius, LayerMask sphereCastLayer, LayerMask wallLayer, float yOffset = 0)
        {
            var selfPos = castFrom.position;
            selfPos.y += yOffset;

            NativeList<RaycastHit> noWallHits = new NativeList<RaycastHit>(Allocator.Temp);

            var count = Physics.SphereCastNonAlloc(selfPos, castRadius, Vector3.up, sphereCastHits, float.MaxValue, sphereCastLayer);

            for (int i = 0; i < count; i++)
            {
                var targetPos = sphereCastHits[i].transform.position;
                targetPos.y += yOffset;
                var vector = targetPos - selfPos;

                if (!Physics.Raycast(selfPos, vector.normalized, castRadius, wallLayer))
                {
                    noWallHits.Add(sphereCastHits[i]);
                }
            }

            for (int i = 0; i < noWallHits.Length; i++)
            {
                sphereCastHits[i] = noWallHits[i];
            }

            return noWallHits.Length;
        }
    }
}