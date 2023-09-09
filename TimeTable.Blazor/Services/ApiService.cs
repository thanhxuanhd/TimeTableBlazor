using Microsoft.AspNetCore.Components;
using TimeTable.Blazor.Interfaces;

namespace TimeTable.Blazor.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _client;
    private readonly INotificationService _notificationService;

    private readonly ILogger<ApiService> _logger;

    public ApiService(NavigationManager navigationManager, HttpClient client, INotificationService notificationService, ILogger<ApiService> logger)
    {
        _client = client;
        _client.BaseAddress = new Uri(navigationManager.BaseUri);
        _notificationService = notificationService;
        _logger = logger;
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