using System.Collections.Generic;
using Code.Infrastructure.Services;
using UnityEngine;

namespace Code.Features.View.Pool
{
  public class ViewPool : IViewPool
  {
    private readonly IProjectContext _projectContext;
    
    private readonly Dictionary<string, Queue<IUnityView>> _pool = new();
    private readonly Transform _parent;

    public ViewPool(IProjectContext projectContext)
    {
      _projectContext = projectContext;
      _parent = new GameObject("View Pool").transform;
      _parent.SetParent(projectContext.transform);
    }
    
    public IUnityView Get(string viewPath)
    {
      EnsurePoolEntry(viewPath);
      
      if (!Has(viewPath)) 
        return null;
      
      IUnityView view = _pool[viewPath].Dequeue();
      view.transform.SetParent(_projectContext.transform);
      view.gameObject.SetActive(true);
      
      return view;
    }

    public void Put(IUnityView view, string viewPath)
    {
      EnsurePoolEntry(viewPath);
      view.transform.SetParent(_parent, false);
      view.gameObject.SetActive(false);
      _pool[viewPath].Enqueue(view);
    }
    
    public bool Has(string viewPath)
    {
      return _pool.ContainsKey(viewPath) && _pool[viewPath].Count > 0;
    }

    private void EnsurePoolEntry(string viewPath)
    {
      if (!_pool.ContainsKey(viewPath))
        _pool[viewPath] = new Queue<IUnityView>();
    }
  }
}