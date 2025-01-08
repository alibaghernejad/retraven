namespace Retraven.AppHost.Config
open Aspire.Hosting
open System.Runtime.CompilerServices
open Microsoft.Extensions.Configuration
open Aspire.Hosting.ApplicationModel




[<AutoOpen>]
type RabbitMQResourceBuilderExtensions =

    [<Extension>]
    static member RunWithStableNodeName (builder: IResourceBuilder<RabbitMQServerResource>) =
        if (builder.ApplicationBuilder.ExecutionContext.IsRunMode) then
            builder.WithEnvironment(fun context ->
                // Set a stable node name so queue storage is consistent between sessions
                let nodeName =  $"{builder.Resource.Name}@localhost"
                context.EnvironmentVariables.["RABBITMQ_NODENAME"] <- nodeName
            ) |> ignore
        builder
        
    [<Extension>]
    static member AddRabbitMqIfRequired(builder: IDistributedApplicationBuilder) =

        let username = builder.AddParameter("rabbitmqusername", "rabbituser", secret= true)
        let password = builder.AddParameter("rabbitmqpassword", "rabbitpassword", secret= true)
        let rabbitMq =  builder.AddRabbitMQ("rabbitmq", username,password)
                          .RunWithStableNodeName()  
                          .WithDataVolume("rabbitmq-data",false)
                          .WithManagementPlugin()
            // | false -> builder.AddConnectionString("rabbitmq")  
        rabbitMq                                            
