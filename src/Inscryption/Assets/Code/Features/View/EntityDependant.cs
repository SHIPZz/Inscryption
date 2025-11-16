using Code.Common.Extensions;
using UnityEngine;

namespace Code.Features.View
{
  public abstract class EntityDependant : MonoBehaviour
  {
    public EntityBehaviour EntityView;

    public GameEntity Entity => EntityView != null ? EntityView.Entity : null;
    public EntityBehaviour EntityBehaviour => EntityView;

    private void Awake()
    {
      if (!EntityView)
        EntityView = this.GetComponentAnyWhere<EntityBehaviour>();
      
      OnAwake();
    }
    
    protected virtual void OnAwake() { }

    private void OnValidate()
    {
      if (EntityView == null)
        EntityView = this.GetComponentAnyWhere<EntityBehaviour>();
    }
  }
}