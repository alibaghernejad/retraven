#nowarn "57"
namespace Retraven.AppHost.Config
open Aspire.Hosting
open System.Runtime.CompilerServices
open Microsoft.Extensions.Configuration
open Minio
open Minio.DataModel.Args
open System.Threading.Tasks
open System
open Aspire.Hosting.ApplicationModel


type MinioDefaultBuckets = {
    RawDocuments: string
    ChunkedDocuments: string
    EmbeddingsText: string
    EmbeddingsImage: string
    IndexesFaiss: string
    Metadata: string
    Snapshots: string
    TrainingData: string
    EvaluationResults: string
}

type MinioConfig = {
    Endpoint: string
    AccessKey: string
    SecretKey: string
    Secure: bool
    DefaultBuckets: MinioDefaultBuckets
}

[<AutoOpen>]
type DistributedAppBuilderMinioExtensions =
    static member private InitializeBuckets(config: MinioConfig) =
        task {
            let minioClient = 
                let client = new MinioClient()
                client.WithEndpoint(config.Endpoint)
                     .WithCredentials(config.AccessKey, config.SecretKey)
                     .WithSSL(config.Secure)
                     .Build()
            
            use minio = minioClient
            
            let createBucketIfNotExists (bucketName: string) = 
                task {
                    let bucketExistsArgs = BucketExistsArgs().WithBucket(bucketName)
                    let! exists = minio.BucketExistsAsync(bucketExistsArgs)
                    if not exists then
                        let makeBucketArgs = MakeBucketArgs().WithBucket(bucketName)
                        do! minio.MakeBucketAsync(makeBucketArgs)
                }

            // Create all default buckets
            do! createBucketIfNotExists config.DefaultBuckets.RawDocuments
            do! createBucketIfNotExists config.DefaultBuckets.ChunkedDocuments
            do! createBucketIfNotExists config.DefaultBuckets.EmbeddingsText
            do! createBucketIfNotExists config.DefaultBuckets.EmbeddingsImage
            do! createBucketIfNotExists config.DefaultBuckets.IndexesFaiss
            do! createBucketIfNotExists config.DefaultBuckets.Metadata
            do! createBucketIfNotExists config.DefaultBuckets.Snapshots
            do! createBucketIfNotExists config.DefaultBuckets.TrainingData
            do! createBucketIfNotExists config.DefaultBuckets.EvaluationResults
        }
        


    [<Extension>]
    static member AddMinio(builder: IDistributedApplicationBuilder) =
        
        
        let configuration = builder.Configuration
        let minioConfig = {
            Endpoint = configuration.GetValue<string>("MinIO:Endpoint")
            AccessKey = configuration.GetValue<string>("MinIO:AccessKey")
            SecretKey = configuration.GetValue<string>("MinIO:SecretKey")
            Secure = configuration.GetValue<bool>("MinIO:Secure")
            DefaultBuckets = {
                RawDocuments = configuration.GetValue<string>("MinIO:DefaultBuckets:RawDocuments")
                ChunkedDocuments = configuration.GetValue<string>("MinIO:DefaultBuckets:ChunkedDocuments")
                EmbeddingsText = configuration.GetValue<string>("MinIO:DefaultBuckets:EmbeddingsText")
                EmbeddingsImage = configuration.GetValue<string>("MinIO:DefaultBuckets:EmbeddingsImage")
                IndexesFaiss = configuration.GetValue<string>("MinIO:DefaultBuckets:IndexesFaiss")
                Metadata = configuration.GetValue<string>("MinIO:DefaultBuckets:Metadata")
                Snapshots = configuration.GetValue<string>("MinIO:DefaultBuckets:Snapshots")
                TrainingData = configuration.GetValue<string>("MinIO:DefaultBuckets:TrainingData")
                EvaluationResults = configuration.GetValue<string>("MinIO:DefaultBuckets:EvaluationResults")
            }
        }
        // let minio = builder.AddContainer("minio", "minio/minio")

        //                 .WithEnvironment("MINIO_ROOT_USER", "minioadmin")
        //                 .WithEnvironment("MINIO_ROOT_PASSWORD", "minioadmin123")
        //                 .WithEndpoint(name= "api", port= 9000)       // S3 API
        //                 .WithEndpoint(name= "console", port= 9001)  //
        // Add MinIO container and return it


        let username = builder.AddParameter("user", "minioadmin")
        let password = builder.AddParameter("password", "minioadmin", secret= true)

        let minio = builder.AddMinioContainer("minio", username, password)
                        // .WithVolume("minio-data", "/usr/data", false)
                        .WithBindMount("/mnt/volumes/minio", "/data", false)
        // let minio = builder.AddMinioContainer("minio")
        
        // minio.WithEnvironment("MINIO_ROOT_USER", minioConfig.AccessKey)
        //        .WithEnvironment("MINIO_ROOT_PASSWORD", minioConfig.SecretKey)
        //        .WithPublishingCallback(fun ctx -> task {
        //              printfn "Publishing callback executed"} )
        //         // .WithEndpoint(name= "api", port= 9000)       // S3 API
        //         // .WithEndpoint(name= "console", port= 9001)
            
        //        |> ignore
        
        minio
        // Initialize buckets asynchronously
        // try
        // Task.Run(fun () -> 
        //     DistributedAppBuilderMinioExtensions.InitializeBuckets(minioConfig).Result)
        // // |> Async.AwaitTask
        // |> ignore
        // with
        // | ex -> 

        //     printfn $"Error initializing buckets: {ex.Message}"
        //     raise ex

