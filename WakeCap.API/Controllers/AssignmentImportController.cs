using Microsoft.AspNetCore.Mvc;
using WakeCap.Service.Interfaces;

namespace WakeCap.API.Controllers;

[ApiController]
[Route("api/assignments")]
public class AssignmentImportController(IWorkerZoneAssignmentImportService assignmentImportService) : ControllerBase
{
    [HttpPost("import")]
    public async Task<IActionResult> ImportAssignments([FromForm] IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("CSV file is required.");
            }

            using var stream = file.OpenReadStream();
            var result = await assignmentImportService.ImportWorkerZoneAssignmentsAsync(stream, file.FileName);

            return Ok(result);
        }
        catch(Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
