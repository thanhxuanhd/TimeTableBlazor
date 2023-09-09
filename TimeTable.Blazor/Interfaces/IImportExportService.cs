using TimeTable.Domain.Dtos;

namespace TimeTable.Blazor.Interfaces;

public interface IImportExportService
{
    public List<ImportTemplate> GetImportTemplates();
}