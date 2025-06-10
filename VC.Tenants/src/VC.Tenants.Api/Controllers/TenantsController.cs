using FluentResults;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using VC.Tenants.Api.Models.Request.Create;
using VC.Tenants.Api.Models.Request.Update;
using VC.Tenants.Api.Models.Response;
using VC.Tenants.Application.Models.Create;
using VC.Tenants.Application.Models.Update;
using VC.Tenants.Application.TenantsUseCases.Interfaces;

namespace VC.Tenants.Api.Controllers;

[ApiController]
[Route("api/v1/[Controller]")]
[ApiExplorerSettings(GroupName = OpenApi.OpenApiConfig.GroupName)]
public class TenantsController : ControllerBase
{
    private readonly IValidator<CreateTenantRequest> _createTenantValidator;
    private readonly IValidator<UpdateTenantRequest> _updateTenantValidator;

    public TenantsController(IValidator<CreateTenantRequest> createTenantValidator,
                             IValidator<UpdateTenantRequest> updateTenantValidator)
    {
        _createTenantValidator = createTenantValidator;
        _updateTenantValidator = updateTenantValidator;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseTenantDto>> GetAsync([FromServices] IGetTenantUseCase useCase)
    {
        var getResult = await useCase.ExecuteAsync();

        if (!getResult.IsSuccess)
            return BadRequest(getResult);

        var mappedResponseDto = getResult.Value.Adapt<ResponseTenantDto>();

        return Ok(mappedResponseDto);
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<Guid>> GetByUserIdAsync([FromServices] IGetTenantIdByUserIdUseCase useCase, Guid userId)
    {
        var getResult = await useCase.ExecuteAsync(userId);
        if(!getResult.IsSuccess)
            return NotFound(getResult);

        return Ok(getResult);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync([FromServices] ICreateTenantUseCase useCase, CreateTenantRequest createRequest)
    {
        var validationResult = await _createTenantValidator.ValidateAsync(createRequest);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var mappedCreateDto = createRequest.Adapt<CreateTenantParams>();

        var createResult = await useCase.ExecuteAsync(mappedCreateDto);

        if (!createResult.IsSuccess)
            return BadRequest(createResult);

        return Ok(createResult);
    }

    [HttpPost("verify-email")]
    public async Task<ActionResult<Result>> VerifyMailAsync([FromServices] IVerifyTenantEmailUseCase useCase, [FromQuery] string code)
    {
        var verifyResult = await useCase.ExecuteAsync(code);

        if(verifyResult.IsSuccess)
            return Ok(verifyResult);

        return BadRequest(verifyResult);
    }

    [HttpPost("send-verify-mail")]
    public async Task<ActionResult<Result>> SendVerifyMailAsync([FromServices] ISendVerificationMailUseCase useCase)
    {
        var sendMailResult = await useCase.ExecuteAsync();

        if(!sendMailResult.IsSuccess)
            return BadRequest();

        return Accepted();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateAsync([FromServices] IUpdateTenantUseCase useCase, UpdateTenantRequest updateRequest)
    {
        var validationResult = await _updateTenantValidator.ValidateAsync(updateRequest);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var mappedUpdateDto = updateRequest.Adapt<UpdateTenantParams>();

        var updateResult = await useCase.ExecuteAsync(mappedUpdateDto);

        if (updateResult.IsSuccess)
            return Ok(updateResult);

        return BadRequest(updateResult);
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteByIdAsync([FromServices] IDeleteTenantUseCase useCase)
    {
        var deleteResult = await useCase.ExecuteAsync();

        if (deleteResult.IsSuccess)
            return Ok();

        return BadRequest(deleteResult);
    }
}