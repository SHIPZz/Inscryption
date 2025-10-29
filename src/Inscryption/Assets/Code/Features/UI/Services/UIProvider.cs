using Code.Features.UI.Views;
using UnityEngine;

namespace Code.Features.UI.Services
{
    public class UIProvider : MonoBehaviour, IUIProvider
    {
     [field: SerializeField]   public GameHUD GameHUD { get; set; }
    }
}