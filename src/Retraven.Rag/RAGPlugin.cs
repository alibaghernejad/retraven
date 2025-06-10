namespace Retraven.Rag;

using System.ComponentModel;
using System.Net;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

public class RAGPlugin
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly IChatCompletionService _chatCompletion;

    public RAGPlugin(IHttpClientFactory httpFactory, IConfiguration config, IChatCompletionService chatCompletion)
    {
        _httpClient = httpFactory.CreateClient("openAiHttp");
        _config = config;
        _chatCompletion = chatCompletion;
    }

    [KernelFunction, Description("Answer a question using RAG by fetching from retriever API and calling the LLM.")]
    public async Task<string> AnswerWithRAGAsync(string query)
    {

        try
        {
            // Call Retriever API
            var retrieverUrl = _config["Retriever:Url"];
            var context = await _httpClient.GetStringAsync($"{retrieverUrl}?q={Uri.EscapeDataString(query)}");

            // Load Prompt Template
            var t = _config.GetValue<string>("Prompt:TemplatePath");
            var templatePath = _config["Prompt:TemplatePath"];
            var prompt = await File.ReadAllTextAsync(templatePath);
            prompt = prompt.Replace("{{context}}", context).Replace("{{question}}", query);

            // Call OpenAI ChatCompletion via Semantic Kernel
            var chat = new ChatHistory();
            chat.AddSystemMessage("You are a helpful assistant answering based on retrieved context.");
            chat.AddUserMessage(prompt);

            var result = await _chatCompletion.GetChatMessageContentAsync(chat);
            return result?.Content ?? "[No response]";

        }
        catch (HttpOperationException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
        {
            return "[Quota exceeded – please check your LLM model billing or try again later.]";
        }
        catch (Exception ex)
        {
            return $"[Error: {ex.Message}]";
        }
    }
}
