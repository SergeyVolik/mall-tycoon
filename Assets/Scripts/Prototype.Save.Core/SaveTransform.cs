using Newtonsoft.Json;
using UnityEngine;

namespace Prototype
{
    [System.Serializable]
    public class TransformSave : ISaveComponentData
    {
        public Vector3S position;
        public QuaternionS rotation;
        public Vector3S localScale;
        public SerializableGuid SaveId { get; set; }
    }

    [DisallowMultipleComponent]
    public class SaveTransform : SaveableObject, ISceneSaveComponent<TransformSave>
    {
        public bool savePosition = true;
        public bool saveRotation = true;
        public bool saveScale = true;

        public TransformSave SaveComponent()
        {
            var tran = transform;

            return new TransformSave
            {
                position = tran.position,
                rotation = tran.rotation,
                localScale = tran.localScale,
            };
        }

        public void LoadComponent(TransformSave data)
        {
            Debug.Log("Load Transform");
            var tran = transform;

            if (savePosition)
            {
                tran.position = data.position;
            }

            if (saveRotation)
            {
                tran.rotation = data.rotation;
            }

            if (saveScale)
            {
                tran.localScale = data.localScale;
            }
        }
    }
}
