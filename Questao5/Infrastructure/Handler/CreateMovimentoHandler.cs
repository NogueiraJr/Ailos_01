using Dapper;
using MediatR;
using Microsoft.Data.Sqlite;
using Questao5.Infrastructure.Commands;
using Questao5.Infrastructure.Sqlite;
using System.ComponentModel.DataAnnotations;

namespace Questao5.Infrastructure.Handler
{
    public class CreateMovimentoHandler : IRequestHandler<CreateMovimentoCommand, Guid>, ICreateMovimentoHandler
    {
        private readonly IDatabaseConfig _databaseConfig;

        public override bool Equals(object? obj)
        {
            return obj is CreateMovimentoHandler handler &&
                   EqualityComparer<IDatabaseConfig>.Default.Equals(_databaseConfig, handler._databaseConfig);
        }

        public async Task<Guid> Handle(CreateMovimentoCommand request, CancellationToken cancellationToken)
        {
            using var connection = new SqliteConnection(_databaseConfig.ConnectionString);

            // Validar conta
            var conta = await connection.QueryFirstOrDefaultAsync("SELECT * FROM ContaCorrente WHERE IdContaCorrente = @Id AND Ativa = 1",
                new { Id = request.IdContaCorrente });

            if (conta == null)
                throw new ValidationException("Conta inexistente ou inativa. Tipo: INVALID_ACCOUNT ou INACTIVE_ACCOUNT");

            // Validar valor e tipo
            if (request.Valor <= 0) throw new ValidationException("Valor inválido. Tipo: INVALID_VALUE");
            if (request.TipoMovimento != "C" && request.TipoMovimento != "D")
                throw new ValidationException("Tipo de movimento inválido. Tipo: INVALID_TYPE");

            // Verificar Idempotência
            var idempotenciaExistente = await connection.QueryFirstOrDefaultAsync(
                "SELECT IdMovimento FROM Movimento WHERE IdRequisicao = @IdRequisicao",
                new { request.IdRequisicao });

            if (idempotenciaExistente != null)
                return Guid.Parse(idempotenciaExistente.IdMovimento);

            // Persistir movimento
            var idMovimento = Guid.NewGuid();
            await connection.ExecuteAsync(
                "INSERT INTO Movimento (IdMovimento, IdRequisicao, IdContaCorrente, Valor, TipoMovimento, DataHoraMovimento) " +
                "VALUES (@Id, @IdRequisicao, @IdContaCorrente, @Valor, @TipoMovimento, @DataHoraMovimento)",
                new
                {
                    Id = idMovimento,
                    request.IdRequisicao,
                    request.IdContaCorrente,
                    request.Valor,
                    request.TipoMovimento,
                    DataHoraMovimento = DateTime.UtcNow
                });

            return idMovimento;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string? ToString()
        {
            return base.ToString();
        }
    }
}