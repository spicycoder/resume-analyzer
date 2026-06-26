using Microsoft.AspNetCore.Mvc;

using ResumeAnalyzer.Application.UseCases.Queries;
using ResumeAnalyzer.Domain.Models;

using Wolverine;

namespace ResumeAnalyzer.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AnalyzeController(IMessageBus bus) : ControllerBase
{
    private const int MaxRequestSize = 11 * 1024 * 1024; // 11MB for request headroom

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxRequestSize)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnalysisResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Analyze(IFormFile jd, IFormFile resume, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(jd);
        ArgumentNullException.ThrowIfNull(resume);

        try
        {
            using var jdStream = jd.OpenReadStream();
            using var resumeStream = resume.OpenReadStream();

            var query = new AnalyzeResumeQuery(
                jdStream, jd.FileName, jd.Length,
                resumeStream, resume.FileName, resume.Length);

            var result = await bus.InvokeAsync<AnalysisResult>(query, cancellationToken).ConfigureAwait(false);

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
        catch (RateLimitExceededException ex)
        {
            return StatusCode(StatusCodes.Status429TooManyRequests, ex.Message);
        }
        catch (TimeoutException ex)
        {
            return StatusCode(StatusCodes.Status504GatewayTimeout, ex.Message);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status499ClientClosedRequest);
        }
#pragma warning disable CA1031
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An unexpected error occurred: {ex.Message}");
        }
#pragma warning restore CA1031
    }
}