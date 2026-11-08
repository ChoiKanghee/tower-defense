# Game Brief: TowerDefense

**One-sentence pitch:** Chỉ huy tự vệ Hà Nội mùa đông 1946, giữ từng con phố bằng chiến lũy dựng từ bàn ghế, xe điện trước những đợt tiến công của quân Pháp.

## Core loop
- Tiếp tế tích dần theo thời gian → dùng để đặt đơn vị lên 5 con phố
- Quân Pháp tiến theo đợt dọc từng phố; đơn vị của bạn tự đánh
- Đọc phố nào đang yếu, loại địch nào đang tới → dồn đúng đơn vị vào đó
- Qua màn → mở ngày chiến đấu tiếp theo với đoạn cốt truyện mới

## Player goal & fail state — what "working" looks like
- Thắng: giữ vững qua hết các đợt tiến công của màn.
- Thua: một tên địch bất kỳ đi hết phố và lọt vào phòng tuyến.

## MVP — what must exist to be the game
- Lưới 5 làn (5 con phố) × 9 ô, địch đi dọc theo làn
- 3 đơn vị: tự vệ nấp chiến lũy (bắn thường), chiến lũy (không bắn, chặn và chịu đòn), cảm tử quân bom ba càng (dùng một lần, phá xe bọc thép)
- Tiếp tế tăng dần theo thời gian; mỗi đơn vị tốn một lượng tiếp tế
- 2 loại địch: lính bộ binh (nhanh, yếu) và xe bọc thép (chậm, trâu, đạn thường gần như vô hiệu)
- Đợt tấn công đọc từ file dữ liệu, kèm điều kiện thắng/thua
- 3 màn lấy cảm hứng từ 60 ngày đêm Hà Nội 1946–1947, mỗi màn có màn hình giao nhiệm vụ vài dòng cốt truyện
- Giao diện cơ bản: thanh thẻ chọn đơn vị, số tiếp tế, màn hình thắng/thua, chọn màn

## Out of scope — not building this
- Chế độ tấn công (bước kế tiếp sau MVP, dùng chung lõi)
- Hơn 3 đơn vị hoặc 2 loại địch; nâng cấp; tiến trình dài hạn
- Cắt cảnh động, lồng tiếng
- Lưu game (mọi màn mở sẵn)
- Multiplayer, bản mobile
- Tái hiện chính xác trận đánh và nhân vật có thật — MVP dùng câu chuyện hư cấu lấy cảm hứng lịch sử

## Build order
1. Lưới + tự vệ + lính bộ binh đi trên làn, bắn nhau — chứng minh lõi có vui không
2. Tiếp tế + thanh thẻ đặt đơn vị
3. Chiến lũy, bom ba càng, xe bọc thép — kiểm tra quan hệ khắc chế
4. Đợt tấn công từ dữ liệu + điều kiện thắng/thua
5. Một màn hoàn chỉnh, chơi thử, chỉnh số liệu
6. 3 màn + màn hình giao nhiệm vụ
7. Hoàn thiện giao diện: chọn màn, thắng/thua

---
**Who it's for / what they feel:** Người thích chiến thuật nhẹ, ván 5–10 phút, cảm giác căng thẳng khi giữ từng con phố đến phút cuối.

**Art & audio direction:** Pixel art 2D như ảnh mẫu; phố cổ Hà Nội mùa đông, tông xám nâu ấm, khói lửa; nhạc trống trận, tiếng súng gọn và rõ.

**Reference game:** Plants vs. Zombies — giữ lối chơi lưới 5 làn và đặt đơn vị bằng tài nguyên tăng dần; bỏ mini-game, khu vườn và bộ sưu tập lớn.
