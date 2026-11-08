# Epic: TowerDefense

> **Tier**: minimal (không có GDD, ADR, control manifest; brief là bản thiết kế)
> **Brief**: `design/game-brief.md`
> **Engine**: Unity 6000.3.24f1 (6.3 LTS) | **Risk**: HIGH (theo `docs/engine-reference/unity/VERSION.md`)
> **Created**: 2026-11-08

## Goal

Chỉ huy tự vệ Hà Nội mùa đông 1946, giữ từng con phố bằng chiến lũy dựng từ bàn ghế, xe điện trước những đợt tiến công của quân Pháp.

## Scope (MVP của brief)

- Lưới 5 làn × 9 ô, địch đi dọc theo làn
- 3 đơn vị: tự vệ, chiến lũy, cảm tử quân bom ba càng
- Tiếp tế tăng dần theo thời gian; mỗi đơn vị có giá
- 2 loại địch: lính bộ binh, xe bọc thép
- Đợt tấn công đọc từ file dữ liệu, kèm điều kiện thắng/thua
- 3 màn có màn hình giao nhiệm vụ
- Giao diện cơ bản: thanh thẻ, tiếp tế, thắng/thua, chọn màn

## Out of scope

Chế độ tấn công, hơn 3 đơn vị hoặc 2 loại địch, nâng cấp, cắt cảnh động, lồng tiếng, lưu game, multiplayer, bản mobile, tái hiện chính xác trận đánh hay nhân vật có thật.

## Build order (từ brief)

1. Lưới + tự vệ + lính bộ binh bắn nhau: chứng minh lõi có vui không (stories 001, 002, 003; làm tự vệ và bộ binh trước, chơi thử)
2. Tiếp tế + thẻ đặt đơn vị (story 004)
3. Chiến lũy, bom ba càng, xe bọc thép: kiểm tra quan hệ khắc chế (stories 002, 003, phần còn lại)
4. Đợt tấn công từ dữ liệu + thắng/thua (story 005)
5. Một màn hoàn chỉnh, chơi thử, chỉnh số liệu (story 005)
6. 3 màn + màn hình giao nhiệm vụ (story 006)
7. Hoàn thiện giao diện (story 007)

## Stories

| # | Story | Type | Status | ADR |
|---|-------|------|--------|-----|
| 001 | Lưới 5 làn và địch đi theo làn | Logic | Ready | N/A |
| 002 | Ba đơn vị phòng thủ | Logic | Ready | N/A |
| 003 | Hai loại địch | Logic | Ready | N/A |
| 004 | Tiếp tế và giá đơn vị | Logic | Ready | N/A |
| 005 | Đợt tấn công từ dữ liệu, thắng/thua | Integration | Ready | N/A |
| 006 | Ba màn chơi và màn hình giao nhiệm vụ | UI | Ready | N/A |
| 007 | Giao diện cơ bản | UI | Ready | N/A |
