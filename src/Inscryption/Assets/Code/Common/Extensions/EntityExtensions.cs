using Entitas;
using UnityEngine;

namespace Code.Common.Extensions
{
    public static class EntityExtensions
    {
        public static GameEntity SetParent(this GameEntity entity, Transform parent, bool worldPositionStays = true)
        {
            if (entity == null || parent == null)
                return entity;

            if (entity.hasParent)
            {
                entity.ReplaceParent(parent);
            }
            else
            {
                entity.AddParent(parent);
            }

            if (entity.hasTransform && entity.Transform != null)
            {
                entity.Transform.SetParent(parent, worldPositionStays);
            }

            return entity;
        }
    }
}

