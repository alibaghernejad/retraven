namespace Retraven.WebApi
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Configuration
open System.Runtime.CompilerServices  

[<Extension>]
type WebApplicationBuilderExtensions =
    [<Extension>]
    static member BootstrapStorageServices(builder: WebApplicationBuilder) =
        // get NPGSQL connection string from configuration
        let connectionString = builder.Configuration.GetConnectionString("Retraven")
        DbContext.initializeDatabase(connectionString)


