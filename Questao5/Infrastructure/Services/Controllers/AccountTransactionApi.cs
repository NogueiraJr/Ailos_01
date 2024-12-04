using MediatR;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;

namespace BankAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("movement")]
        public async Task<IActionResult> CreateMovement([FromBody] CreateMovementRequest request)
        {
            var result = await _mediator.Send(request);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message, Type = result.ErrorType });
            }
            return Ok(new { MovementId = result.MovementId });
        }

        [HttpGet("balance/{accountId}")]
        public async Task<IActionResult> GetBalance([FromRoute] int accountId)
        {
            var query = new GetAccountBalanceQuery { AccountId = accountId };
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message, Type = result.ErrorType });
            }

            return Ok(new
            {
                AccountNumber = result.AccountNumber,
                AccountHolder = result.AccountHolder,
                ResponseTime = DateTime.UtcNow,
                Balance = result.Balance
            });
        }
    }

    public class CreateMovementRequest : IRequest<CreateMovementResponse>
    {
        public string RequestId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // "C" for Credit, "D" for Debit
    }

    public class CreateMovementResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorType { get; set; }
        public int? MovementId { get; set; }
    }

    public class GetAccountBalanceQuery : IRequest<GetAccountBalanceResponse>
    {
        public int AccountId { get; set; }
    }

    public class GetAccountBalanceResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorType { get; set; }
        public string AccountNumber { get; set; }
        public string AccountHolder { get; set; }
        public decimal Balance { get; set; }
    }
}
