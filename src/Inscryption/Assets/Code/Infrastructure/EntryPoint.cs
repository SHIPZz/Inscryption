using System;
using Code.Features.Cards.Data;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure
{
	public class EntryPoint : IInitializable
	{
		private readonly IAssetsService _assetsService;
		private readonly IConfigService _configService;
		private readonly GameRunner _gameRunner;

		public EntryPoint(IAssetsService assetsService, IConfigService configService, GameRunner gameRunner)
		{
			_assetsService = assetsService;
			_configService = configService;
			_gameRunner = gameRunner;
		}

		public async void Initialize()
		{
			try
			{
				await InitializeAsync();
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}

		private async UniTask InitializeAsync()
		{
			Debug.Log("[EntryPoint] Starting initialization...");

			await _assetsService.Initialize();
			await _configService.Initialize();

			Debug.Log("[EntryPoint] Loading configs...");
			await _configService.LoadConfigAsync<CardConfig>(nameof(CardConfig));
			await _configService.LoadConfigAsync<GameConfig>(nameof(GameConfig));

			Debug.Log("[EntryPoint] Initializing game...");
			_gameRunner.Initialize();

			Debug.Log("[EntryPoint] Initialization complete!");
		}
	}
}

