using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RapidPay.Application.Features.CardManagement.AuthorizeCard;
using RapidPay.Application.Features.CardManagement.CreateCard;
using RapidPay.Application.Features.CardManagement.GetCardBalance;
using RapidPay.Application.Features.CardManagement.PayWithCard;
using RapidPay.Application.Features.CardManagement.UpdateCardDetails;

namespace RapidPayApi.Controllers;

[ApiController]
[Authorize]
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
        if (Request.Headers.TryGetValue("X-Idempotency-Key", out var idempotencyKey))
        {
            command = new CreateCardCommand(command.CreditLimit, idempotencyKey);
        }

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

    // POST: api/Card/pay
    [HttpPost("pay")]
    public async Task<IActionResult> Pay([FromBody] PayWithCardCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // GET: api/Card/{cardId}/balance
    [HttpGet("{cardId}/balance")]
    public async Task<IActionResult> GetBalance(Guid cardId)
    {
        var query = new GetCardBalanceQuery(cardId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // PUT: api/Card/update
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateCardDetailsCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
