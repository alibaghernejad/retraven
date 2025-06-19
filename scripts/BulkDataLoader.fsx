#r "nuget: FSharp.Data"
open FSharp.Data

[<Literal>]
let contents = "documents.json"

type Doc = JsonProvider<contents>;;
let sample = Doc.GetSample()

printfn "%A" sample
