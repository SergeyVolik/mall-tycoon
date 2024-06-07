namespace Prototype
{
    public static class SerializeableGuidHelper
    {
        public static SerializableGuid NewGuid() => new SerializableGuid(System.Guid.NewGuid());
    }
}
