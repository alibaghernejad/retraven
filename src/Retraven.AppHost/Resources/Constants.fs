module Constants

[<Literal>]
let PostgresDbName = "Retraven"
let PostgresDbBootstrapScript = $"""CREATE DATABASE {PostgresDbName};"""
