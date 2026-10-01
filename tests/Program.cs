using GPTJapanesePredectEditor;
using System.Text.Json;

int checks = 0;
void Check(bool condition, string description) { if (!condition) throw new Exception(description); checks++; }
var state = new EditorState();
state.Backspace(); Check(state.Text == "", "Empty Backspace");
for (int i = 0; i < 5; i++) Check(!state.TryAcceptCandidate(i), "F key before suggestions");
Check(!state.TryAcceptCandidate(-1), "Negative index");
foreach (char c in "日本語") state.Append(c);
Check(state.TrySetCandidatesJson("[\"続き,句読点\",\"引用\\\"と\\\\と\\n改行\"]", out _), "Valid JSON");
Check(state.Candidates.Count == 2, "Comma must not split string");
Check(!state.TryAcceptCandidate(4) && state.Text == "日本語", "Missing F5");
Check(state.TryAcceptCandidate(0) && state.Text == "日本語続き,句読点", "Accept suggestion");
Check(state.Candidates.Count == 0 && !state.TryAcceptCandidate(0), "Cannot reuse stale suggestion");
string before = state.Text;
foreach (string bad in new[] { "", "not json", "null", "{}", "[null]", "[1]", "[\"a\",]", "[\"1\",\"2\",\"3\",\"4\",\"5\",\"6\"]", new string('x', 65537) })
{
    state.TrySetCandidatesJson("[\"stale\"]", out _);
    Check(!state.TrySetCandidatesJson(bad, out var error) && error.Length > 0, "Malformed response rejected");
    Check(state.Candidates.Count == 0 && state.Text == before, "Failure preserves typed text only");
}
Check(state.TrySetCandidatesJson("[]", out _), "Empty candidates");
state.TrySetCandidatesJson("[\"x\"]", out _); state.Append('a');
Check(state.Candidates.Count == 0, "Edit invalidates suggestions");
before = state.Text; state.Append('\0'); state.Append('\r'); state.Append('\t');
Check(state.Text == before, "Control keys not added to text");
var unicode = new EditorState();
foreach (var text in new[] { "💞", "か\u3099", "👨‍👩‍👧‍👦" })
{
    Check(unicode.TrySetCandidatesJson(JsonSerializer.Serialize(new[] { text }), out _), "Unicode JSON");
    unicode.TryAcceptCandidate(0); unicode.Backspace();
    Check(unicode.Text == "", "Backspace removes one text element");
}
Console.WriteLine($"PASS {checks} checks");
