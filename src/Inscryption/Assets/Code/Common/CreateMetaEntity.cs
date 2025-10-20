namespace Code.Common
{
    public static class CreateMetaEntity
    {
        public static MetaEntity Empty() =>
            Contexts.sharedInstance.meta.CreateEntity();
    }
}