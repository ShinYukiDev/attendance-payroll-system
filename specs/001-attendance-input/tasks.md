# Tasks: 勤怠入力画面

**Input**: Design documents from `specs/001-attendance-input/`
**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/attendance-input-ui-contract.md`

**Tests**: 憲章のテスト先行原則に従い、各ユーザーストーリーで先にユニットテストを定義する。

**Organization**: ユーザーストーリー単位で独立実装・独立検証できるようにタスクを分割する。

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: 既存プロジェクトに勤怠入力機能の実装土台を追加する

- [x] T001 `src/AtendancePayrollSystem/AtendancePayrollSystem.csproj` に EF Core / SQLite 依存関係を追加する
- [x] T002 `src/AtendancePayrollSystem/appsettings.json` と `src/AtendancePayrollSystem/appsettings.Development.json` に SQLite 接続設定と監査ログ保持設定を追加する
- [x] T003 [P] `src/AtendancePayrollSystem/` 直下に `Domain/`, `Application/`, `Infrastructure/` の実装ディレクトリを作成する
- [x] T004 [P] `tests/AtendancePayrollSystem.Tests/` 直下に `Domain/`, `Application/`, `Infrastructure/` のテストディレクトリを作成する

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: 全ユーザーストーリーの前提となる永続化・検証・監査基盤を実装する

**⚠️ CRITICAL**: このフェーズ完了までユーザーストーリー実装に進まない

- [x] T005 `src/AtendancePayrollSystem/Infrastructure/AttendanceDbContext.cs` を作成し、勤怠関連テーブルを定義する
- [x] T006 [P] `src/AtendancePayrollSystem/Domain/Entities/AttendanceRecord.cs` と `src/AtendancePayrollSystem/Domain/Entities/AttendanceAuditLog.cs` を作成する
- [x] T007 `src/AtendancePayrollSystem/Infrastructure/Configurations/AttendanceRecordConfiguration.cs` と `src/AtendancePayrollSystem/Infrastructure/Configurations/AttendanceAuditLogConfiguration.cs` を作成し、一意制約・ConcurrencyToken を設定する
- [x] T008 `src/AtendancePayrollSystem/Domain/Services/AttendanceValidationService.cs` を作成し、社員ID・時刻・休憩時間・実働時間の業務検証を実装する
- [x] T009 `src/AtendancePayrollSystem/Application/Services/AttendanceAuditService.cs` と `src/AtendancePayrollSystem/Infrastructure/Jobs/AuditLogRetentionService.cs` を作成し、監査ログ生成と3か月保持削除を実装する
- [x] T010 `src/AtendancePayrollSystem/Program.cs` に DbContext、アプリケーションサービス、保持ジョブの DI 設定を追加する
- [x] T011 `src/AtendancePayrollSystem/Application/Contracts/AttendanceOperationResult.cs` を作成し、Validation/Conflict/NotFound を統一表現する

**Checkpoint**: 基盤完了後、各ユーザーストーリーを独立して着手可能

---

## Phase 3: User Story 1 - 勤怠を登録する (Priority: P1) 🎯 MVP

**Goal**: 新規勤怠を登録し、一覧へ即時反映できる

**Independent Test**: 必須項目を入力して登録すると一覧に追加され、実働時間が計算表示される

### Tests for User Story 1

- [x] T012 [P] [US1] `tests/AtendancePayrollSystem.Tests/Domain/AttendanceValidationServiceCreateTests.cs` に登録時バリデーション失敗ケース（必須・形式・時刻・休憩）を追加する
- [x] T013 [P] [US1] `tests/AtendancePayrollSystem.Tests/Application/AttendanceCreateServiceTests.cs` に登録成功と実働時間算出のテストを追加する

### Implementation for User Story 1

- [x] T014 [US1] `src/AtendancePayrollSystem/Application/Services/AttendanceCreateService.cs` を作成し、登録処理と監査ログ記録を実装する
- [x] T015 [US1] `src/AtendancePayrollSystem/Components/Pages/AttendanceInput.razor` に新規登録フォーム（社員ID・勤務日・出勤時刻・退勤時刻・休憩時間）を実装する
- [x] T016 [US1] `src/AtendancePayrollSystem/Components/Pages/AttendanceInput.razor.cs` を作成し、登録実行・結果メッセージ・一覧再読込を実装する
- [x] T017 [US1] `src/AtendancePayrollSystem/Application/Queries/AttendanceListQueryService.cs` を作成し、登録後に一覧表示用データを取得できるようにする

**Checkpoint**: US1 単体で登録機能が動作し、検証可能

---

## Phase 4: User Story 2 - 勤怠を修正する (Priority: P2)

**Goal**: 既存勤怠を更新し、実働時間再計算と競合検出ができる

**Independent Test**: 既存データを編集保存すると更新され、競合時は保存拒否と再読込案内が表示される

### Tests for User Story 2

- [x] T018 [P] [US2] `tests/AtendancePayrollSystem.Tests/Application/AttendanceUpdateServiceTests.cs` に更新成功・再計算のテストを追加する
- [x] T019 [P] [US2] `tests/AtendancePayrollSystem.Tests/Application/AttendanceUpdateConcurrencyTests.cs` に競合時保存拒否テストを追加する

### Implementation for User Story 2

- [x] T020 [US2] `src/AtendancePayrollSystem/Application/Services/AttendanceUpdateService.cs` を作成し、ConcurrencyToken を用いた更新処理を実装する
- [x] T021 [US2] `src/AtendancePayrollSystem/Components/Pages/AttendanceInput.razor` に編集 UI（行選択・編集状態）を実装する
- [x] T022 [US2] `src/AtendancePayrollSystem/Components/Pages/AttendanceInput.razor.cs` に更新実行、競合メッセージ、再読込導線を実装する
- [x] T023 [US2] `src/AtendancePayrollSystem/Application/Services/AttendanceUpdateService.cs` に更新時監査ログ（UPDATE/CONFLICT_REJECTED）記録を追加する。CONFLICT_REJECTED は必ず記録すること（FR-014, contracts.md Audit Contract）

**Checkpoint**: US2 単体で更新と競合拒否が検証可能

---

## Phase 5: User Story 3 - 勤怠を一覧確認・削除する (Priority: P3)

**Goal**: 一覧表示と物理削除を実現し、削除結果を即時反映する

**Independent Test**: 一覧表示後に対象行を削除すると一覧から消え、監査ログが記録される

### Tests for User Story 3

- [x] T024 [P] [US3] `tests/AtendancePayrollSystem.Tests/Application/AttendanceListQueryServiceTests.cs` に一覧表示項目（社員ID・勤務日・時刻・休憩・実働）のテストを追加する
- [x] T025 [P] [US3] `tests/AtendancePayrollSystem.Tests/Application/AttendanceDeleteServiceTests.cs` に物理削除成功・NotFound・競合時失敗のテストを追加する

### Implementation for User Story 3

- [x] T026 [US3] `src/AtendancePayrollSystem/Application/Services/AttendanceDeleteService.cs` を作成し、物理削除と監査ログ記録を実装する
- [x] T027 [US3] `src/AtendancePayrollSystem/Components/Pages/AttendanceInput.razor` に一覧テーブル表示（全必須列）を実装する
- [x] T028 [US3] `src/AtendancePayrollSystem/Components/Pages/AttendanceInput.razor` に削除確認 UI を実装する
- [x] T029 [US3] `src/AtendancePayrollSystem/Components/Pages/AttendanceInput.razor.cs` に削除実行・一覧更新・結果メッセージ表示を実装する

**Checkpoint**: US3 単体で一覧確認と削除が検証可能

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: 受け入れ性と横断品質を仕上げる

- [x] T030 [P] `tests/AtendancePayrollSystem.Tests/Domain/AttendanceValidationServiceEdgeCaseTests.cs` にエッジケース回帰テスト（重複・0分以下実働・時刻境界）を追加する
- [x] T031 `specs/001-attendance-input/quickstart.md` を実装後の実行手順・確認結果に合わせて更新する
- [x] T032 `specs/001-attendance-input/checklists/requirements.md` に FR-001〜FR-015 の実装・テスト追跡を記録する
- [x] T033 `tests/AtendancePayrollSystem.Tests/` を対象に `dotnet test` 実行結果を確認し、失敗時に該当実装を修正する
- [x] T034 `specs/001-attendance-input/tasks.md` の完了状況と成果物を照合し、最小差分・日本語成果物・追跡可能性の最終確認を行う

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: 依存なし
- **Phase 2 (Foundational)**: Phase 1 完了後に開始、全 US をブロック
- **Phase 3-5 (US1-US3)**: Phase 2 完了後に開始可能（優先度順は P1 → P2 → P3）
- **Phase 6 (Polish)**: US1-US3 完了後

### User Story Dependencies

- **US1 (P1)**: Foundational 完了後、単独で着手・検証可能
- **US2 (P2)**: Foundational と US1 の一覧/フォーム基盤を前提にするが、更新機能として独立検証可能
- **US3 (P3)**: Foundational と一覧取得基盤を前提にするが、一覧/削除として独立検証可能

### Within Each User Story

- テストタスクを先に作成し、失敗を確認してから実装する
- アプリケーションサービス実装後に UI 連携を行う
- 監査ログ記録を各操作（CREATE/UPDATE/DELETE/CONFLICT_REJECTED）に組み込む

## Parallel Opportunities

- Phase 1: T003 と T004 は並列可能
- Phase 2: T006 は T005 と並列可能（完了後 T007 へ）
- US1: T012 と T013 は並列可能
- US2: T018 と T019 は並列可能
- US3: T024 と T025 は並列可能
- Polish: T030 は T031 と並列可能

## Parallel Example: User Story 1

```bash
Task: "T012 [US1] 登録時バリデーション失敗ケースのユニットテスト追加"
Task: "T013 [US1] 登録成功と実働時間算出のユニットテスト追加"
```

## Implementation Strategy

### MVP First (US1)

1. Phase 1 と Phase 2 を完了する
2. US1 (Phase 3) を完了する
3. US1 の独立テストを実施して MVP として検証する

### Incremental Delivery

1. 基盤完成後に US1 をリリース可能単位で完成
2. US2 を追加して更新機能を提供
3. US3 を追加して一覧確認・削除まで完成
4. 最後に Polish で横断品質と追跡性を仕上げる
