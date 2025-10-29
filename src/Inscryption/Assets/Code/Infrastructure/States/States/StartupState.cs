using System;
using System.Threading;
using Code.Features.Cards.Data;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.States.States
{
    public class StartupState : IEnterState
    {
        private readonly IAssetsService _assetsService;
        private readonly IConfigService _configService;
        private readonly IStateMachine _stateMachine;

        public StartupState(IAssetsService assetsService, IConfigService configService,IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _assetsService = assetsService;
            _configService = configService;
        }

        public async UniTask EnterAsync(CancellationToken cancellationToken = default)
        {
            Debug.Log("[StartupState] 🚀 ENTER START UP STATE");
            
            try
            {
                await _assetsService.InitializeAsync(cancellationToken);
                await _configService.InitializeAsync(cancellationToken);

                Debug.Log("[EntryPoint] Loading configs...");
                await _configService.LoadConfigAsync<CardConfig>(cancellationToken);
                await _configService.LoadConfigAsync<GameConfig>(cancellationToken);
                
                await _stateMachine.EnterAsync<LoadGameRunnerState>(cancellationToken);
            }
            catch (Exception e)
            {
                Debug.LogError($"[StartupState] ❌ Error in startup: {e.Message}");
                Debug.LogError($"[StartupState] Stack trace: {e.StackTrace}");
                throw;
            }
        }
    }
}