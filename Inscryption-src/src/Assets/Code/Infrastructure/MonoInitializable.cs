using UnityEngine;
using Zenject;

namespace Code.Infrastructure
{
    public abstract class MonoInitializable : MonoBehaviour, IInitializable
    {
        public abstract void Initialize();
    }
}