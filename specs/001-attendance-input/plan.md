# Implementation Plan: 勤怠入力画面

**Branch**: `001-implement-attendance-input` | **Date**: 2026-05-09 | **Spec**: `specs/001-attendance-input/spec.md`
**Input**: Feature specification from `specs/001-attendance-input/spec.md`

## Summary

勤怠入力画面（一覧表示・新規追加・変更・削除）を Blazor Web アプリとして実装し、実働時間計算、入力検証、重複防止、同時更新競合検出、操作監査ログ（3か月保持）を満たす。実装は既存の単一 Web プロジェクト構成を維持し、UI（Components/Pages）・業務ロジック（Application/Services）・永続化（EF Core + SQLite）を同一プロジェクト内で役割分離して最小差分で追加する。

## Technical Context

**Language/Version**: C# / .NET 10.0（現行リポジトリ実態に合わせる）  
**Primary Dependencies**: ASP.NET Core Blazor, Entity Framework Core, SQLite provider, DataAnnotations, xUnit  
**Storage**: SQLite（勤怠レコード・監査ログを保存）  
**Testing**: xUnit（ユニットテスト先行、必要最小限の回帰テスト）  
**Target Platform**: Windows 開発環境上の ASP.NET Core 実行環境（ローカル運用）
**Project Type**: 単一 Web アプリ（Blazor）  
**Performance Goals**: 画面操作で体感遅延が発生しないこと（一覧表示・CRUD 応答を通常操作で即時確認できるレベル）  
**Constraints**: 1勤務日は日跨ぎなし、社員IDは半角数字7桁固定、同一社員ID+勤務日一意、競合時は保存拒否  
**Scale/Scope**: v1 は勤怠入力画面のみ（残業確認・給与確認は対象外）

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

### Phase 0 前チェック

- [x] 成果物・レビューコメントを日本語で記述している
- [x] `spec.md` / `plan.md` / `tasks.md` の整合を維持する方針を定義した
- [x] テスト先行方針に基づく検証計画（xUnit 回帰防止）を含めた
- [x] 機微情報保護（入力検証・監査ログ最小化）を設計に含めた
- [x] 変更範囲を勤怠入力機能に限定し、最小差分方針を明示した

### Phase 1 後チェック

- [x] `research.md` で技術判断（競合制御・監査保持・検証方針）を確定済み
- [x] `data-model.md` でエンティティ・検証規則・状態遷移を定義済み
- [x] `contracts/attendance-input-ui-contract.md` で外部インターフェース（UI 契約）を定義済み
- [x] `quickstart.md` で検証手順を日本語で提供済み

## Project Structure

### Documentation (this feature)

```text
specs/001-attendance-input/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── attendance-input-ui-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
src/
└── AtendancePayrollSystem/
    ├── Components/
    │   ├── Layout/
    │   └── Pages/
    ├── Program.cs
    ├── appsettings.json
    └── AtendancePayrollSystem.csproj

tests/
└── AtendancePayrollSystem.Tests/
    ├── AtendancePayrollSystem.Tests.csproj
    └── UnitTest1.cs
```

**Structure Decision**: 既存の単一 Web プロジェクトを維持し、勤怠入力機能に必要なクラスとページを追加する。新規プロジェクトの追加は行わない。

## Phase 0 Research Plan

1. Blazor CRUD 画面の検証・表示・更新反映パターンを調査して採用案を決定する。
2. EF Core + SQLite の競合制御（楽観ロック）方針を決定する。
3. 監査ログ 3か月保持のデータ設計と削除運用方針を決定する。
4. 憲章適合（日本語、最小差分、テスト先行、機微情報保護）を満たす実装境界を確定する。

## Phase 1 Design Plan

1. 勤怠記録・監査ログ・操作主体のデータモデルを定義する。
2. UI 契約（一覧・登録・更新・削除・競合時再読込）を定義する。
3. クイックスタートに実行・テスト・手動検証手順をまとめる。
4. エージェントコンテキスト（`.github/copilot-instructions.md`）を本 plan 参照に更新する。

## Complexity Tracking

| Violation                                     | Why Needed                                                                            | Simpler Alternative Rejected Because                                                       |
| --------------------------------------------- | ------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------ |
| 該当なし | - | - |
