# Contract: 残業確認画面 UI 契約

## Scope

本契約は、残業確認画面で提供する検索条件入力、日別一覧表示、月合計行表示、結果なし表示、検証エラー表示の振る舞いを定義する。

## Screen Data Contract

### OvertimeSearchInput

- EmployeeId: string（required, `^[0-9]{7}$`）
- TargetMonth: string（required, `yyyy-MM`）

### OvertimeReviewDailyItem

- WorkDate: yyyy-MM-dd
- WorkMinutes: int
- StandardMinutes: int（480 固定）
- OvertimeMinutes: int（`max(WorkMinutes - 480, 0)`）

### OvertimeReviewMonthlyTotal

- TotalWorkMinutes: int
- TotalStandardMinutes: int（日別行数 × 480）
- TotalOvertimeMinutes: int
- RowCount: int

## Operation Contract

### 1. 検索実行

- Trigger: 検索ボタン押下
- Input: OvertimeSearchInput
- Validation:
  - EmployeeId 必須
  - EmployeeId 7 桁数字
  - TargetMonth 必須
  - TargetMonth は有効な月形式
- Result:
  - Success with data: `OvertimeReviewDailyItem[]` と `OvertimeReviewMonthlyTotal` を表示
  - Success with no data: 結果なしメッセージを表示し、月合計行は表示しない
  - ValidationError: 項目エラー表示
  - Failure: 取得失敗メッセージを表示

### 2. 再検索

- Trigger: 検索条件変更後の再度の検索ボタン押下
- Result:
  - Success: 新しい条件に一致する日別行と月合計行へ置き換える

## UX Messages

- Validation:
  - 社員IDは半角数字7桁で入力してください。
  - 対象月を入力してください。
  - 対象月の形式が正しくありません。
- Empty:
  - 指定した条件に一致する勤怠データはありません。
- Failure:
  - 残業確認データの取得に失敗しました。

## Security & Logging Contract

- 本画面は既存 v1 アプリの非認証前提を継承し、新規の認証・認可機能は追加しない。
- アプリログや例外ログには、生の社員ID、実働時間明細、月合計明細を出力しない。
- エラーメッセージは利用者向けの必要最小限に限定し、内部例外詳細を表示しない。
