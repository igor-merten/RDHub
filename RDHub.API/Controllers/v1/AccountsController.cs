using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RDHub.API.Contracts.Accounts;
using RDHub.Application.Commands.CreateAccount;
using RDHub.Application.Commands.DeleteAccount;
using RDHub.Application.Commands.UpdateAccount;
using RDHub.Application.Queries.GetAccountById;

namespace RDHub.API.Controllers.v1;

/// <summary>
/// API para gerenciamento de contas.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/accounts")]
[Produces("application/json")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IMediator mediator, ILogger<AccountsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Cria uma nova conta.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateAccountResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAccount(
        [FromBody] CreateAccountRequest request,
        CancellationToken ct)
    {
        _logger.LogInformation(
            "Criando conta: Document={Document}, BankId={BankId}",
            request.Document,
            request.BankId);

        var command = new CreateAccountCommand(
            CredentialId: request.CredentialId,
            Document: request.Document,
            BankId: request.BankId,
            AccountNumber: request.AccountNumber,
            Agency: request.Agency);

        var result = await _mediator.Send(command, ct);

        var response = new CreateAccountResponse(
            Id: result.Id,
            CredentialId: result.CredentialId,
            Document: result.Document,
            BankId: result.BankId,
            AccountNumber: result.AccountNumber,
            Agency: result.Agency,
            CreatedAt: result.CreatedAt);

        return CreatedAtAction(nameof(GetAccountById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Consulta uma conta pelo Id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetAccountByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountById(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        _logger.LogInformation("Consultando conta: Id={Id}", id);

        var result = await _mediator.Send(new GetAccountByIdQuery(id), ct);

        var response = new GetAccountByIdResponse(
            Id: result.Id,
            CredentialId: result.CredentialId,
            Document: result.Document,
            BankId: result.BankId,
            AccountNumber: result.AccountNumber,
            Agency: result.Agency,
            CreatedAt: result.CreatedAt);

        return Ok(response);
    }

    /// <summary>
    /// Atualiza uma conta existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAccount(
        [FromRoute] Guid id,
        [FromBody] UpdateAccountRequest request,
        CancellationToken ct)
    {
        _logger.LogInformation("Atualizando conta: Id={Id}", id);

        var command = new UpdateAccountCommand(
            Id: id,
            CredentialId: request.CredentialId,
            Agency: request.Agency,
            AccountNumber: request.AccountNumber,
            Document: request.Document);

        var result = await _mediator.Send(command, ct);

        var response = new UpdateAccountResponse(
            Id: result.Id,
            CredentialId: result.CredentialId);

        return Ok(response);
    }

    /// <summary>
    /// Remove uma conta.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccount(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        _logger.LogInformation("Removendo conta: Id={Id}", id);

        await _mediator.Send(new DeleteAccountCommand(id), ct);

        return NoContent();
    }
}