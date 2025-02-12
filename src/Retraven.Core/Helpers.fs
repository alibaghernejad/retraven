namespace Retraven.Core
open System 

module Helpers =
    open System.Text.Json
    let bind switchFunction  = 
        fun ttinput -> 
            match ttinput with
            | Ok s -> switchFunction s
            | Error f -> Error f

    let (>>=) x f = 
        bind f x

    let map singleTrackkFunction twoTrackInput =
        match twoTrackInput with
        | Ok value -> Ok (singleTrackkFunction value)
        | Error err -> Error err

    let map1 singleTrackFunction =
        bind (singleTrackFunction >> Ok) 

    let tap deadEndFunction oneTrackInput=
        deadEndFunction oneTrackInput
        oneTrackInput

    let normalize (input: string) =
        input.Trim().ToLower()

    let getJsonStr data :string Option =
        match data with
        | Some d -> Some (JsonSerializer.Serialize(d, JsonSerializerOptions(
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                WriteIndented = true)))
        | _ -> None

    let getDocIdOrDef docId = 
        match docId with
        | Some _ -> docId
        | _ -> Some (Guid.NewGuid().ToString())

