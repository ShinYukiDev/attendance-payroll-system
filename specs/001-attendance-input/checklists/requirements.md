# Specification Quality Checklist: 勤怠入力画面

**Purpose**: Validate specification completeness and quality before proceeding to planning
**Created**: 2026-05-09
**Feature**: [spec.md](../spec.md)

## Content Quality

- [x] No implementation details (languages, frameworks, APIs)
- [x] Focused on user value and business needs
- [x] Written for non-technical stakeholders
- [x] All mandatory sections completed

## Requirement Completeness

- [x] No [NEEDS CLARIFICATION] markers remain
- [x] Requirements are testable and unambiguous
- [x] Success criteria are measurable
- [x] Success criteria are technology-agnostic (no implementation details)
- [x] All acceptance scenarios are defined
- [x] Edge cases are identified
- [x] Scope is clearly bounded
- [x] Dependencies and assumptions identified

## Feature Readiness

- [x] All functional requirements have clear acceptance criteria
- [x] User scenarios cover primary flows
- [x] Feature meets measurable outcomes defined in Success Criteria
- [x] No implementation details leak into specification

## Notes

- 検証結果: 2 回目の検証で全項目を満たした。初回確認で DG-003 を満たすアクセス制御・操作追跡の明示が不足していたため、[spec.md](../spec.md) に追記した。
- 補足: 対象範囲は①勤怠入力画面に限定し、残業確認・給与確認は意図的に除外した。

## FR Traceability (Implementation)

- [x] FR-001: 一覧表示（`AttendanceInput.razor` のテーブル + `AttendanceListQueryService`）
- [x] FR-002: 新規追加（`AttendanceCreateService` + 入力フォーム）
- [x] FR-003: 更新（`AttendanceUpdateService` + 編集 UI）
- [x] FR-004: 物理削除（`AttendanceDeleteService`）
- [x] FR-005: 実働時間算出（`AttendanceValidationService.CalculateWorkMinutes`）
- [x] FR-006: 必須項目検証（フォーム必須 + サービス検証）
- [x] FR-006a: 社員ID 7 桁数字検証（正規表現）
- [x] FR-007: 時刻整合検証（退勤 > 出勤）
- [x] FR-008: 休憩時間検証（0 以上かつ勤務時間未満）
- [x] FR-009: 同一社員ID+勤務日一意制約（DB 一意インデックス + 重複検証）
- [x] FR-010: 操作結果メッセージ表示（成功/失敗/競合/未検出）
- [x] FR-011: 認証・認可なし（Actor/By は SYSTEM 固定）
- [x] FR-012: 登録/更新/削除の監査ログ生成（`AttendanceAuditService`）
- [x] FR-013: スコープ限定（勤怠入力ページのみ追加）
- [x] FR-014: 更新競合検出と保存拒否（ConcurrencyToken 比較 + `CONFLICT_REJECTED`）
- [x] FR-015: 監査ログ 3 か月保持（`AuditLogRetentionService` + `RetentionDays=90`）

## FR Traceability (Tests)

- [x] FR-002/FR-005: `AttendanceCreateServiceTests`
- [x] FR-003/FR-005: `AttendanceUpdateServiceTests`
- [x] FR-004: `AttendanceDeleteServiceTests`
- [x] FR-001: `AttendanceListQueryServiceTests`
- [x] FR-006/006a/007/008: `AttendanceValidationServiceCreateTests`
- [x] FR-008/エッジ: `AttendanceValidationServiceEdgeCaseTests`
- [x] FR-014: `AttendanceUpdateConcurrencyTests`, `AttendanceDeleteServiceTests`
