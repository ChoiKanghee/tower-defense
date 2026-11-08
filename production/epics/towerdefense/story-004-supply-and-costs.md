# Story 004: Tiếp tế và giá đơn vị

> **Epic**: TowerDefense
> **Status**: Ready
> **Layer**: Core
> **Type**: Logic
> **Estimate**: S
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: [set by /dev-story when implementation begins]

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 3 — tiếp tế tăng dần theo thời gian; mỗi đơn vị tốn một lượng tiếp tế

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.24f1 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md`: MVP feature 3 and Core loop:*

- [ ] Tiếp tế bắt đầu từ một giá trị khởi đầu và tăng theo tốc độ (đơn vị mỗi giây); cả hai đọc từ config
- [ ] Mỗi loại đơn vị có giá đọc từ config
- [ ] Đặt đơn vị khi đủ tiếp tế: tiếp tế bị trừ đúng bằng giá
- [ ] Đặt đơn vị khi thiếu tiếp tế: bị từ chối, tiếp tế không đổi, đơn vị không xuất hiện
- [ ] Tiếp tế không bao giờ xuống dưới 0
- [ ] Tốc độ tăng tiếp tế không phụ thuộc framerate (tính theo thời gian được truyền vào)
- [ ] Chạy game thật: thấy tiếp tế tăng dần và bị trừ khi đặt đơn vị (số hiện tạm bằng chữ); ảnh chụp lưu trong `production/qa/evidence/`

---

## Implementation Notes

- Thời gian truyền vào để test xác định được, không đọc `Time` trực tiếp
- Giá và tốc độ tăng nằm trong config vì cần chỉnh nhiều khi cân bằng
- Tích hợp với việc đặt đơn vị ở story 002: kiểm tra đủ tiếp tế trước khi cho đặt
- Logic tách khỏi MonoBehaviour; public API có doc comment

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Story 002: việc đặt đơn vị lên ô
- Story 007: hiển thị số tiếp tế và thẻ mờ khi thiếu tiền (ở đây chỉ cần số hiện tạm)

---

## Files

- `Assets/Scripts/Supply/SupplyWallet.cs`
- `Assets/Tests/EditMode/Supply/SupplyTests.cs`

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

*`qa.level: minimal`: test tự động được miễn (advisory), nhưng ảnh chụp màn hình khi chạy game thật thì không.*

**Story Type**: Logic
**Required evidence**:
- Logic: `Assets/Tests/EditMode/Supply/SupplyTests.cs` (khuyến nghị; miễn ở `qa.level: minimal`)
- Ảnh chụp màn hình chạy game, lưu trong `production/qa/evidence/` (bắt buộc)

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/towerdefense/story-002-defender-units.md`
- Unlocks: `production/epics/towerdefense/story-005-waves-and-outcome.md`, `production/epics/towerdefense/story-007-basic-ui.md`
