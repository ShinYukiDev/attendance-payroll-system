# Contract: 勤怠入力画面 UI 契約

## Scope

本契約は、勤怠入力画面で提供する一覧表示・新規追加・更新・削除・競合通知の振る舞いを定義する。

## Screen Data Contract

### AttendanceListItem

- AttendanceRecordId: Guid
- EmployeeId: string（7桁数字）
- WorkDate: yyyy-MM-dd
- StartTime: HH:mm
- EndTime: HH:mm
- BreakMinutes: int
- WorkMinutes: int
- ConcurrencyToken: int

### AttendanceEditInput

- EmployeeId: string（required, ^[0-9]{7}$）
- WorkDate: date（required）
- StartTime: time（required）
- EndTime: time（required, StartTime より後）
- BreakMinutes: int（required, 0以上かつ総勤務分未満）

## Operation Contract

### 1. 一覧取得

- Trigger: 画面初期表示、保存成功後、競合発生後の再読込
- Result:
  - Success: AttendanceListItem[] を表示
  - Failure: 画面上部に取得失敗メッセージを表示

### 2. 新規追加

- Input: AttendanceEditInput
- Validation:
  - 必須項目
  - 社員ID形式
  - 時刻整合性
  - 休憩時間整合性
  - 同一社員ID + 勤務日の重複禁止
- Result:
  - Success: 追加成功メッセージ、一覧に1行追加
  - ValidationError: 項目エラー表示

### 3. 更新

- Input: AttendanceRecordId + AttendanceEditInput + ConcurrencyToken
- Result:
  - Success: 更新成功メッセージ、対象行を更新
  - ValidationError: 項目エラー表示
  - ConflictError: 保存拒否、再読込メッセージ表示
  - NotFound: 対象なしメッセージ表示

### 4. 削除

- Input: AttendanceRecordId + ConcurrencyToken
- Result:
  - Success: 削除成功メッセージ、対象行を除去
  - ConflictError: 保存拒否、再読込メッセージ表示
  - NotFound: 対象なしメッセージ表示

## Audit Contract

- CREATE / UPDATE / DELETE の成功時は監査ログを1件以上生成する。
- CONFLICT_REJECTED は失敗として監査ログを記録しなければならない（必須）。
- 監査ログ保持期間は最低3か月とする。

## UX Messages

- Success:
  - 登録しました。
  - 更新しました。
  - 削除しました。
- Validation:
  - 社員IDは半角数字7桁で入力してください。
  - 退勤時刻は出勤時刻より後にしてください。
  - 休憩時間は勤務時間未満で入力してください。
- Conflict:
  - 他の担当者が先に更新しました。再読込して再入力してください。
