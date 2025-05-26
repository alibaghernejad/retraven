module DbContext 
open System
open System.Data
open Npgsql
open FSharp.Data.Sql
open Retraven.Core
open System.Threading.Tasks


let [<Literal>] dbVendor = Common.DatabaseProviderTypes.POSTGRESQL
let [<Literal>] connectionString = "Host=127.0.0.1;Port=32774;Username=postgresroot;Password=Ert@123;Database=Retraven"
let [<Literal>] resolutionPath = "/home/qfwfq/.nuget/packages/npgsql/"
let [<Literal>] individualsAmount = 1000
let [<Literal>] useOptTypes  = Common.NullableColumnType.OPTION
let [<Literal>] owner = "public, admin, postgresroot, references"

let initializeDatabase2 () =
    use conn = new NpgsqlConnection(connectionString)
    conn.Open()

let getConn conn = new NpgsqlConnection(conn)
let initializeDatabase (conn:string) =
    // Print "Initializing database..."
    printfn "Initializing database with connection string: %s" conn
    
    // Build the connection string
    use conn = getConn conn
    // Open the connection
    conn.Open()

    let createTableSQL = """
    CREATE TABLE IF NOT EXISTS embedding_tasks (
        id SERIAL PRIMARY KEY,
        storage_provider VARCHAR(32) NOT NULL,
        storage_bucket_name VARCHAR(64) NOT NULL,
        storage_object_key VARCHAR(256) NOT NULL,
        vector_db_provider VARCHAR(32) NOT NULL,
        embeddings_provider VARCHAR(32),
        correlation_key VARCHAR(36) NULL,
        chunk_size INT,
        chunk_overlap INT,
        chunk_strategy VARCHAR(64),
        data_bag JSONB NULL,
        status VARCHAR(16) NOT NULL,
        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        updated_at TIMESTAMP NULL,
        document_id varchar(36) NULL
    );
    """
    
    use cmd = new NpgsqlCommand(createTableSQL, conn)
    cmd.ExecuteNonQuery() |> ignore

    printfn "Table 'embedding_tasks' created or already exists." 

// SQLProvider setup
type sql = SqlDataProvider<
                dbVendor,
                connectionString,
                "",
                resolutionPath,
                individualsAmount,
                useOptTypes,
                owner>

let mutable ctx = sql.GetDataContext()

let embeddingTaskPersister : EmbeddingTask -> Task<Result<EmbeddingTask,string>> =
    fun (embedTsk: EmbeddingTask)   ->

        task {
            try
            let entry = ctx.Public.EmbeddingTasks.Create()
            entry.Status <- embedTsk.Status 
            entry.StorageProvider <- embedTsk.StorageProvider
            entry.StorageBucketName <- embedTsk.StorageBucketName
            entry.StorageObjectKey <- embedTsk.StorageObjectKey 
            entry.VectorDbProvider <- embedTsk.VectorDbProvider
            entry.EmbeddingsProvider <- embedTsk.EmbeddingsProvider
            entry.DocumentId <- embedTsk.DocumentId
            entry.CorrelationKey <- embedTsk.CorrelationKey
            entry.ChunkSize <- embedTsk.ChunkSize
            entry.ChunkOverlap <- embedTsk.ChunkOverlap
            entry.ChunkStrategy <- embedTsk.ChunkStrategy
            do! ctx.SubmitUpdatesAsync()
            printfn "Embedding task persisted successfully."
            // Return the persisted embedding task
            return Ok { embedTsk with Id=entry.Id }
            with
            | ex -> return Error (sprintf "An error occurred while persisting the embedding task: %s" ex.Message)
        }