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
		private readonly GameTestRunner _gameTestRunner;

		public EntryPoint(IAssetsService assetsService, IConfigService configService, GameTestRunner gameTestRunner)
		{
			_assetsService = assetsService;
			_configService = configService;
			_gameTestRunner = gameTestRunner;
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
			await _configService.LoadConfigAsync<BoardConfig>(nameof(BoardConfig));

			Debug.Log("[EntryPoint] Initializing game...");
			_gameTestRunner.Initialize();

			Debug.Log("[EntryPoint] Initialization complete!");
		}
	}
}

