module Endpoints.RetrieveDataWithQuery
open Giraffe
open Microsoft.AspNetCore.Http
open Minio
open Retraven.Shell.Messaging
open Microsoft.Extensions.Configuration
open System
open System.IO
open Minio.DataModel.Args
open Retraven.Core.Helpers
open System.Threading.Tasks
open System.Linq
open Minio.DataModel.Response
open Retraven.Core
open Retraven.Core.Helpers
open System.Collections.Generic
open RabbitMQ.Client
open Retraven.Core.DomainEvents
open Qdrant.Client
open EmbedApiClient

[<CLIMutable>]
type Out =
    {
    Status: string
    Message: string
    TaskId: string
    }
    

[<CLIMutable>]
type In = 
    {
    Query: string
    }

    member this.HasErrors() =
        if this.Query.Length > 250 then Some "Query is too long."
        elif this.Query.Length < 3 then Some "Query is too short."

        else None

    interface IModelValidation<In> with
        member this.Validate() =
            match this.HasErrors() with
            | Some msg -> Error (RequestErrors.badRequest (text msg))
            | None     -> Ok this
    
let handler=
    fun (req:In) (nxt : HttpFunc) (ctx : HttpContext) ->
        task {
                
                // The Qdrant client registered at composition root, is available here. 
                // However, current support is limited for scenarios such as Hybrid data retrieval (dense + sparse), 
                // Late Interaction (e.g., ColBERT), or native prefetch mechanisms.
                // Possible options include:
                //   1. Proceed with Python for advanced scenarios.
                //   2. Use another embedding library with better .NET support (e.g., ML.NET with ONNX models).
                //   3. Use the Qdrant .NET client for the currently supported tasks 
                //      and stick to the raw API provided by the Qdrant server. (Current Approach)                
                let config = ctx.GetService<IConfiguration>()
                let qdrant = ctx.GetService<QdrantClient>()
                let! embeddings = getEmbeddings req.Query
                // Set API key once
                QdrantRaw.setApiKey "61d67203-edad-40ef-9999-b9adb1bca800"

                let! resultJson = QdrantRaw.queryRerank2_1 "gapfilm" embeddings.dense_embedding_vector embeddings.bm25_embedding_vector embeddings.late_interaction_embedding_vector
                return! json resultJson nxt ctx 
        }