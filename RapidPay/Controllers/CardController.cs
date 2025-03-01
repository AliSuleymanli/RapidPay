using MediatR;
using Microsoft.AspNetCore.Mvc;
using RapidPay.Application.Features.CardManagement.AuthorizeCard;
using RapidPay.Application.Features.CardManagement.CreateCard;

namespace RapidPayApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardController : ControllerBase
{
    private readonly IMediator _mediator;

    public CardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: api/Card/create
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateCardCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // POST: api/Card/authorize
    [HttpPost("authorize")]
    public async Task<IActionResult> Authorize([FromBody] AuthorizeCardCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
