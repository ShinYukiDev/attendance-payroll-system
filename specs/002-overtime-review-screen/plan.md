# Implementation Plan: 残業確認画面

**Branch**: `002-overtime-review-screen` | **Date**: 2026-05-10 | **Spec**: `specs/002-overtime-review-screen/spec.md`
**Input**: Feature specification from `specs/002-overtime-review-screen/spec.md`

## Summary

残業確認画面を既存 Blazor Web アプリに追加し、社員IDと対象月で検索した勤怠記録を日別行で表示しつつ、最下部に月合計行を表示する。残業時間は既存勤怠データの実働時間から導出し、検索条件検証、結果なし表示、個人情報を露出しないログ方針を含めて、既存の Application Query と Blazor ページ構成に最小差分で追加する。

## Technical Context

**Language/Version**: C# / .NET 10.0  
**Primary Dependencies**: ASP.NET Core Blazor, Entity Framework Core, Microsoft.EntityFrameworkCore.Sqlite, DataAnnotations, xUnit  
**Storage**: SQLite（既存勤怠データを参照。残業確認用の新規永続化テーブルは追加しない）  
**Testing**: xUnit（クエリサービスと検索条件検証のユニットテストを先行定義）  
**Target Platform**: Windows 開発環境上で動作する ASP.NET Core Blazor Server アプリ  
**Project Type**: 単一 Web アプリ（Blazor + EF Core）  
**Performance Goals**: 1社員・1か月分の検索結果を体感待ちなしで表示できること  
**Constraints**: 社員IDは半角数字7桁、対象月は必須、表示は日別行 + 月合計行、残業時間は実働時間超過分のみ、CRUD は追加しない  
**Scale/Scope**: v1 の対象は残業確認画面のみ。検索対象は単一社員の単一月で、表示件数は月内勤怠日数相当を想定する

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

### Phase 0 前チェック

- [x] 成果物・レビューコメントを日本語で記述している
- [x] `spec.md` / `plan.md` / `tasks.md` の整合を維持する方針を定義した
- [x] テスト先行方針に基づく検証計画（xUnit 回帰防止）を含めた
- [x] 機微情報保護（検索条件検証、検索ログへの個人情報非露出）を設計に含めた
- [x] 変更範囲を残業確認画面の検索・表示に限定し、最小差分方針を明示した

### Phase 1 後チェック

- [x] `research.md` で UI・集計・ログ方針・テスト方針を確定済み
- [x] `data-model.md` で検索条件、日別行、月合計行のデータモデルを定義済み
- [x] `contracts/overtime-review-ui-contract.md` で画面入出力契約を定義済み
- [x] `quickstart.md` で起動、テスト、手動確認手順を日本語で提供済み

## Project Structure

### Documentation (this feature)

```text
specs/002-overtime-review-screen/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── overtime-review-ui-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
src/
└── AtendancePayrollSystem/
    ├── Application/
    │   ├── Contracts/
    │   └── Queries/
    ├── Components/
    │   ├── Layout/
    │   └── Pages/
    ├── Domain/
    │   └── Services/
    ├── Infrastructure/
    ├── Program.cs
    └── AtendancePayrollSystem.csproj

tests/
└── AtendancePayrollSystem.Tests/
    ├── Application/
    └── Domain/
```

**Structure Decision**: 既存の単一 Web プロジェクト構成を維持し、残業確認画面に必要な DTO、クエリサービス、Blazor ページ、テストのみを追加する。既存勤怠入力機能の責務分離に合わせ、画面表示は `Components/Pages`、取得・集計は `Application/Queries`、再利用可能な計算や入力検証は `Domain/Services` に閉じ込める。

## Phase 0 Research Plan

1. 既存勤怠入力実装を踏襲しつつ、検索型の read-only Blazor 画面構成を決定する。
2. 残業時間と月合計行を既存 `AttendanceRecord.WorkMinutes` から導出する責務配置を決定する。
3. 検索条件検証、アクセス制御前提、検索ログ非露出の方針を決定する。
4. xUnit で先行定義するクエリ集計テストと入力検証テストの範囲を確定する。

## Phase 1 Design Plan

1. 残業確認検索条件、日別表示行、月合計行のデータモデルを定義する。
2. UI 契約として検索入力、検索結果、結果なし表示、検証エラー表示の振る舞いを定義する。
3. クイックスタートに起動、テスト、手動確認、個人情報非露出確認の手順をまとめる。
4. エージェントコンテキスト（`.github/copilot-instructions.md`）を本 feature の plan 参照へ更新する。

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
| --------- | ---------- | ------------------------------------ |
| 該当なし  | -          | -                                    |
