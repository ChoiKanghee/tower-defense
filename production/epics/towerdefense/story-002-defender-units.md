# Story 002: Ba đơn vị phòng thủ

> **Epic**: TowerDefense
> **Status**: Complete
> **Layer**: Core
> **Type**: Logic
> **Estimate**: L
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: 2026-11-08

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 2 — 3 đơn vị: tự vệ nấp chiến lũy (bắn thường), chiến lũy (không bắn, chặn và chịu đòn), cảm tử quân bom ba càng (dùng một lần, phá xe bọc thép)

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.24f1 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md`: MVP feature 2 and Core loop:*

- [ ] Đặt được đơn vị lên một ô trống; đặt lên ô đã có đơn vị thì bị từ chối
- [ ] Ba loại đơn vị (tự vệ, chiến lũy, bom ba càng) có chỉ số (máu, sát thương, tốc độ bắn, tầm) đọc từ config
- [ ] Tự vệ bắn địch cùng làn đang ở phía trước nó; không bắn địch khác làn hay ở phía sau
- [ ] Tự vệ không bắn khi làn của nó không có địch
- [ ] Chiến lũy không bắn; nó chịu sát thương và bị phá khi hết máu, ô được giải phóng để đặt lại
- [ ] Bom ba càng gây sát thương lớn lên địch trong tầm kích nổ, nổ đúng một lần, rồi biến mất khỏi lưới
- [ ] Đổi chỉ số trong config có hiệu lực mà không phải sửa code
- [ ] Chạy game thật: đặt một tự vệ, thấy nó bắn địch đi qua làn; ảnh chụp lưu trong `production/qa/evidence/`

---

## Implementation Notes

- **Thứ tự làm (theo Build order):** làm tự vệ trước, cùng lính bộ binh ở story 003, và chơi thử xem lõi có vui không; sau đó mới thêm chiến lũy và bom ba càng
- **Quyết định (2026-11-08, người dùng chốt):** bom ba càng **tự nổ**, người chơi không bấm kích nổ. Vùng là **3×3 ô quanh bom** (ô của bom và 8 ô xung quanh, gồm cả làn trên và làn dưới):
  - Kích nổ: khi có địch bước vào bất kỳ ô nào trong vùng 3×3
  - Sát thương: mọi địch đang ở trong vùng 3×3 lúc nổ đều trúng, rồi bom biến mất
  - Bán kính lấy từ config (bán kính 1 = 3×3), ô ngoài lưới ở mép bị bỏ qua
- Giá trị gameplay nằm ngoài code (ScriptableObject hoặc file dữ liệu), theo coding standard
- Logic tách khỏi MonoBehaviour để test Edit Mode; public API có doc comment
- Hình đại diện tạm là đủ

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Story 001: lưới và di chuyển của địch
- Story 003: chỉ số riêng của từng loại địch, và việc địch dừng lại đánh chiến lũy
- Story 004: giá đơn vị và việc trừ tiếp tế khi đặt
- Story 007: thanh thẻ chọn đơn vị (tạm thời có thể đặt bằng phím hoặc nút debug)

---

## Files

- `Assets/Resources/Config/defenders.json`
- `Assets/Scripts/Defenders/DefenderStats.cs`
- `Assets/Scripts/Defenders/Defender.cs`
- `Assets/Scripts/Defenders/DefenderPlacement.cs`
- `Assets/Scripts/Defenders/DefenderBehaviour.cs`
- `Assets/Scripts/Enemies/EnemyHealth.cs`
- `Assets/Scripts/Enemies/LaneWalkerBehaviour.cs`
- `Assets/Scripts/Demo/LaneDemoBootstrap.cs`

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

*`qa.level: minimal`: test tự động được miễn (advisory), nhưng ảnh chụp màn hình khi chạy game thật thì không.*

**Story Type**: Logic
**Required evidence**:
- Logic: `Assets/Tests/EditMode/Defenders/DefenderTests.cs` (khuyến nghị; miễn ở `qa.level: minimal`)
- Ảnh chụp màn hình chạy game, lưu trong `production/qa/evidence/` (bắt buộc)

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/towerdefense/story-001-lane-grid-and-enemy-walk.md`
- Unlocks: `production/epics/towerdefense/story-003-enemy-types.md`, `production/epics/towerdefense/story-004-supply-and-costs.md`

---

## Completion Notes
**Completed**: 2026-11-08
**Criteria**: 8/8 passing. Untested criteria: AC-7 passed on the user's confirmation only (no recorded run; `defenders.json` is unchanged in the evidence stamp)
**Deviations**: ADVISORY — evidence images 01–07 are crops of the game window captured by screen control during play, not `ScreenCapture` output (placement needs mouse input). `EnemyMaxHealth` sits in `defenders.json` until story 003 moves it to per-enemy config. Demo-only extras in `LaneDemoBootstrap.cs`: right-click debug damage (50), 3 s spawn interval, colours, and on-screen strings hardcoded (throwaway demo). unity-specialist NITs: sentry cooldown discards leftover dt (fire rate frame-rate dependent); sentry fires at enemies at the off-grid spawn X; a damaged bomb is removed without exploding (debug path only); `DefenderStats.Get` default case returns Bomb; `Resources.Load` (superseded by Addressables, same as story 001); runtime IMGUI debug panel; runtime sprite never destroyed; no `.asmdef`, so Edit Mode tests cannot reference game code yet. OUT OF SCOPE — `ProjectSettings/UnityConnectSettings.asset` still uncommitted from story 001; `.vscode/` untracked
**Test Evidence**: waived at qa.level minimal. Run result: OBSERVED — `production/qa/evidence/story-002-defender-units/` (01 tu ve firing, 02 bomb 3x3 blast, 03 same-lane shots, 04 occupied cell rejected, 05–07 chien luy damaged, broken, cell reused)
**Reviewed by**: self, no independent review (solo mode; unity-specialist ran a static API check during /dev-story)
