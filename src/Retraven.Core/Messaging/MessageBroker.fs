namespace Retraven.Core.Messaging
open System
open System.Threading.Tasks
open Retraven.Core.DomainEvents


type TopicName = string

type MessageBrokerHandlerResult = Task<Result<DomainEvent, string>>
type MessageBrokerHandler = DomainEvent -> TopicName -> MessageBrokerHandlerResult

type ServiceBroker =
    | RabbitMQ of MessageBrokerHandler
    | Kafka of MessageBrokerHandler


    


