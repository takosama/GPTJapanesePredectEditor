# GPTJapanesePredectEditor

tab AIPredict
F1~F5 AddMessage

## 安全な状態処理とテスト

`EditorState.cs` はネットワーク不要の編集ロジックです。空文字のBackspace、未取得・不足候補のF1〜F5、JSON内のカンマ・引用符・改行、不正な応答を扱います。通常の編集や候補採用で古い候補を破棄します。BackspaceはUnicodeの文字要素単位です。

.NET 8 SDKで `dotnet run --project tests/GPTJapanesePredectEditor.Regression.csproj` を実行できます。依存パッケージ、APIキー、課金APIは不要です。

`main.cs` は元からAPIクライアントの宣言とSDK型定義がない統合サンプルです。ホスト側で利用中のSDKの `ChatMessage` と構成済み `api` を提供してください。ライブラリprojectはこのサンプルをコンパイル対象に含めません。完全なオンラインアプリのビルド・API動作はこのテストで検証されません。APIクライアントのバージョンはリポジトリに情報がなく、勝手に新しいものへ固定していません。
