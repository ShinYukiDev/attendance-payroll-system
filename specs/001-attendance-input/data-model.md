# Data Model: 勤怠入力画面

## Entity: AttendanceRecord（勤怠記録）

- Purpose: 1社員・1勤務日の勤怠情報を保持する。
- Primary Key: AttendanceRecordId
- Uniqueness: EmployeeId + WorkDate（一意）

### Fields

- AttendanceRecordId: Guid
- EmployeeId: string
  - Validation: 半角数字7桁（正規表現 ^[0-9]{7}$）
- WorkDate: DateOnly
  - Validation: 必須
- StartTime: TimeOnly
  - Validation: 必須
  - Format: HH:mm のみ受け付け（秒は 00 に固定）
- EndTime: TimeOnly
  - Validation: 必須、StartTime より後
  - Format: HH:mm のみ受け付け（秒は 00 に固定）
- BreakMinutes: int
  - Validation: 0以上、総勤務分未満
- WorkMinutes: int
  - Derived: (EndTime - StartTime) - BreakMinutes
  - Validation: 1以上
  - Display Format: HH:mm (例: 570分 = 9:30)
- ConcurrencyToken: int
  - Purpose: 楽観ロック用
- CreatedAtUtc: DateTime
- CreatedBy: string?
  - v1 設定値: "SYSTEM" 固定（認証なし）
- UpdatedAtUtc: DateTime
- UpdatedBy: string?
  - v1 設定値: "SYSTEM" 固定（認証なし）

### Relationships

- 1 AttendanceRecord : N AttendanceAuditLog

### State Transitions

- DraftInput -> Saved
- Saved -> Updated
- Saved -> Deleted（物理削除）
- Updated -> Updated（再更新）
- Updated -> Deleted（物理削除）

## Entity: AttendanceAuditLog（操作追跡ログ）

- Purpose: 登録・更新・削除の監査証跡を3か月保持する。
- Primary Key: AuditId

### Fields

- AuditId: Guid
- OccurredAtUtc: DateTime
- ActionType: string（CREATE / UPDATE / DELETE / CONFLICT_REJECTED）
- ActorId: string?
- TargetRecordId: Guid
- TargetEmployeeIdMasked: string
  - Rule: マスキング: 先頭 2 桁 + 末尾 1 桁 (例: 1234567 → 12_7)
  - Purpose: 追跡可能性を保持しながら PII 最小化
- TargetWorkDate: DateOnly
- ChangedFields: string
  - Rule: カンマ区切りまたは JSON で項目名のみ保持
- Result: string（SUCCESS / FAILURE）
- ReasonCode: string?

### Retention Rule

- 3か月を超過したレコードを日次ジョブで削除する。

## Business Rules Mapping

- FR-006 / FR-006a: EmployeeId, WorkDate, StartTime, EndTime の必須・形式検証
- FR-007: EndTime > StartTime
- FR-008: BreakMinutes の範囲と WorkMinutes 正値
- FR-009: EmployeeId + WorkDate 一意
- FR-012 / FR-015: AttendanceAuditLog の生成と3か月保持
- FR-014: ConcurrencyToken による競合検出
