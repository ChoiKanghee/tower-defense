# Story 002: Ba đơn vị phòng thủ

> **Epic**: TowerDefense
> **Status**: Ready
> **Layer**: Core
> **Type**: Logic
> **Estimate**: L
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: [set by /dev-story when implementation begins]

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
- Cách bom ba càng kích nổ (tự nổ khi địch vào ô, hay người chơi bấm) brief chưa nói rõ. Mặc định: tự nổ khi có địch vào ô của nó hoặc ô kề; ghi lại quyết định khi làm
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

- `Assets/Scripts/Defenders/DefenderStats.cs`
- `Assets/Scripts/Defenders/Defender.cs`
- `Assets/Scripts/Defenders/DefenderPlacement.cs`
- `Assets/Tests/EditMode/Defenders/DefenderTests.cs`

---

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
