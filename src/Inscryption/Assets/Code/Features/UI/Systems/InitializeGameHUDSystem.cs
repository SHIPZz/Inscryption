using Entitas;
using UnityEngine;

namespace Code.Features.UI.Systems
{
    public class InitializeGameHUDSystem : IInitializeSystem
    {
        private readonly MetaContext _meta;

        public InitializeGameHUDSystem(MetaContext meta)
        {
            _meta = meta;
        }

        public void Initialize()
        {
            GameHUD hud = GameObject.FindObjectOfType<GameHUD>();

            if (hud != null)
            {
                _meta.ReplaceGameHUD(hud);
                Debug.Log("[InitializeGameHUDSystem] GameHUD registered in MetaContext");
            }
            else
            {
                Debug.LogWarning("[InitializeGameHUDSystem] GameHUD not found on scene! UI will not work.");
            }
        }
    }
}
