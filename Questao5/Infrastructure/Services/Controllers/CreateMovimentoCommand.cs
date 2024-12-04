using MediatR;

namespace Questao5.Infrastructure.Commands
{
    public class CreateMovimentoCommand : IRequest<Guid>
    {
        public string IdRequisicao { get; set; }
        public string IdContaCorrente { get; set; }
        public decimal Valor { get; set; }
        public string TipoMovimento { get; set; }
    }
}
