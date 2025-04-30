using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Retraven.Rag;

var builder = WebApplication.CreateBuilder(args);
// Register the Kernel as a singleton in DI
builder.Services.AddHttpClient();
builder.AddSemanticKernel();
var app = builder.Build();


app.MapPost("/api/ask", async (Kernel kernel, AskRequest req) =>
{
    var result = await kernel.InvokeAsync(
        pluginName: "RAGPlugin",
        functionName: "AnswerWithRAG",
        arguments: new KernelArguments { ["query"] = req.Question });

    return Results.Ok(new { result = result.ToString() });
});

app.Run();

public record AskRequest(string Question);
