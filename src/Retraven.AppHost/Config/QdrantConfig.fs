namespace Retraven.AppHost.Config
open Aspire.Hosting
open Aspire.Hosting.ApplicationModel
open System.Runtime.CompilerServices
open Microsoft.Extensions.Configuration

[<AutoOpen>]
type QdrantResourceBuilderExtensions =
    [<Extension>]
    static member AddQdrant(builder: IDistributedApplicationBuilder) =

        let configuration = builder.Configuration
        let apiKeyStr = configuration.GetValue<string>("Aspire:Qdrant:ApiKey")
        let apiKeyParam = builder.AddParameter("ApiKey", apiKeyStr, secret= true);
        let qdrant =  builder.AddQdrant("qdrant", apiKeyParam)
                          .WithLifetime(ContainerLifetime.Persistent)
                          .WithDataVolume("qdrant-data",false)
        qdrant                                            
