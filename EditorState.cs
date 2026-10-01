using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;

namespace GPTJapanesePredectEditor;

/// <summary>Pure editor state; no console, network, or API credentials are needed.</summary>
public sealed class EditorState
{
    private string[] _candidates = Array.Empty<string>();
    public string Text { get; private set; } = string.Empty;
    public IReadOnlyList<string> Candidates => Array.AsReadOnly(_candidates);

    public void Backspace()
    {
        if (Text.Length != 0)
        {
            // Remove one text element, avoiding broken Japanese combining marks
            // and surrogate pairs rather than cutting an arbitrary UTF-16 unit.
            var starts = StringInfo.ParseCombiningCharacters(Text);
            Text = Text[..starts[^1]];
        }
        ClearCandidates();
    }

    public void Append(char character)
    {
        if (char.IsControl(character)) return;
        Text += character;
        ClearCandidates();
    }

    public bool TryAcceptCandidate(int index)
    {
        if ((uint)index >= (uint)_candidates.Length) return false;
        Text += _candidates[index];
        ClearCandidates();
        return true;
    }

    public void ClearCandidates() => _candidates = Array.Empty<string>();

    public bool TrySetCandidatesJson(string json, out string error)
    {
        // Any failed or replaced response invalidates old suggestions.
        ClearCandidates();
        error = string.Empty;
        if (json is null || json.Length > 65536)
        {
            error = "候補の応答が空か、長すぎます。";
            return false;
        }
        try
        {
            using var document = JsonDocument.Parse(json, new JsonDocumentOptions { MaxDepth = 8 });
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() > 5)
                throw new JsonException("Expected an array containing at most five strings.");
            var candidates = new string[root.GetArrayLength()];
            int index = 0;
            foreach (var element in root.EnumerateArray())
            {
                if (element.ValueKind != JsonValueKind.String)
                    throw new JsonException("Each suggestion must be a string.");
                candidates[index++] = element.GetString()!;
            }
            _candidates = candidates;
            return true;
        }
        catch (JsonException)
        {
            error = "候補の応答が正しいJSON文字列配列ではありません。";
            return false;
        }
    }
}
