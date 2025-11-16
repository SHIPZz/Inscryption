using System;
using System.Collections.Generic;
using Code.Features.View.Pool;
using Code.Infrastructure.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Code.Features.View.Factory
{
	public class EntityViewFactory : IEntityViewFactory
	{
		private readonly Vector3 _farAway = new(-999, 999, 0);

		private readonly IAssetsService _assetsService;
		private readonly IInstantiator _instantiator;
		private readonly IViewPool _viewPool;
		private readonly IProjectContext _projectContext;

		private Transform _generalParent;
		private readonly Dictionary<string, Transform> _concreteParents = new();

		public EntityViewFactory(
			IAssetsService assetsService,
			IInstantiator instantiator,
			IViewPool viewPool,
			IProjectContext projectContext)
		{
			_assetsService = assetsService;
			_instantiator = instantiator;
			_viewPool = viewPool;
			_projectContext = projectContext;
		}

		public string GetViewPath(GameEntity entity)
		{
			if (entity.hasViewPath)
				return entity.ViewPath;

			if (entity.hasViewPrefab)
				return entity.ViewPrefab.GetInstanceID().ToString();

			if (entity.hasViewAssetReference)
				return entity.ViewAssetReference.AssetGUID;

			if (entity.hasViewAddressableKey)
				return entity.ViewAddressableKey;

			return null;
		}

		public IEntityView CreateViewFromResourcesPath(GameEntity entity)
		{
			Vector3 position = _farAway;

			var viewPrefab = _assetsService.LoadAssetFromResources<EntityBehaviour>(entity.ViewPath);

			if (viewPrefab == null)
				throw new NullReferenceException($"View prefab by path {entity.ViewPath} is null");

			IEntityView view = _viewPool.Has(entity.ViewPath)
				? _viewPool.Get(entity.ViewPath) as IEntityView
				: _instantiator.InstantiatePrefabForComponent<EntityBehaviour>(viewPrefab, position, Quaternion.identity, null);

			if (view == null || entity.hasView)
				return null;

			view.transform.SetParent(GetParent(entity.ViewPath));
			view.SetEntity(entity);
			view.transform.position = position;

			return view;
		}

		public IEntityView CreateViewFromPrefab(GameEntity entity)
		{
			Vector3 position = _farAway;

			var viewPath = entity.ViewPrefab.GetInstanceID().ToString();

			IEntityView view = _viewPool.Has(viewPath)
				? _viewPool.Get(viewPath) as IEntityView
				: _instantiator.InstantiatePrefabForComponent<EntityBehaviour>(entity.ViewPrefab, position, Quaternion.identity, null);

			if (view == null || entity.hasView)
				return null;

			view.transform.SetParent(GetParent(viewPath));
			view.SetEntity(entity);
			view.transform.position = position;

			return view;
		}

		public async UniTask<IEntityView> CreateViewFromAssetReference(GameEntity entity)
		{
			Vector3 position = _farAway;

			entity.isLoadingView = true;

			IEntityView view;
			string assetGuid = entity.ViewAssetReference.AssetGUID;

			if (_viewPool.Has(assetGuid))
			{
				view = _viewPool.Get(assetGuid) as IEntityView;
			}
			else
			{
				var viewPrefab = await _assetsService.LoadAsync<GameObject>(entity.ViewAssetReference);

				if (viewPrefab == null)
					throw new NullReferenceException($"View prefab by path {entity.ViewAssetReference} is null");

				view = _instantiator.InstantiatePrefabForComponent<EntityBehaviour>(viewPrefab, position, Quaternion.identity, null);
			}

			if (view == null)
				return null;

			if (entity.GetComponents().Length <= 0 ||
			    !entity.hasViewAssetReference ||
			    entity.hasView)
			{
				_viewPool.Put(view, assetGuid);
				return null;
			}

			view.transform.SetParent(GetParent(view.gameObject.name));
			view.SetEntity(entity);
			view.transform.position = position;
			entity.isLoadingView = false;

			return view;
		}

		public async UniTask<IEntityView> CreateViewFromAddressableKey(GameEntity entity)
		{
			Vector3 position = _farAway;

			entity.isLoadingView = true;

			IEntityView view;
			string viewPath = entity.ViewAddressableKey;

			if (_viewPool.Has(viewPath))
			{
				view = _viewPool.Get(viewPath) as IEntityView;
			}
			else
			{
				var viewPrefab = await _assetsService.LoadAsync<GameObject>(viewPath);

				if (viewPrefab == null)
					throw new NullReferenceException($"View prefab by path {viewPath} is null");

				view = _instantiator.InstantiatePrefabForComponent<EntityBehaviour>(viewPrefab, position, Quaternion.identity, null);
			}

			if (view == null)
				return null;

			if (entity.GetComponents().Length <= 0 ||
			    entity.hasView)
			{
				_viewPool.Put(view, viewPath);
				return null;
			}

			view.transform.SetParent(GetParent(view.gameObject.name));
			view.SetEntity(entity);
			view.transform.position = position;
			entity.isLoadingView = false;

			return view;
		}

		private Transform GetParent(string name)
		{
			if (_generalParent == null)
			{
				_generalParent = new GameObject("Entity Views").transform;
				_generalParent.SetParent(_projectContext.transform);
			}

			if (_concreteParents.TryGetValue(name, out var parent) && parent != null)
			{
				return parent;
			}
			else
			{
				Transform newParent = new GameObject(name.Replace("(Clone)", string.Empty) + " Parent").transform;
				newParent.SetParent(_generalParent);
				_concreteParents.Add(name, newParent);

				return newParent;
			}
		}
	}
}