using System.Net;

namespace UserService.Application.Web
{
    public interface IApiResponse<TResult>
    {
        TResult? ResultValue { get; set; }
        HttpStatusCode StatusCode { get; set; }
        string Message { get; set; }
        bool IsSuccess { get; set; }
    }
}