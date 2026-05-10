# Tasks: 残業確認画面

**Input**: Design documents from `specs/002-overtime-review-screen/`
**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/overtime-review-ui-contract.md`

**Tests**: 憲章と `quickstart.md` の方針に従い、各ユーザーストーリーで xUnit テストを先に追加して失敗を確認してから実装する。

**Organization**: ユーザーストーリー単位で独立実装・独立検証できるようにタスクを分割する。

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: 残業確認画面の追加先となる UI とテストの土台を用意する

- [x] T001 `src/AtendancePayrollSystem/Components/Pages/OvertimeReview.razor` と `src/AtendancePayrollSystem/Components/Pages/OvertimeReview.razor.cs` のページ雛形を作成する
- [x] T002 [P] `tests/AtendancePayrollSystem.Tests/Application/OvertimeReviewQueryServiceTests.cs` と `tests/AtendancePayrollSystem.Tests/Domain/OvertimeReviewValidationServiceTests.cs` のテストファイルを作成する

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: 全ユーザーストーリーで共有する検索条件・集計・DI 基盤を実装する

**⚠️ CRITICAL**: このフェーズ完了までユーザーストーリー実装に進まない

- [x] T003A [P] `tests/AtendancePayrollSystem.Tests/Domain/OvertimeCalculationServiceTests.cs` に残業算出ロジックの失敗テスト（8 時間超過/以下）を先行追加する
- [x] T003B [P] `tests/AtendancePayrollSystem.Tests/Domain/OvertimeReviewValidationServiceTests.cs` に検索条件検証ロジックの失敗テスト（未入力/形式不正）を先行追加する
- [x] T003 `src/AtendancePayrollSystem/Application/Contracts/OvertimeSearchCriteria.cs`、`src/AtendancePayrollSystem/Application/Contracts/OvertimeReviewDailyRow.cs`、`src/AtendancePayrollSystem/Application/Contracts/OvertimeReviewMonthlySummary.cs`、`src/AtendancePayrollSystem/Application/Contracts/OvertimeReviewResult.cs` を作成する
- [x] T004 [P] `src/AtendancePayrollSystem/Domain/Services/OvertimeCalculationService.cs` を作成し、所定 480 分基準の日別残業時間算出と月合計集計ロジックを実装する
- [x] T005 [P] `src/AtendancePayrollSystem/Domain/Services/OvertimeReviewValidationService.cs` を作成し、社員ID 7 桁数字検証、対象月必須検証、`yyyy-MM` 形式検証、月範囲変換を実装する
- [x] T006 `src/AtendancePayrollSystem/Program.cs` に残業確認用 Query Service / Domain Service の DI 登録を追加する

**Checkpoint**: 検索条件 DTO、計算ロジック、検証ロジック、DI が揃い、各ユーザーストーリーを独立実装できる

---

## Phase 3: User Story 1 - 社員と月を指定して残業を確認する (Priority: P1) 🎯 MVP

**Goal**: 社員IDと対象月で検索し、日別行と月合計行を含む残業確認結果を表示できるようにする

**Independent Test**: 社員IDと対象月を入力して検索し、一覧に日単位の実働時間、所定労働時間、残業時間と月合計行が表示されれば単独で価値を検証できる

### Tests for User Story 1

- [x] T007 [P] [US1] `tests/AtendancePayrollSystem.Tests/Application/OvertimeReviewQueryServiceTests.cs` に検索成功時の日別行取得と条件変更後の再検索ケースを追加する

### Implementation for User Story 1

- [x] T008 [US1] `src/AtendancePayrollSystem/Application/Queries/OvertimeReviewQueryService.cs` を作成し、社員IDと対象月で勤怠を取得して日別行と月合計行を返す処理を実装する
- [x] T009 [US1] `src/AtendancePayrollSystem/Components/Pages/OvertimeReview.razor` に社員ID、対象月、検索ボタン、日別一覧、月合計行を表示する UI を実装する
- [x] T010 [US1] `src/AtendancePayrollSystem/Components/Pages/OvertimeReview.razor.cs` に検索実行、再検索、検索結果置換の状態管理を実装する
- [x] T011 [US1] `src/AtendancePayrollSystem/Components/Layout/NavMenu.razor` に `/overtime-review` への導線を追加する

**Checkpoint**: US1 単体で検索から一覧表示まで動作し、MVP として確認できる

---

## Phase 4: User Story 2 - 残業時間の算出結果を確認する (Priority: P2)

**Goal**: 8 時間超過分のみを残業として扱う計算と、月合計行の集計結果を仕様どおりに安定表示する

**Independent Test**: 実働時間が 8 時間を超えるケースと超えないケースを含む勤怠データで検索し、日単位行と月合計行の残業時間が期待どおりに表示されれば単独で検証できる

### Tests for User Story 2

- [x] T012 [US2] `tests/AtendancePayrollSystem.Tests/Domain/OvertimeCalculationServiceTests.cs` に集計観点（合計値整合、表示用丸め/整形前提）のテストを追加する
- [x] T013 [US2] `tests/AtendancePayrollSystem.Tests/Application/OvertimeReviewQueryServiceTests.cs` に月合計行の実働・所定・残業集計テストを追加する

### Implementation for User Story 2

- [x] T014 [US2] `src/AtendancePayrollSystem/Application/Queries/OvertimeReviewQueryService.cs` を更新し、`OvertimeCalculationService` を用いた日別残業時間算出と月合計組み立てを完成させる
- [x] T015 [US2] `src/AtendancePayrollSystem/Components/Pages/OvertimeReview.razor` に `H:mm` 表示整形と月合計行の実働・所定・残業表示を仕様どおりに反映する

**Checkpoint**: US2 単体で残業計算と月合計集計の正しさを回帰テスト込みで検証できる

---

## Phase 5: User Story 3 - 条件不備や検索結果なしを判別する (Priority: P3)

**Goal**: 入力不備と該当データなしを明確に区別し、取得失敗時も必要最小限の情報だけを表示する

**Independent Test**: 未入力で検索した場合と、該当データがない条件で検索した場合を確認し、それぞれ適切なメッセージが表示されれば独立して検証できる

### Tests for User Story 3

- [x] T016 [US3] `tests/AtendancePayrollSystem.Tests/Domain/OvertimeReviewValidationServiceTests.cs` に利用者向けメッセージ判別（不足項目/形式不正）のテストを追加する
- [x] T017 [US3] `tests/AtendancePayrollSystem.Tests/Application/OvertimeReviewQueryServiceTests.cs` に検索結果 0 件時の空結果テストを追加する

### Implementation for User Story 3

- [x] T018 [US3] `src/AtendancePayrollSystem/Components/Pages/OvertimeReview.razor.cs` に検証エラー、結果なし、取得失敗の状態分岐と個人情報を含めないログ出力を実装する
- [x] T019 [US3] `src/AtendancePayrollSystem/Components/Pages/OvertimeReview.razor` に検証メッセージ、結果なしメッセージ、0 件時の月合計非表示を実装する
- [x] T020 [US3] `src/AtendancePayrollSystem/Application/Queries/OvertimeReviewQueryService.cs` に `OvertimeReviewValidationService` 連携と安全な失敗応答を追加する

**Checkpoint**: US3 単体で条件不備とデータ未存在を判別でき、取得失敗時も機微情報を露出しない

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: 受け入れ性、追跡性、横断品質を仕上げる

- [x] T021 [P] `specs/002-overtime-review-screen/quickstart.md` を実装後の起動手順、確認観点、ログ非露出確認結果に合わせて更新する
- [x] T022 `specs/002-overtime-review-screen/checklists/requirements.md` に FR-001 から FR-009 と DG-001 から DG-004 の実装・テスト追跡を記録する
- [x] T023 `tests/AtendancePayrollSystem.Tests/` を対象に `dotnet test` を実行し、失敗時は該当実装を修正する
- [x] T024 `specs/002-overtime-review-screen/tasks.md` の完了状況と成果物を照合し、日本語成果物、最小差分、機微情報非露出、要件追跡可能性の最終確認を行う
- [x] T025 `specs/002-overtime-review-screen/quickstart.md` に SC-001（3分以内表示）の手動計測手順と合否判定基準を追記し、検証結果を記録する
- [x] T026 `specs/002-overtime-review-screen/checklists/requirements.md` に DG-003 のアクセス制御前提（既存 v1 非認証継承・新規認証追加なし）の確認項目を追加する

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: 依存なし
- **Phase 2 (Foundational)**: Phase 1 完了後に開始し、全ユーザーストーリーをブロックする
- **Phase 3-5 (US1-US3)**: Phase 2 完了後に開始可能。推奨順は P1 → P2 → P3
- **Phase 6 (Polish)**: US1-US3 完了後に実施する

### User Story Dependencies

- **US1 (P1)**: Foundational 完了後、単独で着手・検証可能
- **US2 (P2)**: Foundational と US1 の検索結果表示基盤を前提にするが、計算正当性として独立検証可能
- **US3 (P3)**: Foundational と US1 の検索画面を前提にするが、入力不備・結果なし判別として独立検証可能

### Within Each User Story

- 先に xUnit テストを追加し、失敗を確認してから実装する
- Query Service / Domain Service のロジックを先に固め、その後に Blazor ページへ接続する
- UI 反映後にメッセージ、空表示、ログ非露出の確認を行う

## Parallel Opportunities

- Phase 1: T002 は T001 と並行で着手可能
- Phase 2: T004 と T005 は並列可能。完了後に T006 を進める
- Phase 2: T003A/T003B を先に失敗確認し、その後に T004/T005 を進める
- US1: T007 は UI 実装開始前に単独で進められる
- US2: T012 と T013 は別ファイルではないため順次進める。T012 の後に T014 を着手すると差分が安定する
- US3: T016 と T017 は別ファイルではないため順次進める。T016 の後に T018 を着手する
- Polish: T021 は T022 と並列可能

## Parallel Example: User Story 1

```bash
Task: "T007 [US1] 検索成功時の日別行取得と条件変更後の再検索テスト追加"
Task: "T009 [US1] 残業確認画面の検索フォームと一覧 UI 実装"
```

## Implementation Strategy

### MVP First (US1)

1. Phase 1 と Phase 2 を完了する
2. US1 (Phase 3) を完了する
3. US1 の独立テストと手動確認を実施して MVP として検証する

### Incremental Delivery

1. 基盤完成後に US1 を完成させ、検索と一覧表示をリリース可能にする
2. US2 を追加して残業計算と月合計集計の信頼性を固める
3. US3 を追加して運用時の入力不備と結果なし判別を完成させる
4. 最後に Polish で quickstart、要件追跡、総合テストを仕上げる

## Notes

- `[P]` は別ファイルで依存が薄い並列実行可能タスクを示す
- `[US1]` から `[US3]` は `spec.md` のユーザーストーリーとの追跡ラベルである
- 新規永続化テーブルや CRUD の追加は行わず、既存勤怠データ参照に限定する
- ログとエラーメッセージには社員IDや勤怠明細を含めない
- DG-003 のアクセス制御は「既存 v1 非認証前提を継承し、新規認証・認可機能を追加しない」方針で確認する
