// Integration sample: the host supplies its configured `api` chat client and
// ChatMessage type. See README.md; this file is not the offline library entrypoint.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GPTJapanesePredectEditor;

var editor = new EditorState();
while (true)
{
    var key = Console.ReadKey(intercept: true);
    string status = "";
    if (key.Key == ConsoleKey.Backspace)
    {
        editor.Backspace();
    }
    else if (key.Key == ConsoleKey.Tab)
    {
        editor.ClearCandidates();
        try
        {
            var list = new List<ChatMessage>
            {
                ChatMessage.CreateSystemMessage("文章の\"続き\"を5種類JSON形式で出力してください長さは\"3token\"程度にしてください\n形式\n[\"続きの文章\",\"続きの文章\"]"),
                editor.Text
            };
            var res = api.CompleteChat(list.ToArray());
            if (res.Value.Content.Count == 0)
                status = "候補の応答が空でした。";
            else
                editor.TrySetCandidatesJson(res.Value.Content[0].Text, out status);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            // Preserve typed text; do not retain suggestions from a previous request.
            editor.ClearCandidates();
            status = "候補を取得できませんでした。Tabで再試行できます。";
        }
    }
    else if (key.Key >= ConsoleKey.F1 && key.Key <= ConsoleKey.F5)
    {
        editor.TryAcceptCandidate((int)key.Key - (int)ConsoleKey.F1);
    }
    else
    {
        editor.Append(key.KeyChar);
    }

    Console.Clear();
    for (int i = 0; i < editor.Candidates.Count; i++)
        Console.WriteLine($"F{i + 1} {editor.Candidates[i]}");
    if (status.Length > 0) Console.WriteLine(status);
    Console.WriteLine(editor.Text);
    Thread.Sleep(60);
}
