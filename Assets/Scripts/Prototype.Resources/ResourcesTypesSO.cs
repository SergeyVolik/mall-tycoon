using UnityEngine;

namespace Prototype
{
    public interface IResourceTypes
    {
        public ResourceTypeSO[] Types { get; }
    }

    [CreateAssetMenu]
    public class ResourcesTypesSO : ScriptableObject, IResourceTypes
    {
        public ResourceTypeSO[] Value;

        public ResourceTypeSO[] Types => Value;
    }
}
