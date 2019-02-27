using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZendeskApi.Client.Extensions;

namespace ZendeskApi.Client.Exceptions
{
    internal class ZendeskRequestExceptionBuilder
    {
        private HttpResponseMessage _response;
        private List<HttpStatusCode> _expectedStatusCode;
        private string _helpDocsResource;

        public ZendeskRequestExceptionBuilder()
        {
            
        }

        public ZendeskRequestExceptionBuilder WithResponse(HttpResponseMessage response)
        {
            this._response = response;
            return this;
        }
        public ZendeskRequestExceptionBuilder WithExpectedHttpStatus(params HttpStatusCode[] status)
        {
            if (status != null)
            {
                if (_expectedStatusCode != null)
                {
                    this._expectedStatusCode.AddRange(status);
                }
                else
                {
                    this._expectedStatusCode = new List<HttpStatusCode>(status);
                }
            }
            return this;
        }
        public ZendeskRequestExceptionBuilder WithHelpDocsLink(string link)
        {
            this._helpDocsResource = link;
            return this;
        }

        public async Task<ZendeskRequestException> Build()
        {
            var message = new StringBuilder();
            ErrorResponse error = null;

            if (_response != null && (int)_response.StatusCode <= 499)
            {
                error = await _response.Content.ReadAsAsync<ErrorResponse>();

                if (error?.Error != null && error.Description != null)
                {
                    message.AppendLine($"{error.Error}: {error.Description}.");
                }
            }

            if (_expectedStatusCode != null)
            {
                if (_expectedStatusCode.Count > 1)
                {
                    var expectedCodes = string.Join(", ", _expectedStatusCode.Select(x => $"({(int)x} {x})"));
                    message.AppendLine(_response != null
                        ? $"Status code retrieved was {_response.StatusCode} and not in {expectedCodes} as expected."
                        : $"Status code retrieved was not in {expectedCodes} as expected.");
                }
                else
                {
                    message.AppendLine(_response != null
                        ? $"Status code retrieved was {_response.StatusCode} and not {_expectedStatusCode[0]}({(int)_expectedStatusCode[0]}) as expected."
                        : $"Status code retrieved was not {_expectedStatusCode[0]} as expected.");
                }
            }
            else if (_response != null)
            {
                message.AppendLine($"Status code retrieved was {_response.StatusCode}.");
            }

            if (_helpDocsResource != null)
            {
                message.AppendLine($"See: https://developer.zendesk.com/rest_api/docs/{_helpDocsResource} .");
            }

            return new ZendeskRequestException(message.ToString(), error, _response);
        }

    }
}