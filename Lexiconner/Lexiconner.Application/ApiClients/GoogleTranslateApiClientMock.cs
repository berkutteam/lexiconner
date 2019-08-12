﻿using System;
using System.Net.Http;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Cloud.Translation.V2;
using Lexiconner.Application.ApiClients.Dtos;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using static Lexiconner.Application.Config.GoogleSettings;
using Lexiconner.Application.Exceptions;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Lexiconner.Application.ApiClients
{
    public class GoogleTranslateApiClientMock : IGoogleTranslateApiClient
    {
        public async Task<GoogleTranslateDetectLanguageResponseDto> DetectLanguage(string content)
        {
            return new GoogleTranslateDetectLanguageResponseDto
            {
                Languages = new List<GoogleTranslateDetectLanguageResponseDto.GoogleTranslateDetectLanguageResponseItemDto>
                {
                    new GoogleTranslateDetectLanguageResponseDto.GoogleTranslateDetectLanguageResponseItemDto
                    {
                        Confidence = 0.9,
                        LanguageCode = "ru"
                    }
                }
            };
        }

        public async Task<GoogleTranslateResponseDto> Translate(List<string> contents, string sourceLanguageCode, string targetLanguageCode)
        {
            return new GoogleTranslateResponseDto()
            {
                Translations = new List<GoogleTranslateResponseDto.GoogleTranslateResponseItemDto>
                {
                    new GoogleTranslateResponseDto.GoogleTranslateResponseItemDto
                    {
                        TranslatedText = "some text"
                    }
                },
                GlossaryTranslations = new List<GoogleTranslateResponseDto.GoogleTranslateResponseItemDto>()
            };
        }
    }
}