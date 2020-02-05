using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Celin.Jira
{
    public class Server
    {
        readonly string MEDIA_TYPE = "application/json";
        readonly JsonSerializerOptions jsonInputOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
        };
        string BaseUrl { get; }
        ILogger Logger { get; }
        HttpClient Http { get; }
        public async Task<T> GetAsync<T>(Request.Base request, CancellationTokenSource cancel = null)
        {
            HttpResponseMessage responseMessage;
            try
            {
                var uri = new UriBuilder($"{BaseUrl}{request.Path}")
                {
                    Query = request.Query
                };
                Logger?.LogDebug(uri.ToString());
                responseMessage = await Http.GetAsync(uri.Uri, cancel == null ? CancellationToken.None : cancel.Token);
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message);
                throw;
            }
            Logger?.LogDebug("{0}\n{1}", request.ToString(), responseMessage.ReasonPhrase);
            if (responseMessage.IsSuccessStatusCode)
            {
                Logger?.LogTrace(responseMessage.Content.ReadAsStringAsync().Result);
                try
                {
                    T result = JsonSerializer.Deserialize<T>(
                        responseMessage.Content.ReadAsStringAsync().Result,
                                            new JsonSerializerOptions
                                            {
                                                Converters = { new DateJsonConverter() }
                                            });
                    return result;
                }
                catch (Exception e)
                {
                    Logger?.LogError(e.Message);
                    throw;
                }
            }
            else
            {
                var resp = await responseMessage.Content.ReadAsStringAsync();
                Logger?.LogTrace(resp);
                throw new Exception(resp);
            }
        }
        public async Task PutAsync(Request.Base request, CancellationTokenSource cancel = null)
        {
            HttpResponseMessage responseMessage;
            var uri = new UriBuilder($"{BaseUrl}{request.Path}")
            {
                Query = request.Query
            };
            Logger?.LogDebug(uri.ToString());
            var content = new StringContent(
                JsonSerializer.Serialize(request.RequestBody, request.RequestBody.GetType(), jsonInputOptions), Encoding.UTF8, MEDIA_TYPE);
            try
            {
                responseMessage = await Http.PutAsync(uri.Uri, content, cancel == null ? CancellationToken.None : cancel.Token);
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message);
                throw;
            }
            Logger?.LogDebug("{0}\n{1}", request.ToString(), responseMessage.ReasonPhrase);
            Logger?.LogTrace(content.ReadAsStringAsync().Result);
        }
        public async Task<T> PostAsync<T>(Request.Base request, CancellationTokenSource cancel = null)
            where T : new()
        {
            HttpResponseMessage responseMessage;
            var uri = new UriBuilder($"{BaseUrl}{request.Path}")
            {
                Query = request.Query
            };
            Logger?.LogDebug(uri.ToString());
            var content = new StringContent(
                JsonSerializer.Serialize(request.RequestBody, request.RequestBody.GetType(), jsonInputOptions), Encoding.UTF8, MEDIA_TYPE);
            try
            {
                responseMessage = await Http.PostAsync(uri.Uri, content, cancel == null ? CancellationToken.None : cancel.Token);
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message);
                throw;
            }
            Logger?.LogDebug("{0}\n{1}", request.ToString(), responseMessage.ReasonPhrase);
            Logger?.LogTrace(content.ReadAsStringAsync().Result);
            if (responseMessage.IsSuccessStatusCode)
            {
                Logger?.LogTrace(responseMessage.Content.ReadAsStringAsync().Result);
                if (typeof(T).Equals(typeof(Response.Empty)))
                {
                    return new T();
                }
                else
                {

                    try
                    {
                        T result = JsonSerializer.Deserialize<T>(
                            responseMessage.Content.ReadAsStringAsync().Result,
                                                    new JsonSerializerOptions
                                                    {
                                                        Converters = { new DateJsonConverter() }
                                                    });
                        return result;
                    }
                    catch (Exception e)
                    {
                        Logger?.LogError(e.Message);
                        throw;
                    }
                }
            }
            else
            {
                var error = responseMessage.Content.ReadAsStringAsync().Result;
                Logger?.LogTrace(error);
                throw new Exception(error);
            }
        }
        public async Task<Response.Issues> Search(string jql)
            => await PostAsync<Response.Issues>(new Request.Search
            {
                RequestBody = new Request.RequestBody
                {
                    jql = jql
                }
            });
        public async Task<Response.Comments> ListComments(string issueIdOrKey)
            => await GetAsync<Response.Comments>(new Request.Issue.Comment
            {
                IdOrKey = issueIdOrKey
            });
        public async Task<Response.Comment> AddComment(string issueIdOrKey, IEnumerable<string> paras)
            => await PostAsync<Response.Comment>(new Request.Issue.Comment
            {
                IdOrKey = issueIdOrKey,
                RequestBody = new Request.RequestBody
                {
                    body = new Request.DocFormat<Request.ParagraphDoc<Request.TextContent>>
                    {
                        content = paras.Select(p => new Request.ParagraphDoc<Request.TextContent>
                        {
                            content = new Request.TextContent[]
                            {
                                new Request.TextContent
                                {
                                    text = p
                                }
                            }
                        })
                    }
                }
            });
        public async Task<Response.Transitions> ListTransitions(string issueIdOrKey)
            => await GetAsync<Response.Transitions>(new Request.Issue.Transition
            {
                IdOrKey = issueIdOrKey
            });
        public async Task TransitionIssue(string issueIdOrKey, string transitionId)
            => await PostAsync<Response.Empty>(new Request.Issue.Transition
            {
                IdOrKey = issueIdOrKey,
                RequestBody = new Request.RequestBody
                {
                    transition = new Request.Id
                    {
                        id = transitionId
                    }
                }
            });
        public async Task<Response.Issue> GetIssue(string issueIdOrKey)
            => await GetAsync<Response.Issue>(new Request.Issue
            {
                IdOrKey = issueIdOrKey
            });
        public async Task EditIssu(string issueIdOrKey, Request.Fields fields)
            => await PutAsync(new Request.Issue
            {
                IdOrKey = issueIdOrKey,
                RequestBody = new Request.RequestBody
                {
                    fields = fields
                }
            });
        public async Task<Response.Base> AddIssue(string projectIdOrKey, string issueTypeId, string summary, IEnumerable<string> description, IEnumerable<string> labels)
            => await PostAsync<Response.Base>(new Request.Issue
            {
                RequestBody = new Request.RequestBody
                {
                    fields = new Request.Fields
                    {
                        project = new Request.Key { key = projectIdOrKey },
                        issuetype = new Request.Id { id = issueTypeId },
                        summary = summary,
                        labels = labels,
                        description = new Request.DocFormat<Request.ParagraphDoc<Request.TextContent>>
                        {
                            content = description.Select(p
                                => new Request.ParagraphDoc<Request.TextContent>
                                {
                                    content = new Request.TextContent[]
                                    {
                                        new Request.TextContent
                                        {
                                            text = p
                                        }
                                    }
                                })
                        }
                    }
                }
            });
        public async Task<Response.Project> GetProject(string projectIdOrKey)
            => await GetAsync<Response.Project>(new Request.Project
            {
                IdOrKey = projectIdOrKey
            });
        public async Task<IEnumerable<Response.TaskType>> GetTaskTypes(string projectIdOrKey)
            => await GetAsync<IEnumerable<Response.TaskType>>(new Request.Project.Statuses
            {
                IdOrKey = projectIdOrKey
            });
        public Server(string baseUrl, string token, ILogger logger, HttpClient http = null)
        {
            Http = http ?? new HttpClient();
            Logger = logger;
            BaseUrl = baseUrl;
            Logger?.LogDebug(BaseUrl);
            Http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MEDIA_TYPE));
            Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
        }
    }
}
