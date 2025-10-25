using Code.Common.Extensions;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Entitas;

namespace Code.Features.Input.Systems
{
    public class CreateInputSystem : IInitializeSystem
    {
        private readonly InputContext _input;
        private readonly IConfigService _configService;

        public CreateInputSystem(InputContext input, IConfigService configService)
        {
            _configService = configService;
            _input = input;
        }

        public void Initialize()
        {
            GameConfig gameConfig = _configService.GetConfig<GameConfig>();

            _input.CreateEntity()
                .With(x => x.AddLayerMask(gameConfig.InputMask))
                .With(x => x.isInput = true)
                ;
        }
    }
}