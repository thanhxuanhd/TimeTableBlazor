using TimeTable.Blazor.Interfaces;
using TimeTable.Domain.Dtos;

namespace TimeTable.Blazor.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _client;
    private readonly INotificationService _notificationService;

    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient client, INotificationService notificationService, ILogger<ApiService> logger)
    {
        _client = client;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<byte[]> GetFile(ExportAppointmentDto exportAppointment)
    {
        if (!exportAppointment.StartDate.HasValue && !exportAppointment.EndDate.HasValue)
        {
            return Array.Empty<byte>();
        }

        var response = await _client.GetAsync($"/api/ImportExport/ExportFile?startDate={exportAppointment.StartDate.Value:o}&endDate={exportAppointment.EndDate.Value:o}");

        if (!response.IsSuccessStatusCode)
        {
            return Array.Empty<byte>();
        }

        var file = await response.Content.ReadAsByteArrayAsync();

        return file;
    }

    public async Task<bool> PostFile(MultipartFormDataContent content, string templateSelect)
    {
        var success = true;

        try
        {
            var response = await _client.PostAsync($"/api/ImportExport/Upload/{templateSelect}", content);

            var messageResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _notificationService.Error(messageResponse);
                success = false;
            }
            else
            {
                _notificationService.Success(messageResponse);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error: {message}", ex.Message);
        }

        return success;
    }
}