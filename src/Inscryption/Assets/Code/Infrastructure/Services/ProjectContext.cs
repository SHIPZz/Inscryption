using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Services
{
	public class ProjectContext : MonoBehaviour, IProjectContext
	{
		[Inject]
		private void Construct()
		{
		}
	}
}

