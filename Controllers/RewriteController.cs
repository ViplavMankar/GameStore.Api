using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameStore.Api.DTOs;
using GameStore.Api.Services;

[ApiController]
[Route("api/[controller]")]
public class RewriteController : ControllerBase
{
    private readonly IAiRewriteService _rewriter;

    public RewriteController(IAiRewriteService rewriter)
    {
        _rewriter = rewriter;
    }

    [HttpPost]
    // [Authorize] // enable if needed
    [ProducesResponseType(typeof(RewriteResult), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<RewriteResult>> Post([FromBody] RewriteRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Input) && string.IsNullOrWhiteSpace(request.Prompt))
            return BadRequest("Provide either Input (draft) or Prompt (topic/instructions).");

        try
        {
            var result = await _rewriter.RewriteAsync(request, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}