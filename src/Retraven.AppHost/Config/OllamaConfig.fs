namespace Retraven.AppHost.Config
open Aspire.Hosting
open System.Runtime.CompilerServices
open Microsoft.Extensions.Configuration
open Aspire.Hosting.ApplicationModel

[<AutoOpen>]
type OllamaResourceBuilderExtensions =
        
    [<Extension>]
    static member AddOllama(builder: IDistributedApplicationBuilder) =
        
        let ollama = builder.AddOllama("ollama")
                            .WithDataVolume("ollama-data")
                            .WithOpenWebUI()
                            .AddModel("phi3.5")
        ollama                                            
