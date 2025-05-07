using FluentResults;
using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using VC.Tenants.Api.Models.Request.Create;
using VC.Tenants.Api.Models.Request.Update;
using VC.Tenants.Api.Models.Response;
using VC.Tenants.Application.Models.Create;
using VC.Tenants.Application.Models.Update;
using VC.Tenants.Application.Tenants;

namespace VC.Tenants.Api.Controllers;

[ApiController]
[Route("[Controller]")]
[ApiExplorerSettings(GroupName = OpenApi.OpenApiConfig.GroupName)]
public class TenantsController : ControllerBase
{
    private readonly ITenantsService _tenantService;
    private readonly IValidator<CreateTenantRequest> _createTenantValidator;
    private readonly IValidator<UpdateTenantRequest> _updateTenantValidator;

    private readonly IMapper _mapper;

    public TenantsController(ITenantsService tenantService,
        IValidator<CreateTenantRequest> createTenantValidator,
        IValidator<UpdateTenantRequest> updateTenantValidator,
        IMapper mapper)
    {
        _tenantService = tenantService;
        _createTenantValidator = createTenantValidator;
        _updateTenantValidator = updateTenantValidator;

        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseTenantDto>> GetAsync()
    {
        var getResult = await _tenantService.GetAsync();

        if (!getResult.IsSuccess)
            return BadRequest(getResult);

        var mappedResponseDto = _mapper.Map<ResponseTenantDto>(getResult.Value);

        return Ok(mappedResponseDto);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(CreateTenantRequest createRequest)
    {
        var validationResult = await _createTenantValidator.ValidateAsync(createRequest);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var mappedCreateDto = _mapper.Map<CreateTenantParams>(createRequest);

        var createResult = await _tenantService.CreateAsync(mappedCreateDto);

        if (!createResult.IsSuccess)
            return BadRequest(createResult);

        return Ok(createResult);
    }

    [HttpGet("verify-email")]
    public async Task<ActionResult<Result>> VerifyMailAsync([FromQuery] string code)
    {
        var verifyResult = await _tenantService.VerifyEmailAsync(code);

        if(verifyResult.IsSuccess)
            return Ok(verifyResult);

        return BadRequest(verifyResult);
    }

    [HttpPost("send-verify-mail")]
    public async Task<ActionResult<Result>> SendVerifyMailAsync()
    {
        var sendMailResult = await _tenantService.SendVerificationMailAsync();

        if(!sendMailResult.IsSuccess)
            return BadRequest();

        return Accepted();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateAsync(UpdateTenantRequest updateRequest)
    {
        var validationResult = await _updateTenantValidator.ValidateAsync(updateRequest);

        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var mappedUpdateDto = _mapper.Map<UpdateTenantRequest, UpdateTenantParams>(updateRequest);

        var updateResult = await _tenantService.UpdateAsync(mappedUpdateDto);

        if (updateResult.IsSuccess)
            return Ok(updateResult);

        return BadRequest(updateResult);
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteByIdAsync()
    {
        var deleteResult = await _tenantService.DeleteAsync();

        if (deleteResult.IsSuccess)
            return Ok();

        return BadRequest(deleteResult);
    }
}