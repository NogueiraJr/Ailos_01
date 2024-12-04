using MediatR;

public record GetSaldoQuery(string IdContaCorrente) : IRequest<SaldoResponse>;
