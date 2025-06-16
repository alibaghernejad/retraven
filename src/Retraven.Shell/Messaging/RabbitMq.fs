namespace Retraven.Shell

open System
open Retraven.Core.DomainEvents

module Messaging =
    open RabbitMQ.Client
    open Newtonsoft.Json  // Use Newtonsoft.Json instead of System.Text.Json
    open Retraven.Core
    open System.Text
    open Retraven.Core.Messaging
    open System.IO

    // Function to create and send domain event to RabbitMQ
    let sendDomainEventToQueue (con: IConnection) =
        fun (evt: DomainEvent) (tpk: TopicName)  ->
            task {
                try
                    use channel = con.CreateModel()
                    // Declare a queue for the task
                    channel.QueueDeclare(tpk, durable = true, exclusive = false, autoDelete = false, arguments = null)
                    |> ignore

                    // Serialize the DomainEvent object to JSON using Newtonsoft.Json
                    let message = JsonConvert.SerializeObject(evt)
                    let body = Encoding.UTF8.GetBytes(message)

                    // Set message properties to make it persistent
                    let properties = channel.CreateBasicProperties()
                    properties.Persistent <- true // Make message persistent
                    
                    // Send the message to the queue
                    channel.BasicPublish(
                        exchange = "", 
                        routingKey = tpk, 
                        basicProperties = properties, 
                        body = body
                    )

                    printfn "Domain event sent to queue %s: %s" tpk message
                    return Ok evt
                with ex ->
                    let errorMsg = sprintf "Error sending Domain event to queue %s: %s" tpk ex.Message
                    printfn "%s" errorMsg
                    return Error errorMsg
            }
