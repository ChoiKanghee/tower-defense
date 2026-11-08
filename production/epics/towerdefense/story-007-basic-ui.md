# Story 007: Giao diện cơ bản

> **Epic**: TowerDefense
> **Status**: Ready
> **Layer**: Presentation
> **Type**: UI
> **Estimate**: M
> **Manifest Version**: N/A (minimal — no control manifest)
> **Last Updated**: [set by /dev-story when implementation begins]

## Context

**GDD**: `design/game-brief.md`
**Requirement**: Brief MVP feature 7 — giao diện cơ bản: thanh thẻ chọn đơn vị, số tiếp tế, màn hình thắng/thua, chọn màn

**ADR Governing Implementation**: N/A (minimal — no ADRs)
**ADR Decision Summary**: N/A (minimal — no ADRs)
**ADR Version**: N/A (minimal — no ADRs)

**Engine**: Unity 6000.3.24f1 | **Risk**: HIGH
**Engine Notes**: none (no ADR engine-compatibility analysis at minimal)

**Control Manifest Rules (this layer)**: N/A (minimal — no control manifest)

---

## Acceptance Criteria

*From `design/game-brief.md`: MVP feature 7 và "Player goal & fail state":*

- [ ] Thanh thẻ liệt kê 3 đơn vị, mỗi thẻ hiện tên và giá
- [ ] Chọn một thẻ rồi bấm vào ô trống trên lưới để đặt đơn vị (điều khiển bằng chuột)
- [ ] Thẻ có giá cao hơn tiếp tế hiện có hiển thị khác đi (mờ) và không đặt được
- [ ] Số tiếp tế luôn hiện trên màn hình và cập nhật theo thời gian thực
- [ ] Màn hình thắng và màn hình thua hiện khi có sự kiện thắng hoặc thua; cả hai có nút chơi lại, màn thắng có thêm nút sang màn kế
- [ ] Màn hình chọn màn liệt kê 3 màn, bấm vào một màn để vào chơi (mọi màn mở sẵn, không lưu tiến trình)
- [ ] Chữ tiếng Việt có dấu hiển thị đúng trên mọi màn hình
- [ ] Chạy game thật: ảnh chụp từng màn hình (màn chơi có thanh thẻ và tiếp tế, thắng, thua, chọn màn) lưu trong `production/qa/evidence/`

---

## Implementation Notes

- Chỉ dùng chuột, không có thao tác chỉ hover mới thấy (giữ cửa mở cho bản web sau này, theo Platform Notes trong technical-preferences)
- UI Toolkit hay UGUI do `unity-ui-specialist` chọn; tra `docs/engine-reference/unity/modules/ui.md` trước khi dùng API
- Giá và tiếp tế lấy từ story 004; sự kiện thắng/thua lấy từ story 005
- Public API có doc comment

---

## Out of Scope

*Handled by neighbouring stories — do not implement here:*

- Story 004: logic tiếp tế
- Story 005: điều kiện thắng/thua
- Story 006: màn hình giao nhiệm vụ và nội dung 3 màn
- Làm đẹp bằng pixel art hoàn chỉnh, âm thanh, hiệu ứng (ngoài MVP)

---

## Files

- `Assets/Scripts/UI/CardBar.cs`
- `Assets/Scripts/UI/SupplyDisplay.cs`
- `Assets/Scripts/UI/OutcomeScreen.cs`
- `Assets/Scripts/UI/LevelSelectScreen.cs`

---

## QA Test Cases

*N/A — no qa-lead specs at this tier; implement against the Acceptance Criteria above*

---

## Test Evidence

*`qa.level: minimal`: test tự động được miễn (advisory), nhưng ảnh chụp màn hình khi chạy game thật thì không.*

**Story Type**: UI
**Required evidence**:
- UI: ảnh chụp từng màn hình đã làm, lưu trong `production/qa/evidence/` (bắt buộc)

**Status**: [ ] Not yet created

---

## Dependencies

- Depends on: `production/epics/towerdefense/story-004-supply-and-costs.md`, `production/epics/towerdefense/story-005-waves-and-outcome.md`, `production/epics/towerdefense/story-006-three-levels-and-briefing.md`
- Unlocks: None
