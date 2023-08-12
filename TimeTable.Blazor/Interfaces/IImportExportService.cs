using TimeTable.Blazor.Models;

namespace TimeTable.Blazor.Interfaces;

public interface IImportExportService
{
    public List<ImportTemplate> GetImportTemplates();
    public int ImportData(string type, string datas);
}