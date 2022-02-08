open System.Text.RegularExpressions
open Funogram
open Funogram.Telegram
open Funogram.Telegram.Bot
open Funogram.Telegram.Types

// s/^(?:https:\/\/)?(?:www\.)?youtube\.com\/watch\?v\=(\w+)$/https:\/\/youtu.be\/$1
let youtubeFullPattern = @"(?:https:\/\/)?(?:www\.)?youtube\.com\/watch\?v\=(\w+)"
let youtubeShortReplacement = @"https://youtu.be/$1"

let onUpdate (context: UpdateContext) =
    match context.Update.ChannelPost with
    | Some ({ Text = Some messageText } as message) ->
        if Regex.IsMatch(messageText, youtubeFullPattern) then
            let newText = Regex.Replace(messageText, youtubeFullPattern, youtubeShortReplacement)
            async {
                let! result =
                    Api.editMessageTextBase (Some (ChatId.Int message.Chat.Id)) (Some message.MessageId) None newText None (Some false) None
                    |> Api.api context.Config
                printfn $"Message edited: %A{result}"
            } |> Async.RunSynchronously
    | _ -> ()

[<EntryPoint>]
let main args =
    let token = args |> Array.tryItem 0 |> Option.defaultWith (fun () -> failwith "Expected Telegram Token as first argument")
    let config = { defaultConfig with Token = token }
    printfn "Started"
    startBot config onUpdate None
    |> Async.RunSynchronously
    0

//
// Funogram actually sucks
//
