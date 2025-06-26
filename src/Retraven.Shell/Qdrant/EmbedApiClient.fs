module EmbedApiClient 
open System.Net.Http
open System.Net.Http.Json
open System.Threading.Tasks
open Retraven.Core

let private httpClient = new HttpClient()
/// Call the /embed API
let getEmbeddings (text: string) : Task<Retraven.Core.EmbedResponse> = task {
    let content = JsonContent.Create({| text = text |})
    use! response = httpClient.PostAsync("http://localhost:8000/embed", content)
    response.EnsureSuccessStatusCode() |> ignore
    return! response.Content.ReadFromJsonAsync<EmbedResponse>()
}