namespace Retraven.WebApi
#nowarn "20"
open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Retraven.ServiceDefaults
open Microsoft.AspNetCore.Http

module Program =
    open Giraffe
    open Endpoints
    open Microsoft.AspNetCore.Http.Features
    let exitCode = 0

    [<EntryPoint>]
    let main args =
        
        
        let buildOptions = new WebApplicationOptions()
        buildOptions.Args=args
        buildOptions.WebRootPath = "wwwroot"
        let builder = WebApplication.CreateBuilder(buildOptions)
        
        // Add service defaults & Aspire client integrations.
        builder.AddServiceDefaults()
       
        // Add services to the container.
        builder.Services.AddProblemDetails()

        //Add Giraffe Integration.
        builder.Services.AddGiraffe()

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi()

        // Add postgres client integration.
        builder.AddNpgsqlDataSource(connectionName= "Retraven")
        
        // //Add minIo client integration
        let endpoint = builder.Configuration.GetConnectionString("MinIO")
        builder.AddMinioClient(connectionName = "MinIO")

        // Add RabbitMq Service Broker
        builder.AddRabbitMQClient(connectionName= "RabbitMQ");

        // Add Qdrant integration
        builder.AddQdrantClient("qdrant");

        let parsingError err = RequestErrors.BAD_REQUEST err
        let webApp  =
            choose [
                GET >=> choose [
                    route "/ping"   >=> text "pong"
                    route "/"       >=> htmlFile "/index.html"
                    route "/retrieves/query" 
                                  >=> tryBindQuery<RetrieveDataWithQuery.In> parsingError None (fun entry -> validateModel RetrieveDataWithQuery.handler entry)]
                POST >=> choose [
                    route "/embeddings" 
                    >=> tryBindForm<CreateEmbeddingTask.In> parsingError None (fun entry -> validateModel CreateEmbeddingTask.handler entry)]
       
                ]

        builder.Services.Configure<FormOptions>(fun (options:FormOptions) ->
            options.MultipartBodyLengthLimit = Int64.MaxValue // Example: 1MB for the entire body
            options.BufferBodyLengthLimit = Int64.MaxValue // Example: 1MB for the entire body
            options.MultipartBoundaryLengthLimit = Int32.MaxValue // Example: 1MB for the entire body
            options.MemoryBufferThreshold  = Int32.MaxValue // Example: 32KB for headers
            |> ignore
        )

        let app = builder.Build()
        // Bootstrap Storage related Services.
        printfn "Bootstrapping storage services..."
        builder.BootstrapStorageServices()
        app.UseStaticFiles()
        app.UseDefaultFiles()

        // Configure the HTTP request pipeline.
        // app.UseExceptionHandler();

        // if (app.Environment.IsDevelopment()) then
        //     app.MapOpenApi() |> ignore




        // app.UseAuthentication()
        // app.UseAuthorization()

        //  Required if you’re using antiforgery tokens
        // app.UseAntiforgery()


        app.MapDefaultEndpoints();

        // Add Giraffe to the ASP.NET Core pipeline
        app.UseGiraffe webApp


        app.Run();


        exitCode
