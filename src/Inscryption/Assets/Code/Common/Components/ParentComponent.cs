using Entitas;
using UnityEngine;

namespace Code.Common.Components
{
    [Game]
    public class ParentComponent : IComponent
    {
        public Transform Value;
    }
}
