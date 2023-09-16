using Microsoft.AspNetCore.Mvc;
using TimeTable.Api.Interfaces;

namespace TimeTable.Blazor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportExportController : ControllerBase
    {
        private readonly IImportExportService _importExportService;
        private readonly ILogger<ImportExportController> _logger;

        public ImportExportController(IImportExportService importExportService, ILogger<ImportExportController> logger)
        {
            _importExportService = importExportService;
            _logger = logger;
        }

        [HttpPost("Upload/{templateId}")]
        public IActionResult Upload([FromForm] IEnumerable<IFormFile> files, string templateId)
        {
            try
            {
                if (!files.Any())
                {
                    return BadRequest("Files empty.");
                }

                int count = 0;
                foreach (var file in files)
                {
                    using var stream = new StreamReader(file.OpenReadStream());
                    var contents = stream.ReadToEnd();

                    count += _importExportService.ImportData(templateId, contents);
                }

                if (count == 0)
                {
                    return BadRequest("There are no records imported");
                }

                return Ok($"Total records: {count}");
            }
            catch (Exception ex)
            {
                return BadRequest($"ERROR: {ex.Message}");
            }
        }

        [HttpGet("ExportFile")]
        public IActionResult Export(DateTime? startDate, DateTime? endDate)
        {
            var file = Array.Empty<byte>();
            try
            {
                file = _importExportService.ExportData(startDate, endDate);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: {message}", ex.Message);
            }

            return File(file, "text/csv");
        }
    }
}