using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dte.Api.Acceptance.Test.Helpers.Clients;
using Dte.Common.Extensions;
using Dte.Common.Helpers;
using Dte.Common.Http;
using Dte.Common.Responses;

namespace ReferenceDataApi.Acceptance.Tests.Clients
{
    public class ReferenceDataApiClient : IStubApiClient
    {
        private readonly ApiClient _apiClient;
        private readonly List<(IApiResponse<ResponseBase> Response, string Uri)> _responses;
        private readonly List<object> _requests;

        public IEnumerable<IApiResponse<ResponseBase>> Responses => _responses.Select(r => r.Response);
        public IEnumerable<(IApiResponse<ResponseBase> Response, string Uri)> ResponsesByUri => _responses;
        public IEnumerable<object> Requests => _requests;
        
        public ReferenceDataApiClient(ApiClient apiClient)
        {
            _apiClient = apiClient;
            _requests = new List<object>();
            _responses = new List<(IApiResponse<ResponseBase> Response, string Uri)>();

            //ConversationId = Guid.NewGuid().ToString();
        }
        
        public string ConversationId { get; set; }
        
        public void SetBasicAuthorisation(string username, string password)
        {
            _apiClient.SetBasicAuthorisationHeader(username, password);
        }
        
        public void SetBearerAuthorisation(string token)
        {
            _apiClient.SetBearerAuthorisationHeader(token);
        }
        
        public async Task<IApiResponse<TResponse>> SendAsync<TRequest, TResponse>(TRequest request, string uri, HttpMethod method, RequestOptions options, string contentType = "application/json", string accept = "application/json") where TResponse : ResponseBase
        {
            if (request != null)
            {
                _requests.Add(request);
            }

            var contentNegotiation = GetContentNegotiation(options ?? new RequestOptions(), contentType, accept);

            var response = await _apiClient.SendRequest<object, TResponse>(method, uri, request, contentNegotiation);

            _responses.Add((response, uri));

            return response;
        }
        
        private ContentNegotiation GetContentNegotiation(RequestOptions options, string contentType, string accept)
        {
            var contentNegotiation = new ContentNegotiation
            {
                Accept = accept,
                ContentType = contentType
            };
            
            if (options.IncludeConversationIdHeader && !string.IsNullOrWhiteSpace(ConversationId))
            {
                contentNegotiation.Headers["ConversationId"] = ConversationId;
            }
            
            if (options.IncludeUserAgentHeader)
            {
                contentNegotiation.Headers["user-agent"] = "user-agent-value";
            }
            
            if (!options.IncludeAuthorizationHeader)
            {
                contentNegotiation.Headers["Authorization"] = "";
            }

            if (options.IncludeVersionHeader)
            {
                contentNegotiation.Headers["Version"] = "1";
            }
            
            return contentNegotiation;
        }
    }
}