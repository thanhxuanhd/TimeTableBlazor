using TimeTable.Domain.Dtos;

namespace TimeTable.Blazor.Interfaces;

public interface IApiService
{
    Task<bool> PostFile(MultipartFormDataContent content, string templateSelect);

    Task<byte[]> GetFile(ExportAppointmentDto exportAppointment);
}