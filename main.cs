var text = "";
var ai = "";
string[] arr = new string[0];
while (true)
{
    var key = Console.ReadKey(intercept: true);
    if (key.Key == ConsoleKey.Backspace)
    {
        text = text[..^1];
        ai = "";

    }
    else if (key.Key == ConsoleKey.Tab)
    {
        var list = new List<ChatMessage> { ChatMessage.CreateSystemMessage("文章の\"続き\"を5種類JSON形式で出力してください長さは\"3token\"程度にしてください\n形式\n[\"続きの文章\",\"続きの文章\",,...]"), text };
        var res = api.CompleteChat(list.ToArray());
        var x = res.Value.Content[0];
        var y = x.Text[2..^2];
        arr =
           x.Text[2..^2]
           .Split(",")
           .Select(x => x.Replace("\"", ""))
           .ToArray();
        Console.Clear();
        for (int i = 0; i < arr.Length; i++)
            ai += $"F{i + 1} " + arr[i] + "\n";
        Console.WriteLine();
        Console.WriteLine(text);
    }
    else if (key.Key == ConsoleKey.F1) { ai = ""; text += arr[0]; }
    else if (key.Key == ConsoleKey.F2) { ai = ""; text += arr[1]; }
    else if (key.Key == ConsoleKey.F3) { ai = ""; text += arr[2]; }
    else if (key.Key == ConsoleKey.F4) { ai = ""; text += arr[3]; }
    else if (key.Key == ConsoleKey.F5) { ai = ""; text += arr[4]; }

    else
    {
        text += key.KeyChar;
        ai = "";
    }
    Console.Clear();
    Console.Write(ai + "\n" + text);
    Thread.Sleep(60);
}




