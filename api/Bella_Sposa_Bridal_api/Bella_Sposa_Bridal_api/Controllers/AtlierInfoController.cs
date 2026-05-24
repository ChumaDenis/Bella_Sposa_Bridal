using BellaSposaBridal.Application.DTOs.AtlierInfo;
using BellaSposaBridal.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bella_Sposa_Bridal_api.Controllers;

[ApiController]
[Route("api/atlier")]
public class AtlierInfoController : ControllerBase
{
    private readonly IAtlierInfoService _atlierInfoService;

    public AtlierInfoController(IAtlierInfoService atlierInfoService)
    {
        _atlierInfoService = atlierInfoService;
    }

    [HttpGet]
    public async Task<ActionResult<AtlierInfoDto>> Get()
    {
        var info = await _atlierInfoService.GetAsync();
        if (info is null) return NotFound();
        return Ok(info);
    }

    [HttpPut]
    public async Task<ActionResult<AtlierInfoDto>> Upsert([FromBody] AtlierInfoDto dto)
    {
        var result = await _atlierInfoService.UpsertAsync(dto);
        return Ok(result);
    }
}
