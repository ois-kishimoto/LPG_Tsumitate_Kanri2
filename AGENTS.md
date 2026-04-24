# AGENTS.md

このファイルは Codex (Codex.ai/code) がこのリポジトリで作業する際のガイダンスを提供します。

## プロジェクト概要

**LPG積立管理2** — 部署内の積立金を管理する社内向け ASP.NET Core MVC (.NET 10) Web アプリケーション。
「通常積立」「還暦積立」の2種類を対象とし、徴収チェック・収支帳簿・領収書管理を Web 化する。
認証なし（社内 LAN / VPN 限定アクセスを前提）。

## コマンド

```bash
# ビルド
dotnet build

# 実行 (HTTPS: https://localhost:7084 / HTTP: http://localhost:5190)
dotnet run --project LPG_Tsumitate_Kanri2/LPG_Tsumitate_Kanri2.csproj

# DB マイグレーション適用（プロジェクトディレクトリで実行）
cd LPG_Tsumitate_Kanri2
dotnet ef database update

# マイグレーション再作成（モデル変更時）
dotnet ef migrations remove --force
dotnet ef migrations add InitialCreate
dotnet ef database update
```

テストプロジェクトは存在しない。

## アーキテクチャ

標準的な ASP.NET Core MVC 構成：

- **[Program.cs](LPG_Tsumitate_Kanri2/Program.cs)** — DI 登録・ミドルウェア設定・開発時シードデータ投入
- **[Data/AppDbContext.cs](LPG_Tsumitate_Kanri2/Data/AppDbContext.cs)** — EF Core DbContext。`OnModelCreating` で全 FK の `OnDelete(DeleteBehavior.Restrict)` を設定（SavingsTypes への複数 CASCADE パス回避のため）。`HasData` で SavingsTypes・ContributionAmountRules の初期データを投入
- **[Data/DataSeeder.cs](LPG_Tsumitate_Kanri2/Data/DataSeeder.cs)** — 開発環境専用のテストデータ投入。Employees テーブルが空の場合のみ実行される
- **[Models/Entities/](LPG_Tsumitate_Kanri2/Models/Entities/)** — EF Core エンティティ
- **[Models/ViewModels/](LPG_Tsumitate_Kanri2/Models/ViewModels/)** — View 専用の ViewModel
- **[Services/ContributionCalculator.cs](LPG_Tsumitate_Kanri2/Services/ContributionCalculator.cs)** — Priority 昇順でルールを評価し最初に一致した金額を返す
- **[Services/LedgerService.cs](LPG_Tsumitate_Kanri2/Services/LedgerService.cs)** — `BalanceAfter` の計算・再計算を担当
- **[Controllers/](LPG_Tsumitate_Kanri2/Controllers/)** — Home / Collections / Employees / Ledger / ContributionRules / Receipts
- **[Views/](LPG_Tsumitate_Kanri2/Views/)** — Razor `.cshtml`。共通レイアウトは `Views/Shared/_Layout.cshtml`（Bootstrap 5 ダークナビバー）
- **[wwwroot/](LPG_Tsumitate_Kanri2/wwwroot/)** — 静的ファイル。Bootstrap・jQuery・jQuery Validation は `wwwroot/lib/` にベンダリング

## DB 構成（主要テーブル）

| テーブル | 概要 |
|---|---|
| `SavingsTypes` | 積立種別マスタ（通常積立 / 還暦積立）|
| `Employees` | 社員マスタ（IsActive フラグで在籍管理）|
| `ContributionAmountRules` | 積立金額ルール（Priority・条件・有効期間）|
| `CollectionSessions` | 徴収セッション（種別・年・月の組み合わせは UNIQUE）|
| `CollectionRecords` | 社員ごとの徴収記録（セッション作成時に自動生成）|
| `LedgerEntries` | 収支帳簿（`BalanceAfter` で残高を管理）|
| `Receipts` | 領収書ファイル情報（実ファイルは `App_Data/receipts/` に保存）|

## 主要な実装上の注意点

### [ValidateNever] の付与
Nullable 参照型が有効なプロジェクトのため、エンティティのナビゲーションプロパティ（`SavingsType`・`Employee` 等）は `[ValidateNever]` を付けている。付けないと POST 時に「The XXX field is required.」エラーが発生する。

### BalanceAfter の整合性
収支帳簿は同一 `SavingsTypeId` 内で `TransactionDate` 昇順・`EntryId` 昇順を正規順序とする。手動編集・削除後は `LedgerService.RecalculateFromAsync` で後続レコードを再計算する。

### 領収書ファイル
`wwwroot` 外の `App_Data/receipts/{GUID}.ext` に保存。直接 URL アクセス不可。`ReceiptsController.View`（インライン）と `ReceiptsController.Download`（添付）の2アクションで返却する。

### 徴収締め切りの解除
`CollectionSessions.IsCompleted = true` にした後でも `POST /Collections/{id}/Reopen` で解除可能。解除時は自動生成された `LedgerEntry` を削除し、後続の `BalanceAfter` を再計算する。

### バリデーションサマリー
全フォームで `@if (!ViewData.ModelState.IsValid)` で囲んで条件付きレンダリングしている（空の `alert-danger` が常時表示されると jQuery Validation が誤ってブロックするため）。

### CSS
`site.css` に `.field-validation-valid, .validation-summary-valid { display: none; }` が必要（標準テンプレートから欠落していた）。

## 接続文字列

`appsettings.json` の `ConnectionStrings:DefaultConnection`：
```
Server=localhost;Database=LPG_Tsumitate_Kanri2;Trusted_Connection=True;TrustServerCertificate=True;
```

## コミュニケーションルール

変更・修正を行った際は必ず解説を行うこと。
結論（何をしたか）を最初に述べ、次に理由・解説を記載する。

例：「〇〇ファイルを修正しました。」 理由：〜のため、〜に変更しました。

## 対象デバイスとレスポンシブ対応

- 本プロジェクトの利用想定デバイスは「スマホ」をメインとしている
- ただし、PC・スマホどちらで表示しても画面が崩れないようにレスポンシブで実装する

## ブランチルール

- ユーザーが新しいブランチを切るまでコードの変更を行わない
- 作業開始前に現在のブランチを確認する
- main ブランチ上では絶対にファイルを変更しない

## 禁止事項

- 設定ファイルの DB 接続文字列やポート番号を勝手に変更しない
- パッケージを確認なしに追加・削除しない
- 本番サーバーに関する操作は必ず確認を取ること
- コードや画面上に絵文字を使用しない
- VSCode 上でビルドや実行しない（作業者がローカルでビルドを実施する）
