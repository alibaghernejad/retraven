open System
open Aspire.Hosting
open Retraven.AppHost.Config

let args = Environment.GetCommandLineArgs() 
let builder = DistributedApplication.CreateBuilder(args)
let cache = builder.AddRedis("cache")

let postgres = builder.AddPostgresIfRequired()
           
let minIo = builder.AddMinio()
let rabbitMq = builder.AddRabbitMqIfRequired()
                .RunWithStableNodeName()

let qdrant = builder.AddQdrant()
let ollama = builder.AddOllama()

// let ragProject =builder.AddProject<Projects.
// let apiService = builder.AddProject<Projects.Retraven_WebApi>("webapi", launchProfileName= "webapilaunchprofile")
//                     .WithReference(postgres)
//                     .WaitFor(postgres)
//                     .WithReference(minIo)
//                     .WaitFor(minIo)
//                     .WithReference(rabbitMq)
//                     .WaitFor(rabbitMq)
//                     .WithReference(qdrant)
//                     .WaitFor(qdrant)
//                     .WithHttpHealthCheck("/health")
                    
let host = builder.Build()
host.Run()
