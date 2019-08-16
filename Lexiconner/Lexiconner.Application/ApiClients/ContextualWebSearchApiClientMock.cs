using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lexiconner.Application.ApiClients.Dtos;

namespace Lexiconner.Application.ApiClients
{
    public class ContextualWebSearchApiClientMock : IContextualWebSearchApiClient
    {
        public Task AutoCompleteAsync()
        {
            throw new NotImplementedException();
        }

        public Task NewsSearchAsync()
        {
            throw new NotImplementedException();
        }

        public Task WebSearchAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ImageSearchResponseDto> ImageSearchAsync(string query, int pageNumber = 1, int pageSize = 10, bool isAutoCorrect = false, bool isSafeSearch = false)
        {
            return new ImageSearchResponseDto
            {
                TotalCount = 1,
                _Type = "images",
                Value = new List<ImageSearchResponseDto.ImageSearchResponseItemDto>
                {
                    new ImageSearchResponseDto.ImageSearchResponseItemDto
                    {
                        Url = "FAKE",
                        Height = "100",
                        Width = "200",
                        Thumbnail = "FAKE",
                        ThumbnailHeight = "FAKE",
                        ThumbnailWidth ="FAKE",
                        Base64Encoding = null,
                    }
                }
            };
        }
    }
}
