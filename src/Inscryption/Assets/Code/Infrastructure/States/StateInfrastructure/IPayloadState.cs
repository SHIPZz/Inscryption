using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.StateInfrastructure
{
    public interface IPayloadState<in TPayload>
    {
        UniTask EnterAsync(TPayload payload, CancellationToken cancellationToken = default);
    }
}