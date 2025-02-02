namespace Retraven.Core
open System
open System.Threading.Tasks
open Retraven.Core

[<CLIMutable>]
type EmbeddingTask = {
    Id: int
    StorageProvider: string
    StorageBucketName: string
    StorageObjectKey: string
    VectorDbProvider: string
    EmbeddingsProvider: string option
    DocumentId: string option
    CorrelationKey: string option
    ChunkSize: int option
    ChunkOverlap: int option
    ChunkStrategy: string option
    DataBag: string option
    Status: string
    CreatedAt: DateTime
    UpdatedAt: DateTime option
}


type EmbeddingTaskState =
    | NotGenerated of reason: string
    | Generating 
    | Generated of vector: float[]
    | Stored of id: Guid
    | RetrievalFailed of error: string

// type DocEmbeddingTaskHandlerInput = 
//     | DefaultEmbedingTaskResult of Task<DocumentEmbeddingTask option>

// type DocEmbeddingTaskHandlerOutput = 
//     | DefaultEmbedingTaskResult of Task<DocumentEmbeddingTask option>
    
    
type EmbeddingTaskHandlerResult = Task<Result<EmbeddingTask, string>>
type EmbeddingTaskHandler = EmbeddingTask -> EmbeddingTaskHandlerResult

type EmbeddingTaskPersister =
    | Postgres of EmbeddingTaskHandler
    | MongoDb of EmbeddingTaskHandler


    


