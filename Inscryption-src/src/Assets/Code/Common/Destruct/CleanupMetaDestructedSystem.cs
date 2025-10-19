using System.Collections.Generic;
using Entitas;

namespace Code.Common.Systems.Destruct
{
    public class CleanupMetaDestructedSystem : ICleanupSystem
    {
        private readonly IGroup<MetaEntity> _group;
        private readonly List<MetaEntity> _buffer = new(128);

        public CleanupMetaDestructedSystem(MetaContext meta)
        {
            _group = meta.GetGroup(MetaMatcher.Destructed);
        }

        public void Cleanup()
        {
            foreach (MetaEntity entity in _group.GetEntities(_buffer))
            {
                entity.Destroy();
            }
        }
    }
}