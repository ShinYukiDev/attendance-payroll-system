# Quickstart: 残業確認画面

## 1. 前提

- .NET SDK（`net10.0` 対応版）をインストール済みであること
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

- 残業確認画面の実装後は、検索条件検証と月次集計を対象にした xUnit テストを追加し、失敗を確認してから実装する。
- 追加済みテスト:
  - `tests/AtendancePayrollSystem.Tests/Application/OvertimeReviewQueryServiceTests.cs`
  - `tests/AtendancePayrollSystem.Tests/Domain/OvertimeCalculationServiceTests.cs`
  - `tests/AtendancePayrollSystem.Tests/Domain/OvertimeReviewValidationServiceTests.cs`

## 4. アプリ起動

```powershell
dotnet run --project src/AtendancePayrollSystem/AtendancePayrollSystem.csproj
```

- ブラウザで `https://localhost:xxxx/overtime-review` を開く（ポートは起動ログに従う）
- ナビゲーションメニューに残業確認画面への導線を追加する場合は、そこから遷移しても同じ画面を表示できることを確認する

## 5. 手動確認観点

### 検索条件

- 社員ID と対象月が未入力の場合は検索できない
- 社員ID が 7 桁数字以外の場合はエラー表示される
- 対象月が有効な月形式で入力できる

### 一覧表示

- 検索結果に日別行が表示される
- 各日別行に実働時間、所定労働時間、残業時間が表示される
- 所定労働時間は各日別行で 8 時間固定となる

### 月合計行

- 日別行が 1 件以上ある場合のみ最下部に表示される
- 月合計行の実働時間は日別行の合計と一致する
- 月合計行の所定労働時間は日別行数 × 8 時間と一致する
- 月合計行の残業時間は日別行の残業時間合計と一致する

### 結果なし

- 条件に一致する勤怠がない場合、空テーブルまたは結果なしメッセージで判別できる

### 個人情報保護

- アプリログやエラーメッセージに生の社員IDや勤怠明細全文を出さない
- 検索失敗時も、利用者向け画面には必要最小限の理由だけを表示する

## 6. 受け入れ確認の例

1. 社員ID と対象月を入力して検索し、日別行と月合計行が表示されることを確認する
2. 実働時間が 8 時間超の行で残業時間が正しく表示されることを確認する
3. 実働時間が 8 時間以下の行で残業時間が 0 になることを確認する
4. 月合計行の所定労働時間が表示日数に応じて増えることを確認する
5. 該当データなしの検索で、操作失敗ではなく結果なしと判別できることを確認する

## 7. 実装後の確認記録（2026-05-10）

### ログ非露出確認（DG-003）

- `OvertimeReviewQueryService` の失敗ログは固定文言のみを出力し、社員ID・勤怠明細をログに含めない。
- `OvertimeReview.razor.cs` の検証エラー/失敗ログも固定文言のみを出力し、検索条件の生値を含めない。

### SC-001（3分以内表示）手動計測手順

1. アプリを起動し、`/overtime-review` を開く。
2. 社員ID 7 桁と対象月（`yyyy-MM`）を入力して検索ボタンを押下する。
3. ボタン押下から、日別行または「指定した条件に一致する勤怠データはありません。」表示までの経過時間を計測する。
4. 条件を変更して再検索し、同様に計測する。

### SC-001 合否判定基準

- 合格: 初回検索・再検索の双方が 3 分以内に完了する。
- 不合格: いずれかが 3 分を超える。

### SC-001 検証結果

- 判定: PASS
- 記録: ローカル実行で初回検索・再検索とも 3 分以内に表示を確認した。
