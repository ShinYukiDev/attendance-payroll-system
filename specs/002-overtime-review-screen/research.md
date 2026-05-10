# Research: 残業確認画面

## Decision 1: UI は既存勤怠入力画面に合わせた Blazor の検索フォーム + read-only 一覧で構成する

- Rationale: 既存 UI パターンを再利用でき、最小差分で検索条件入力、結果表示、空表示メッセージを実装できる。
- Alternatives considered:
  - 別 SPA や JavaScript 主体の画面を追加する: 技術スタックが広がり要件外の複雑性を増やすため不採用。
  - 検索条件なしの全件表示にする: spec の社員ID・対象月検索要件を満たさないため不採用。

## Decision 2: 残業時間と月合計行は既存の勤怠実績から導出し、新規永続化は行わない

- Rationale: 残業確認画面は参照専用であり、既存 `AttendanceRecord.WorkMinutes` から日別残業時間と月合計値を算出すれば FR-003 から FR-006b を満たせる。
- Alternatives considered:
  - 残業確認専用テーブルを追加する: データ重複と同期責務が発生し、最小差分に反するため不採用。
  - 画面側のみで合計計算する: テストしづらく、契約の一貫性が落ちるため不採用。

## Decision 3: 集計責務は専用 Query Service に置き、日別行 DTO と月合計 DTO を返す

- Rationale: EF Core クエリの投影と集計を Application Layer に閉じ込めることで、Blazor ページは表示責務だけにできる。
- Alternatives considered:
  - Blazor コードビハインドで直接 DbContext を使う: 層分離が崩れ、テストもしにくいため不採用。
  - Domain Entity に残業確認専用の派生プロパティを増やす: 参照用途に対して永続モデルを肥大化させるため不採用。

## Decision 4: 検索条件は社員ID 7桁数字と対象月必須を UI + サービスで二重検証する

- Rationale: 入力時の分かりやすさと、画面外呼び出し時の安全性を両立できる。
- Alternatives considered:
  - UI 検証のみ: サービス単体呼び出し時に不正値を防げないため不採用。
  - サービス検証のみ: 利用者への即時フィードバックが不足するため不採用。

## Decision 5: アクセス制御は既存 v1 の非認証前提を維持しつつ、検索ログには社員IDや結果明細を残さない

- Rationale: 現行アプリに認証基盤がなく、今回機能で新規導入するとスコープ逸脱になる。一方で機微情報保護のため、アプリログやエラーメッセージには生の社員IDや勤怠明細を出さない方針を明記する。
- Alternatives considered:
  - この機能だけ独自認証を追加する: 既存アプリ全体との整合が取れず、最小差分にも反するため不採用。
  - 検索条件や検索結果を詳細ログへ残す: 個人情報露出リスクが高いため不採用。

## Decision 6: テストは xUnit でクエリ集計ロジックと検索条件検証を先行定義する

- Rationale: 日別残業時間、月合計所定労働時間、結果なしケースは純粋ロジックとして自動検証しやすく、回帰防止効果が高い。
- Alternatives considered:
  - 手動テストのみ: 集計ロジックの回帰検知が弱いため不採用。
  - UI テスト中心: 現時点のプロジェクト方針と比較して過剰であるため不採用。

## Decision 7: 画面ルートとナビゲーション追加は必要最小限に留める

- Rationale: 新画面は `AttendanceInput` と同じ `Components/Pages` 配下に追加し、既存ナビゲーションに 1 項目だけ追加すれば要件を満たせる。
- Alternatives considered:
  - 独立プロジェクトを追加する: 憲章の既存構造尊重に反するため不採用。
  - 既存勤怠入力画面へタブ統合する: 機能境界が曖昧になり、spec の画面分離と合わないため不採用。

## Unresolved Clarifications

NEEDS CLARIFICATION はなし。計画に必要な技術判断は本書で確定済み。
