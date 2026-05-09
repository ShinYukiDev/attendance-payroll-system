# Quickstart: 勤怠入力画面

## 1. 前提

- .NET SDK（プロジェクトの TargetFramework に対応する版）をインストール済みであること
- リポジトリ直下を作業ディレクトリとする

## 2. 復元とビルド

```powershell
dotnet restore
dotnet build AtendancePayrollSystem.sln
```

## 3. テスト実行（ユニットテスト）

```powershell
dotnet test tests/AtendancePayrollSystem.Tests/AtendancePayrollSystem.Tests.csproj
```

## 4. アプリ起動

```powershell
dotnet run --project src/AtendancePayrollSystem/AtendancePayrollSystem.csproj
```

## 5. 勤怠入力機能の手動確認

- 一覧表示:
  - 社員ID、勤務日、出勤時刻、退勤時刻、休憩時間、実働時間が表示される。
- 新規追加:
  - 有効な値で保存でき、一覧に追加される。
- バリデーション:
  - 社員IDが7桁数字以外の場合に保存拒否される。
  - 退勤時刻 <= 出勤時刻の場合に保存拒否される。
  - 休憩時間が不正な場合に保存拒否される。
- 更新:
  - 既存レコード更新後に実働時間が再計算される。
- 削除:
  - 削除確定でレコードが物理削除される。
- 競合:
  - 同一レコード同時更新で後続保存が拒否され、再読込導線が表示される。
- 監査ログ:
  - 登録・更新・削除で監査ログが生成される。

## 6. 保持ポリシー確認

- 監査ログは3か月保持される。
- 日次の保持期限削除処理で超過ログが削除される。
