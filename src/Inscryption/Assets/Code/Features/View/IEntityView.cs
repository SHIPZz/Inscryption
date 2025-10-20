using UnityEngine;

namespace Code.Features.View
{
  public interface IUnityView
  {
    GameObject gameObject { get; }
    Transform transform { get; }
  }
  
  public interface IEntityView : IUnityView
  {
    GameEntity Entity { get; }
    void SetEntity(GameEntity entity);
    void ReleaseEntity();
  }
}