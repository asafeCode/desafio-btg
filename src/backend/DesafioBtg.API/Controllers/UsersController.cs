using DesafioBtg.Application.Abstractions;
using DesafioBtg.Domain.DTOs.Requests;
using DesafioBtg.Domain.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DesafioBtg.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(
        [FromServices] ICreateUserUseCase useCase,
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var response = await useCase.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetByAccount), new { agencyNumber = response.AgencyNumber, accountNumber = response.AccountNumber }, response);
    }

    [HttpGet("{agencyNumber}/{accountNumber}")]
    public async Task<ActionResult<UserResponse>> GetByAccount(
        [FromServices] IGetUserByAccountUseCase useCase,
        [FromRoute] string agencyNumber,
        [FromRoute] string accountNumber,
        CancellationToken cancellationToken)
    {
        var response = await useCase.ExecuteAsync(agencyNumber, accountNumber, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

    [HttpPut("{agencyNumber}/{accountNumber}/pix-limit")]
    public async Task<ActionResult<UserResponse>> UpdatePixLimit(
        [FromServices] IUpdatePixLimitUseCase useCase,
        [FromRoute] string agencyNumber,
        [FromRoute] string accountNumber,
        [FromBody] UpdatePixLimitRequest request,
        CancellationToken cancellationToken)
    {
        var response = await useCase.ExecuteAsync(agencyNumber, accountNumber, request, cancellationToken);

        return response is null ? NotFound() : Ok(response);
    }

    [HttpDelete("{agencyNumber}/{accountNumber}")]
    public async Task<IActionResult> Delete(
        [FromServices] IDeleteUserUseCase useCase,
        [FromRoute] string agencyNumber,
        [FromRoute] string accountNumber,
        CancellationToken cancellationToken)
    {
        var deleted = await useCase.ExecuteAsync(agencyNumber, accountNumber, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{agencyNumber}/{accountNumber}/pix-transactions")]
    public async Task<ActionResult<PixTransactionResponse>> ProcessPix(
        [FromServices] IProcessPixUseCase useCase,
        [FromRoute] string agencyNumber,
        [FromRoute] string accountNumber,
        [FromBody] ProcessPixRequest request,
        CancellationToken cancellationToken)
    {
        var response = await useCase.ExecuteAsync(agencyNumber, accountNumber, request, cancellationToken);
        return Ok(response);
    }
}
