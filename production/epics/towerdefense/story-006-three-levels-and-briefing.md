# Story 006: Ba màn chơi và màn hình giao nhiệm vụ

> **Epic**: TowerDefense
> **Status**: Ready
> **Layer**: Feature
> **Type**: UI
> **Estimate**: M
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: [set by /dev-story when implementation begins]

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 6 — 3 màn lấy cảm hứng từ 60 ngày đêm Hà Nội 1946–1947, mỗi màn có màn hình giao nhiệm vụ vài dòng cốt truyện

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.24f1 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md`: MVP feature 6, Core loop ("qua màn → mở ngày chiến đấu tiếp theo với đoạn cốt truyện mới") và Out of scope:*

- [ ] Có 3 màn, mỗi màn nạp từ một file dữ liệu riêng (các đợt tấn công, tiếp tế khởi đầu)
- [ ] Mỗi màn có màn hình giao nhiệm vụ gồm tiêu đề và vài dòng cốt truyện, đọc từ dữ liệu, không hardcode trong code
- [ ] Màn hình giao nhiệm vụ hiện trước khi màn chơi bắt đầu; địch chưa xuất hiện cho đến khi người chơi bấm nút bắt đầu
- [ ] Thắng màn 1 mở màn 2, thắng màn 2 mở màn 3; thắng màn 3 đưa người chơi về màn hình chọn màn
- [ ] Cốt truyện là hư cấu lấy cảm hứng lịch sử: không dùng tên nhân vật lịch sử có thật, không tái hiện chính xác một trận đánh
- [ ] Chữ tiếng Việt có dấu hiển thị đúng (font được chọn hỗ trợ đủ dấu)
- [ ] Chạy game thật: ảnh chụp màn hình giao nhiệm vụ của cả 3 màn lưu trong `production/qa/evidence/`

---

## Implementation Notes

- Văn bản cốt truyện nằm cùng file dữ liệu của màn để sửa không cần đụng code
- Kiểm tra sớm font hiển thị đủ dấu tiếng Việt trong Unity; đây là chỗ hay lỗi
- Dựa trên lõi và dữ liệu màn của story 005; nội dung 3 màn (đợt, số liệu) cần chơi thử để chỉnh
- Public API có doc comment

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Story 005: cơ chế đợt và điều kiện thắng/thua
- Story 007: màn hình chọn màn, thắng/thua và thanh thẻ
- Cắt cảnh động và lồng tiếng (ngoài phạm vi MVP)

---

## Files

- `Assets/Scripts/Levels/LevelData.cs`
- `Assets/Scripts/Levels/LevelFlow.cs`
- `Assets/Scripts/UI/BriefingScreen.cs`
- `Assets/Data/Levels/Level01.asset`, `Level02.asset`, `Level03.asset`

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

*`qa.level: minimal`: test tự động được miễn (advisory), nhưng ảnh chụp màn hình khi chạy game thật thì không.*

**Story Type**: UI
**Required evidence**:
- UI: ảnh chụp từng màn hình đã làm (3 màn giao nhiệm vụ), lưu trong `production/qa/evidence/` (bắt buộc)

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/towerdefense/story-005-waves-and-outcome.md`
- Unlocks: `production/epics/towerdefense/story-007-basic-ui.md`
