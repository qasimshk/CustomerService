namespace customer.core.Services;

using System.Text.Json;
using Abstractions.Services;
using Model;
using Model.Requests;
using Model.Responses;

public class PublicService : IPublicService
{
    private readonly HttpClient _httpClient;
    private readonly IPagedList<WebsiteInfoDto> _websiteInfoPaging;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public PublicService(HttpClient httpClient, IPagedList<WebsiteInfoDto> websiteInfoPaging)
    {
        _httpClient = httpClient;
        _websiteInfoPaging = websiteInfoPaging;

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            IncludeFields = true,
        };
    }

    public async Task<PagedList<WebsiteInfoDto>> GetWebsiteDetails(ServiceParametersModel serviceParameters)
    {
        var response = await _httpClient.GetAsync("entries");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();

            var serviceResponse = JsonSerializer.Deserialize<Root>(result, _jsonSerializerOptions)!;

            var raw = serviceResponse.entries.Select(x => new WebsiteInfoDto
            {
                Api = x.API,
                Category = x.Category,
                Description = x.Description,
            }).AsQueryable();

            switch (string.IsNullOrEmpty(serviceParameters.Api))
            {
                case false when !string.IsNullOrEmpty(serviceParameters.Category):
                    raw = raw.Where(x =>
                        x.Api == serviceParameters.Api &&
                        x.Category == serviceParameters.Category);
                    break;
                case false:
                    raw = raw.Where(x => x.Api == serviceParameters.Api);
                    break;
                default:
                {
                    if (!string.IsNullOrEmpty(serviceParameters.Category))
                    {
                        raw = raw.Where(x => x.Category == serviceParameters.Category);
                    }

                    break;
                }
            }

            return _websiteInfoPaging.ToPagedList(raw, serviceParameters.PageNumber, serviceParameters.PageSize);
        }

        return default!;
    }

    #region Protected Service Models

    protected class Root
    {
        public int count { get; set; }
        public List<Entry> entries { get; set; }
    }

    protected class Entry
    {
        public string API { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
    }

    #endregion
}
