namespace Prototype
{
    public interface ISaveComponentData : ISaveGuid
    { }

    public interface ISaveGuid
    {
        public SerializableGuid SaveId { get; set; }
    }

    public interface ISceneSaveComponent<T> : ISaveGuid where T : ISaveComponentData
    {
        public T SaveComponent();
        public void LoadComponent(T data);
    }
}