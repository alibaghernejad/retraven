namespace Retraven.ServiceDefaults
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Diagnostics.HealthChecks
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Diagnostics.HealthChecks
open Microsoft.Extensions.Logging
open OpenTelemetry.Trace
open OpenTelemetry
open OpenTelemetry.Metrics
open System.Runtime.CompilerServices
open Microsoft.Extensions.Hosting
open System

type Extentions() =
   
    static let HealthEndpointPath = "/health"
    static let AlivenessEndpointPath = "/alive"
    [<Extension>]
    static member AddServiceDefaults (builder: 'Tbuilder :> IHostApplicationBuilder) = 
        builder.ConfigureOpenTelemetry() |> ignore

        builder.AddDefaultHealthChecks() |> ignore

        builder.Services.AddServiceDiscovery() |> ignore

        builder.Services.ConfigureHttpClientDefaults(fun http ->
            // Turn on resilience by default
            http.AddStandardResilienceHandler() |> ignore
            // Turn on service discovery by default
            http.AddServiceDiscovery() |> ignore
        ) |> ignore

        // Uncomment the following to restrict the allowed schemes for service discovery.
        // builder.Services.Configure<ServiceDiscoveryOptions>(options =>
        // {
        //     options.AllowedSchemes = ["https"];
        // });

        builder
    
    [<Extension>]
    static member ConfigureOpenTelemetry(builder: 'TBuilder :> IHostApplicationBuilder) =
        
        builder.Logging.AddOpenTelemetry(fun logging ->
            logging.IncludeFormattedMessage <- true;
            logging.IncludeScopes <- true;
        ) |> ignore

        
        builder.Services.AddOpenTelemetry()
            .WithMetrics(fun metrics ->
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation() |> ignore
            )
            .WithTracing(fun tracing ->
                tracing.AddSource(builder.Environment.ApplicationName)
                    .AddAspNetCoreInstrumentation(fun tracing ->
                        // Exclude health check requests from tracing
                        tracing.Filter <- fun context ->
                            not (context.Request.Path.StartsWithSegments( HealthEndpointPath))
                            && not (context.Request.Path.StartsWithSegments(AlivenessEndpointPath))
                    )
                    // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    //.AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation() |> ignore
            ) |> ignore 

        builder.AddOpenTelemetryExporters() |> ignore

        builder
    
    [<Extension>]
    static member AddOpenTelemetryExporters(builder: 'TBuilder :> IHostApplicationBuilder) =
        let useOtlpExporter = not (String.IsNullOrWhiteSpace(builder.Configuration.["OTEL_EXPORTER_OTLP_ENDPOINT"]))
        if useOtlpExporter then
            builder.Services.AddOpenTelemetry().UseOtlpExporter() |> ignore

        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
        //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //    builder.Services.AddOpenTelemetry()
        //       .UseAzureMonitor();
        //}

        builder

    [<Extension>]
    static member AddDefaultHealthChecks(builder: 'TBuilder :> IHostApplicationBuilder) =
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", (fun () -> HealthCheckResult.Healthy()), ["live"]) |> ignore

        builder

    [<Extension>]
    static member MapDefaultEndpoints(app: WebApplication) =
    
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment()) then
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks(HealthEndpointPath) |> ignore

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions(
                Predicate =  _.Tags.Contains("live") 
            )) |> ignore

        app

