# Story 003: Hai loại địch

> **Epic**: TowerDefense
> **Status**: Ready
> **Layer**: Core
> **Type**: Logic
> **Estimate**: M
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: [set by /dev-story when implementation begins]

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 4 — 2 loại địch: lính bộ binh (nhanh, yếu) và xe bọc thép (chậm, trâu, đạn thường gần như vô hiệu)

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.24f1 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md`: MVP feature 4 and Core loop ("đọc loại địch đang tới, dồn đúng đơn vị"):*

- [ ] Hai loại địch (lính bộ binh, xe bọc thép) có chỉ số (máu, tốc độ, sát thương) đọc từ config
- [ ] Với config mặc định, lính bộ binh nhanh hơn và ít máu hơn xe bọc thép
- [ ] Xe bọc thép nhận từ đạn thường một lượng sát thương rất nhỏ (hệ số giảm lấy từ config, "gần như vô hiệu")
- [ ] Một lần nổ bom ba càng hạ được một xe bọc thép
- [ ] Địch gặp chiến lũy cùng làn thì dừng lại và đánh chiến lũy; không đi xuyên qua
- [ ] Khi chiến lũy bị phá, địch đi tiếp
- [ ] Địch hết máu thì biến mất và không còn bị nhắm bắn
- [ ] Chạy game thật: thấy lính bộ binh và xe bọc thép khác nhau về tốc độ và hình dạng; ảnh chụp lưu trong `production/qa/evidence/`

---

## Implementation Notes

- **Thứ tự làm (theo Build order):** làm lính bộ binh trước cùng tự vệ ở story 002 và chơi thử; xe bọc thép thêm sau
- Quan hệ khắc chế (xe bọc thép miễn nhiễm với đạn thường, bom hạ được) là điểm cần chơi thử và chỉnh số liệu, nên mọi hệ số nằm trong config
- Logic tách khỏi MonoBehaviour để test Edit Mode; public API có doc comment
- Hình đại diện tạm đủ để phân biệt hai loại

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Story 001: di chuyển cơ bản trên làn
- Story 002: chỉ số và hành vi của đơn vị phòng thủ
- Story 005: lịch xuất hiện của địch theo đợt

---

## Files

- `Assets/Scripts/Enemies/EnemyStats.cs`
- `Assets/Scripts/Enemies/Enemy.cs`
- `Assets/Tests/EditMode/Enemies/EnemyTests.cs`

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

*`qa.level: minimal`: test tự động được miễn (advisory), nhưng ảnh chụp màn hình khi chạy game thật thì không.*

**Story Type**: Logic
**Required evidence**:
- Logic: `Assets/Tests/EditMode/Enemies/EnemyTests.cs` (khuyến nghị; miễn ở `qa.level: minimal`)
- Ảnh chụp màn hình chạy game, lưu trong `production/qa/evidence/` (bắt buộc)

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/towerdefense/story-001-lane-grid-and-enemy-walk.md`, `production/epics/towerdefense/story-002-defender-units.md`
- Unlocks: `production/epics/towerdefense/story-005-waves-and-outcome.md`
