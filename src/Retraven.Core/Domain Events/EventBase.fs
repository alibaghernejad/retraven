namespace Retraven.Core.DomainEvents

open System

type BasicDomainEvent = {
    TaskId: int
    CreatedAt: DateTime
}

type DomainEvent =
    | EmbeddingTaskCreated of BasicDomainEvent

