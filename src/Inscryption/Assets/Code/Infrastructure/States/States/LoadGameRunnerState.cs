using System.Threading;
using Code.Infrastructure.Loading;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.States
{
    public class LoadGameRunnerState : IEnterState
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly IStateMachine _stateMachine;

        public LoadGameRunnerState(ISceneLoader sceneLoader, IStateMachine stateMachine)
        {
            _sceneLoader = sceneLoader;
            _stateMachine = stateMachine;
        }

        public async UniTask EnterAsync(CancellationToken cancellationToken = default)
        {
           await _sceneLoader.LoadSceneAsync(Scenes.Game, cancellationToken);

           await _stateMachine.EnterAsync<GameRunnerState>(cancellationToken);
        }
    }
}