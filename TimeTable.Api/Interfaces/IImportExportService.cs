namespace TimeTable.Api.Interfaces;

public interface IImportExportService
{
    public int ImportData(string type, string datas);

    public byte[] ExportData(DateTime? startDate, DateTime? endDate);
}