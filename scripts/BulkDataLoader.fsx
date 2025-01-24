#r "nuget: FSharp.Data"
open FSharp.Data

[<Literal>]
let contents = "gapfilm-contents.json"


type contents = FSharp.Data.JsonProvider<contents>;;

let doc = contents.GetSample()
doc.Hits.Hits.[0].Source.Title
