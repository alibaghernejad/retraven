using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;
using OllamaSharp.Models;
#pragma warning disable SKEXP0001
namespace Retraven.Rag;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder AddSemanticKernel(this WebApplicationBuilder builder)
    {


        builder.Services.AddSingleton<IChatCompletionService>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();

            // Load OpenAI settings from config (optional)
            var openAiModel = builder.Configuration["LLM:OpenAI:Model"] ?? "gpt-3.5-turbo";
            var openAiKey = builder.Configuration["LLM:OpenAI:ApiKey"] ?? "<your-key>";
            var ollamaUrl = config["LLM:Ollama:Url"] ?? "<your-key>";
            var ollamaModel = config["LLM:Ollama:Model"]?? "phi3";

            // It also can configured as primary/secondry pattern.
            // Primary: OpenAI and fallback to ollama for example.
            var primaryProvider = config["LLM:PrimaryProvider"] ?? "Ollama";
            var model = primaryProvider.ToLower() switch
            {
                "openai" => new OpenAIChatCompletionService(openAiModel, openAiKey),
                _ => new OllamaApiClient(ollamaUrl, ollamaModel).AsChatCompletionService(),
            };
            return model;
        });
        builder.Services.AddSingleton<Kernel>(sp =>
        {
            var kernelBuilder = Kernel.CreateBuilder();
            var kernel = kernelBuilder.Build();
            var httpFactory = sp.GetService<IHttpClientFactory>();
            var chatCompletion = sp.GetService<IChatCompletionService>();

            kernel.ImportPluginFromObject(new RAGPlugin(httpFactory, builder.Configuration, chatCompletion));
            return kernel;
        });
        return builder;
    }
}