# Story 001: Lưới 5 làn và địch đi theo làn

> **Epic**: TowerDefense
> **Status**: Complete
> **Layer**: Foundation
> **Type**: Logic
> **Estimate**: M
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: 2026-11-08

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 1 — lưới 5 làn (5 con phố) × 9 ô, địch đi dọc theo làn

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.24f1 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md`: MVP feature 1 and "Player goal & fail state":*

- [ ] Lưới có đúng 5 làn × 9 ô; kích thước đọc từ config, không hardcode trong code
- [ ] Mỗi ô định danh bằng (làn, cột); đổi ô sang vị trí thế giới rồi đổi ngược lại trả đúng ô ban đầu
- [ ] Hỏi một ô nằm ngoài lưới thì trả kết quả "không hợp lệ", không ném lỗi
- [ ] Một địch sinh ra ở đầu làn được chỉ định và đi dọc làn đó theo tốc độ lấy từ config
- [ ] Địch không đổi làn trong suốt lúc đi
- [ ] Hai địch ở hai làn khác nhau di chuyển độc lập
- [ ] Khi địch đi hết làn, phát đúng một sự kiện "địch lọt vào phòng tuyến" kèm số làn (nền cho điều kiện thua ở story 005)
- [ ] Chạy game thật: thấy lưới 5×9 và một địch đi từ đầu đến cuối làn; ảnh chụp lưu trong `production/qa/evidence/`

---

## Implementation Notes

- Giá trị gameplay (kích thước lưới, tốc độ địch) nằm ngoài code (ScriptableObject hoặc file dữ liệu), theo coding standard của dự án
- Phần logic lưới và di chuyển tách khỏi MonoBehaviour để test Edit Mode được; thời gian truyền vào, không đọc `Time` trực tiếp
- Public API có doc comment
- Hình đại diện tạm (ô vuông, hình khối) là đủ; chưa cần pixel art

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Story 002: đơn vị phòng thủ và đặt lên ô
- Story 003: chỉ số và hành vi riêng của từng loại địch
- Story 005: điều kiện thắng/thua dựng trên sự kiện địch lọt vào

---

## Files

- `Assets/Resources/Config/lane-grid.json`
- `Assets/Scripts/Grid/LaneGridConfig.cs`
- `Assets/Scripts/Grid/LaneGrid.cs`
- `Assets/Scripts/Enemies/LaneWalker.cs`
- `Assets/Scripts/Enemies/LaneWalkerBehaviour.cs`
- `Assets/Scripts/Demo/LaneDemoBootstrap.cs`
- `Assets/Scripts/ScreenshotOnArg.cs`

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

*`qa.level: minimal`: test tự động được miễn (advisory), nhưng ảnh chụp màn hình khi chạy game thật thì không.*

**Story Type**: Logic
**Required evidence**:
- Logic: `Assets/Tests/EditMode/Grid/LaneGridTests.cs` (khuyến nghị; miễn ở `qa.level: minimal`)
- Ảnh chụp màn hình chạy game, lưu trong `production/qa/evidence/` (bắt buộc: story thay đổi thứ người chơi nhìn thấy)

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: None
- Unlocks: `production/epics/towerdefense/story-002-defender-units.md`, `production/epics/towerdefense/story-003-enemy-types.md`

---

## Completion Notes
**Completed**: 2026-11-08
**Criteria**: 8/8 passing. Untested criteria: AC-2, AC-3, AC-6 passed on the user's confirmation only (no automated test, no recorded observation; the demo has a single enemy, so AC-6 was never observed with two)
**Deviations**: ADVISORY — AC-2, 3, 6 lack independent evidence (needs an `.asmdef` plus Edit Mode tests, and a two-enemy demo for AC-6). OUT OF SCOPE — Unity rewrote 5 files during compile/build: `Assets/Settings/DefaultVolumeProfile.asset`, `UniversalRP.asset`, `UniversalRenderPipelineGlobalSettings.asset`, `ProjectSettings/ProjectSettings.asset`, and `ProjectSettings/UnityConnectSettings.asset` (`m_Enabled` 0 → 1: Unity services on; the player log shows failed connections to `config.uca.cloud.unity3d.com` and `cdp.cloud.unity3d.com`)
**Test Evidence**: waived at qa.level minimal. Run result: OBSERVED — `production/qa/evidence/story-001-lane-grid-and-enemy-walk/01-grid-enemy-mid-lane.png`, `02-enemy-reached-end.png`
**Reviewed by**: self, no independent review (solo mode; unity-specialist ran a static API check during /dev-story)
