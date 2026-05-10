# Data Model: 残業確認画面

## Entity: OvertimeSearchCriteria（残業確認検索条件）

- Purpose: 残業確認対象を社員IDと対象月で絞り込む。

### Search Criteria Fields

- EmployeeId: string
  - Validation: 半角数字7桁（正規表現 `^[0-9]{7}$`）
- TargetMonth: string
  - Validation: 必須、`yyyy-MM` 形式の暦月
  - Example: `2026-05`

### Search Criteria Business Rules

- EmployeeId と TargetMonth の両方がそろった場合のみ検索可能。
- TargetMonth は月単位検索であり、日付範囲へ変換して勤怠記録を抽出する。

## Entity: OvertimeReviewDailyRow（残業確認日別行）

- Purpose: 検索結果一覧に表示する1勤務日分の派生表示情報。
- Source: AttendanceRecord（既存勤怠記録）

### Daily Row Fields

- WorkDate: DateOnly
- WorkMinutes: int
  - Source: AttendanceRecord.WorkMinutes
  - Display Format: `H:mm`
- StandardMinutes: int
  - Derived: 480
  - Display Meaning: 所定労働時間 8時間固定
- OvertimeMinutes: int
  - Derived: `max(WorkMinutes - 480, 0)`
  - Display Format: `H:mm`

### Validation / Derivation Notes

- WorkMinutes が 480 以下なら OvertimeMinutes は 0。
- 本 entity は参照専用であり、利用者による編集は行わない。

## Entity: OvertimeReviewMonthlySummary（残業確認月合計行）

- Purpose: 検索結果の最下部に表示する月次集計情報。
- Source: 表示中の OvertimeReviewDailyRow 群

### Monthly Summary Fields

- TotalWorkMinutes: int
  - Derived: 日別行 `WorkMinutes` 合計
- TotalStandardMinutes: int
  - Derived: 日別行数 × 480
- TotalOvertimeMinutes: int
  - Derived: 日別行 `OvertimeMinutes` 合計
- RowCount: int
  - Derived: 表示中の日別行数

### Monthly Summary Business Rules

- 日別行が 0 件の場合、月合計行は表示しない。
- TotalStandardMinutes は表示中の日別行に対してのみ積み上げる。

## Reference Entity: AttendanceRecord（既存勤怠記録）

- Purpose: 実働時間の参照元となる既存永続データ。
- Relevant Fields:
  - EmployeeId: string
  - WorkDate: DateOnly
  - WorkMinutes: int

## Relationships

- 1 OvertimeSearchCriteria : N OvertimeReviewDailyRow
- 1 OvertimeSearchCriteria : 0..1 OvertimeReviewMonthlySummary
- N OvertimeReviewDailyRow : 1 AttendanceRecord（参照関係）

## Business Rules Mapping

- FR-002: OvertimeSearchCriteria の必須入力検証
- FR-003: OvertimeReviewDailyRow 群 + OvertimeReviewMonthlySummary の返却
- FR-004: WorkMinutes を日別行へ表示
- FR-005: StandardMinutes = 480
- FR-006: OvertimeMinutes = `max(WorkMinutes - 480, 0)`
- FR-006a / FR-006b: 月合計行に各合計値を表示
- FR-007: 日別行 0 件時は空結果メッセージを表示
- FR-008: EmployeeId / TargetMonth の形式不正時は検索拒否
