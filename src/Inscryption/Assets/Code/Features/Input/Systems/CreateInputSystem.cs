using Code.Common;
using Code.Common.Extensions;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Entitas;

namespace Code.Features.Input.Systems
{
    public class CreateInputSystem : IInitializeSystem
    {
        private readonly IConfigService _configService;

        public CreateInputSystem(IConfigService configService)
        {
            _configService = configService;
        }

        public void Initialize()
        {
            GameConfig gameConfig = _configService.GetConfig<GameConfig>();

            CreateInputEntity.Empty()
                .With(x => x.AddLayerMask(gameConfig.InputMask))
                .With(x => x.isInput = true)
                ;
        }
    }
}