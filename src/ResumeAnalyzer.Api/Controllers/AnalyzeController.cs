using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzer.Application.UseCases.Queries;
using ResumeAnalyzer.Domain.Exceptions;
using Wolverine;

namespace ResumeAnalyzer.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AnalyzeController(IMessageBus bus) : ControllerBase
{
    private const int MaxRequestSize = 11 * 1024 * 1024; // 11MB for request headroom

    [HttpPost("read-pdf")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxRequestSize)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ReadPdf(IFormFile file, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(file);

        try
        {
            var query = new ReadPdfQuery(file.OpenReadStream(), file.FileName, file.Length);
            
            var result = await bus.InvokeAsync<string>(query, cancellationToken).ConfigureAwait(false);
            
            return Ok(result);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (PdfEncryptedException ex)
        {
            return UnprocessableEntity(ex.Message);
        }
        catch (PdfParseException ex)
        {
            return UnprocessableEntity(ex.Message);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status499ClientClosedRequest);
        }
    }
}
