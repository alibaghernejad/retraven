namespace Retraven.AppHost.Config
open Aspire.Hosting
open System.Runtime.CompilerServices
open Microsoft.Extensions.Configuration
open Aspire.Hosting.ApplicationModel




[<AutoOpen>]
type DistributedAppBuilderExtensions =

    [<Extension>]
    static member AddPostgresIfRequired(builder: IDistributedApplicationBuilder) =
        let configuration = builder.Configuration
        let connectionString = configuration.GetConnectionString("PostgreSQL")
        let pgAdminPort = configuration.GetValue<int>("PostgreSQL:PgAdmin:Port")
        let existingPostgresServer = configuration.GetValue<bool>("PostgreSQL:ExistingPostgresServer")
        // let username = builder.AddParameter("username", secret= true);
        // let password = builder.AddParameter("password", secret= true);

        // let postgres = builder.AddPostgres("postgres", username, password);

        let username = builder.AddParameter("PostgresUsername", "postgresroot", secret = true)
        let password = builder.AddParameter("PostgresPassword", "Ert@123", secret = true)

        let postgres = 
            match not existingPostgresServer with
            | true -> builder.AddPostgres("postgres", username,password)  
                          .WithDataVolume("postgres-data",false)
                          

                          .WithPgAdmin(fun pgAdmin -> pgAdmin.WithHostPort pgAdminPort |> ignore)
                          .AddDatabase(Constants.PostgresDbName)
                        //   .WithCreationScript(Constants.PostgresDbBootstrapScript)
                        :?> IResourceBuilder<IResourceWithConnectionString>
            | false -> builder.AddConnectionString("postgres")  
        // postgres :?> IResourceBuilder<PostgresServerResource>
        postgres                                            

              
    // [<Extension>]
    // static member AddPostgresIfRequired2(builder: IDistributedApplicationBuilder) =
    //     let configuration = builder.Configuration
    //     let connectionString = configuration.GetConnectionString("PostgreSQL")
    //     let pgAdminPort = configuration.GetValue<int>("PostgreSQL:PgAdmin:Port")
    //     let existingPostgresServer = configuration.GetValue<bool>("PostgreSQL:ExistingPostgresServer")
    //     let creationScript = """
    //         -- Create the database
    //         CREATE DATABASE {{databaseName}};

    //         """
        
    //     let postgres = 
    //         match existingPostgresServer with
    //         | true -> builder.AddPostgres("postgres")
    //         | false -> builder.AddConnectionString("postgres") :?> IResourceBuilder<PostgresServerResource>
                       
                       
    //                     // .WithPgAdmin(fun pgAdmin -> pgAdmin.WithHostPort pgAdminPort |> ignore)

    //     postgres
        // builder.AddPostgres("postgres")
                
        //         .WithConnectionString(connectionString)

                
    // [<Extension>]
    // static member WithReference<'Destination, 'Source
    //         when 'Destination :> IResourceWithEnvironment
    //         and 'Source :> IResourceWithServiceDiscovery>
    //     (
    //         builder: IResourceBuilder<'Destination>,
    //         source: IResourceBuilder<'Source>
    //     ) : IResourceBuilder<'Destination> =
    //     builder.WithReference(source :?> IResourceBuilder<IResourceWithServiceDiscovery>)


