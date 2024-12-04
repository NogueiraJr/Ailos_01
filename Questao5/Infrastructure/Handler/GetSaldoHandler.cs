using Dapper;
using MediatR;
using Microsoft.Data.Sqlite;
using Questao5.Infrastructure.Sqlite;
using System.ComponentModel.DataAnnotations;

public class GetSaldoHandler : IRequestHandler<GetSaldoQuery, SaldoResponse>
{
    private readonly IDatabaseConfig _databaseConfig;

    public GetSaldoHandler(IDatabaseConfig databaseConfig)
    {
        _databaseConfig = databaseConfig;
    }

    public async Task<SaldoResponse> Handle(GetSaldoQuery request, CancellationToken cancellationToken)
    {
        using var connection = new SqliteConnection(_databaseConfig.ConnectionString);

        // Validar conta
        var conta = await connection.QueryFirstOrDefaultAsync(
            "SELECT * FROM ContaCorrente WHERE IdContaCorrente = @Id AND Ativa = 1",
            new { Id = request.IdContaCorrente });

        if (conta == null)
            throw new ValidationException("Conta inexistente ou inativa. Tipo: INVALID_ACCOUNT ou INACTIVE_ACCOUNT");

        // Calcular saldo
        var saldo = await connection.QuerySingleOrDefaultAsync<decimal>(
            @"SELECT 
                COALESCE(SUM(CASE WHEN TipoMovimento = 'C' THEN Valor ELSE 0 END), 0) -
                COALESCE(SUM(CASE WHEN TipoMovimento = 'D' THEN Valor ELSE 0 END), 0)
              FROM Movimento
              WHERE IdContaCorrente = @IdContaCorrente",
            new { request.IdContaCorrente });

        return new SaldoResponse
        {
            NumeroContaCorrente = conta.IdContaCorrente,
            NomeTitular = conta.NomeTitular,
            DataHoraConsulta = DateTime.UtcNow,
            SaldoAtual = saldo
        };
    }
}
