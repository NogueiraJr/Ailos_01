using MediatR;
using Questao5.Infrastructure.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace Questao5.Infrastructure.Handler
{
    public interface ICreateMovimentoHandler
    {
        Task<Guid> Handle(CreateMovimentoCommand request, CancellationToken cancellationToken);
    }
}
