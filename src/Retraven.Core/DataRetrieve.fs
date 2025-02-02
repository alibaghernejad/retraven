namespace Retraven.Core
open System
open System.Threading.Tasks
open System.Text.Json.Serialization

/// Represents a dense embedding
type DenseEmbedding = float32[]

/// Represents a sparse embedding with indices and values
type SparseEmbedding = { Indices: int[]; Values: float32[] }

/// Represents an embedding source type
type Embedding =
    | Dense of DenseEmbedding
    | Sparse of SparseEmbedding

/// Configuration for Qdrant Search
type QdrantSearchConfig =
    { 
      CollectionName: string
      DenseVectorName: string // e.g., "all-MiniLM-L6-v2"
      SparseVectorName: string // e.g., "bm25"
      LateVectorName: string } // e.g., "colbertv2.0"

/// Search parameters (optional values)
type SearchParams =
    { DenseLimit: int
      SparseLimit: int
      FinalLimit: int
      WithPayload: bool }

/// Function type aliases for embedding models
type DenseEmbedder = string -> Task<DenseEmbedding>
type SparseEmbedder = string -> Task<SparseEmbedding>
type LateEmbedder = string -> Task<DenseEmbedding>



type SparseVector = {
    indices : int[]
    values  : float[]
}

type EmbedResponse = {
    dense_embedding_vector          : float[]
    late_interaction_embedding_vector: float[][]
    bm25_embedding_vector           : SparseVector
}


type SparseVectorPayload = {
    indices : int[]
    values  : float[]
}

type PrefetchPayload = {
    query : obj
    [<JsonPropertyName("using")>]
    usingField : string
    limit : int
}

type QueryPayload = {
    prefetch : PrefetchPayload[]
    query : obj
    [<JsonPropertyName("using")>]
    usingField : string
    limit : int
    with_payload : bool
}

type QdrantPayload = Map<string, obj>

type QdrantPoint = {
    id: string
    score: float
    payload: QdrantPayload
}

type QdrantQueryResult = {
    result: QdrantPoint[]
    status: string
    time: float
}