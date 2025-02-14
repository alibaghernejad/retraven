
namespace Retraven.Core.Domain
open System.Threading.Tasks
open Retraven.Core

type ObjectStorageHandlerResult = Task<EmbeddingTask option>
type ObjectStorageHandler = EmbeddingTask -> ObjectStorageHandlerResult

type ObjectStorageProvider =
    | Minio of ObjectStorageHandler
    | AzureBlob of ObjectStorageHandler
    | S3 of ObjectStorageHandler