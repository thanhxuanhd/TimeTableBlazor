using Microsoft.Extensions.Options;
using TimeTable.Blazor.Interfaces;
using TimeTable.Domain.Dtos;

namespace TimeTable.Blazor.Services;

public class ImportExportService : IImportExportService
{
    private readonly IOptions<List<ImportTemplate>> _options;

    public ImportExportService(IOptions<List<ImportTemplate>> options)
    {
        _options = options;
    }

    public List<ImportTemplate> GetImportTemplates()
    {
        var templates = _options.Value;

        return templates.OrderBy(x => x.Type).ToList();
    }
}