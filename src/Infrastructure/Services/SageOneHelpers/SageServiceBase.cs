﻿using ApplicationCore;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Infrastructure.Services.SageOneHelpers
{
    public class SageServiceBase
    {
        protected readonly SageSettings _settings;
        protected readonly string _baseUrl;
        protected readonly IAuthConfigRepository _authRepository;
        protected readonly IAppLogger<SageService> _logger;
        protected readonly IMemoryCache _cache;

        public SageServiceBase(
            IOptions<SageSettings> options,
            IAuthConfigRepository authConfigRepository,
            IAppLogger<SageService> logger,
            IMemoryCache memoryCache)
        {
            _settings = options.Value;
            _baseUrl = _settings.SageApiBaseUrl;
            _authRepository = authConfigRepository;
            _logger = logger;
            _cache = memoryCache;
        }
        protected HttpRequestMessage GenerateRequest(HttpMethod method, Uri uri, List<KeyValuePair<string, string>> body, HttpClient httpClient, AuthConfig auth, bool getPdf = false)
        {
            var nonce = SageOneUtils.GenerateNonce();
            var signature = SageOneAPIRequestSigner.GenerateSignature(method.ToString().ToUpper(), uri, body, auth.SigningSecret, auth.AccessToken, nonce); //"TestSigningSecret"
            SageOneUtils.SetHeaders(httpClient, signature, nonce, getPdf);
            HttpRequestMessage request = new HttpRequestMessage(method, uri);
            request.Content = new StringContent(SageOneUtils.ConvertPostParams(body),
                                                body == null ? null : Encoding.UTF8,
                                                "application/x-www-form-urlencoded");//CONTENT-TYPE header
            return request;
        }

        protected void AddHeaderParameter(HttpClient httpClient, List<KeyValuePair<string, string>> parameters)
        {
            if (httpClient == null)
                return;

            if (parameters == null)
                return;

            foreach (var item in parameters)
            {
                httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
            }

        }

        protected HttpClient CreateHttpClient(string token = "")
        {
            var httpClient = new HttpClient(new LoggingHandler(new HttpClientHandler()));

            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return httpClient;
        }

        protected void AddDefaultHeaders(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected async Task<AuthConfig> GetAuthConfigAsync(SageApplicationType type)
        {
            return await _cache.GetOrCreateAsync($"CONFIG_TYPE_{type}", async entry =>
                    {
                        entry.SlidingExpiration = TimeSpan.FromSeconds(30);
                        return await _authRepository.GetAuthConfigAsync(type);
                    });
        }
    }
}