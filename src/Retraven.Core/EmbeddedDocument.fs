namespace Retraven.Core

open System

/// Represents an embedded document with its vector and metadata.
type EmbeddedDocument = {
    Id: string
    Content: string
    Vector: float[]
    Metadata: Map<string, string>
}

/// Represents the type of embeddings
/// Model the different embedding types
type EmbeddingProvider =
    | OpenAI
    | Cohere
    | Jina
    | Titan

/// Represents the type of vector database
/// Model the different vector database types
type VectorDBType =
    | Pinecone
    | Weaviate
    | Milvus
    | Qdrant
    | Deeplake
    | Vespa
    | Pgvector
    | Redis
    | Lancedb
    | Mongodb