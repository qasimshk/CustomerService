namespace customer.core.Abstractions.Services;

using Model;
using Model.Requests;
using Model.Responses;

public interface IPublicService
{
    Task<PagedList<WebsiteInfoDto>> GetWebsiteDetails(ServiceParametersModel serviceParameters);
}
