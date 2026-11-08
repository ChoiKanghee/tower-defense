# Story 005: Đợt tấn công từ dữ liệu, điều kiện thắng/thua

> **Epic**: TowerDefense
> **Status**: Ready
> **Layer**: Feature
> **Type**: Integration
> **Estimate**: M
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: [set by /dev-story when implementation begins]

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 5 — đợt tấn công đọc từ file dữ liệu, kèm điều kiện thắng/thua

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.24f1 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md`: "Player goal & fail state" and MVP feature 5:*

- [ ] File dữ liệu mô tả mỗi đợt bằng danh sách (loại địch, làn, thời điểm xuất hiện)
- [ ] Địch xuất hiện đúng làn và đúng thời điểm theo dữ liệu (thời gian giả lập trong test)
- [ ] Dữ liệu sai (làn ngoài 1 đến 5, loại địch không tồn tại) báo lỗi rõ ràng và màn không chạy, không lỗi im lặng
- [ ] Màn không có đợt nào bị từ chối khi nạp
- [ ] **Thắng:** khi mọi đợt đã xuất hiện và không còn địch sống, phát đúng một sự kiện thắng
- [ ] **Thua:** khi bất kỳ địch nào đi hết làn, phát đúng một sự kiện thua ngay lập tức
- [ ] Sau khi thắng hoặc thua, địch ngừng xuất hiện và ngừng di chuyển
- [ ] Chạy game thật: chơi một màn mẫu đến thắng, và một lần để địch lọt dẫn đến thua; ảnh chụp cả hai trạng thái lưu trong `production/qa/evidence/`

---

## Implementation Notes

- Đây là bước "một màn hoàn chỉnh, chơi thử, chỉnh số liệu" của Build order: sau story này nên chơi thử nhiều lần và chỉnh số liệu trước khi làm tiếp
- Dữ liệu màn nằm ngoài code (ScriptableObject hoặc JSON); kiểm tra hợp lệ khi nạp
- Dùng sự kiện địch lọt vào từ story 001 làm điều kiện thua
- Logic tách khỏi MonoBehaviour để test Edit Mode; public API có doc comment

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Story 006: nhiều màn, chuyển màn và màn hình giao nhiệm vụ
- Story 007: màn hình thắng/thua (ở đây chỉ cần chữ hoặc log báo kết quả)

---

## Files

- `Assets/Scripts/Waves/WaveData.cs`
- `Assets/Scripts/Waves/WaveSpawner.cs`
- `Assets/Scripts/Waves/LevelOutcome.cs`
- `Assets/Data/Levels/Level01.asset`
- `Assets/Tests/EditMode/Waves/WaveAndOutcomeTests.cs`

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

*`qa.level: minimal`: test tự động được miễn (advisory), nhưng ảnh chụp màn hình khi chạy game thật thì không.*

**Story Type**: Integration
**Required evidence**:
- Integration: `Assets/Tests/EditMode/Waves/WaveAndOutcomeTests.cs` hoặc một bản playtest (khuyến nghị; miễn ở `qa.level: minimal`)
- Ảnh chụp màn hình chạy game, lưu trong `production/qa/evidence/` (bắt buộc)

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/towerdefense/story-001-lane-grid-and-enemy-walk.md`, `production/epics/towerdefense/story-003-enemy-types.md`, `production/epics/towerdefense/story-004-supply-and-costs.md`
- Unlocks: `production/epics/towerdefense/story-006-three-levels-and-briefing.md`, `production/epics/towerdefense/story-007-basic-ui.md`
