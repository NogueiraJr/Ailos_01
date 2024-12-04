using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Infrastructure.Commands;
using System.ComponentModel.DataAnnotations;

[ApiController]
[Route("api/[controller]")]
public class MovimentoController : ControllerBase
{
    private readonly IMediator _mediator;

    public MovimentoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MovimentarConta([FromBody] CreateMovimentoCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
