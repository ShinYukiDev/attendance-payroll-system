# Specification Quality Checklist: 残業確認画面

**Purpose**: Validate specification completeness and quality before proceeding to planning
**Created**: 2026-05-10
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

- 初回検証で全項目を満たした。
- 一覧表示は対象月の勤怠記録を複数行で表示する前提を Assumptions に明記した。
- 実装方針は記載せず、画面要件・算出ルール・受け入れ条件に限定した。

## 実装・テスト追跡（2026-05-10）

### Functional Requirements

- [x] FR-001: 検索エリア（社員ID、対象月、検索ボタン）を `OvertimeReview.razor` に実装。
- [x] FR-002: 社員ID・対象月の必須入力と形式検証を `OvertimeReviewValidationService` と UI 検証で実装。
- [x] FR-003: 日別行と月合計行の表示を `OvertimeReview.razor` と `OvertimeReviewQueryService` で実装。
- [x] FR-004: 実働時間の表示を `OvertimeReviewQueryService` の `AttendanceRecord.WorkMinutes` 参照で実装。
- [x] FR-005: 所定労働時間 8 時間固定を `OvertimeCalculationService.StandardMinutesPerDay` で実装。
- [x] FR-006: 残業時間 `max(実働時間 - 8時間, 0)` を `OvertimeCalculationService.CalculateOvertimeMinutes` で実装。
- [x] FR-006a: 月合計行の実働/所定/残業の集計を `OvertimeCalculationService.BuildMonthlySummary` で実装。
- [x] FR-006b: 月合計所定労働時間を「日別行数 × 8 時間」で実装し、テストで確認。
- [x] FR-007: 0 件検索時に「指定した条件に一致する勤怠データはありません。」表示を実装。
- [x] FR-008: 不備/形式不正時に理由メッセージを返す検証を実装（`OvertimeReviewValidationServiceTests`）。
- [x] FR-009: 本機能では参照専用とし、新規追加/更新/削除を未追加で維持。

### Documentation & Governance Requirements

- [x] DG-001: 仕様・計画・タスク・実装記録を日本語で維持。
- [x] DG-002: FR/DG と `tasks.md`、実装ファイル、テストファイルの追跡を本節へ記録。
- [x] DG-003: 入力検証・ログ非露出を実装し、既存 v1 非認証前提を継承（新規認証追加なし）。
- [x] DG-004: 変更範囲を残業確認画面、関連サービス、関連テストに限定。

### テスト結果

- [x] `dotnet test tests/AtendancePayrollSystem.Tests/AtendancePayrollSystem.Tests.csproj`
- [x] 実行結果: 合計 27 / 成功 27 / 失敗 0
