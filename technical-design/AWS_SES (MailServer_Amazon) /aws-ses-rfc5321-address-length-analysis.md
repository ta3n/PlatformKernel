# Phân Tích Giới Hạn Độ Dài Địa Chỉ Email Theo RFC 5321 trong AWS SES

## Tổng Quan

Hiện tại AWS SES (mail server của Amazon) đang tuân thủ chuẩn RFC 5322 theo thông tin từ:

- [AWS SES Email Format Documentation](https://docs.aws.amazon.com/ses/latest/dg/send-email-concepts-email-format.html)
- [Amazon SES Custom Mail-From Domains](https://aws.amazon.com/vi/blogs/messaging-and-targeting/amazon-ses-now-supports-custom-mail-from-domains)

Trong chuẩn RFC 5322 liên kết bao gồm các chuẩn của RFC 5321.

## Tài Liệu Tham Khảo

- [RFC 5322](https://www.ietf.org/rfc/rfc5322.txt) - Internet Message Format
- [RFC 5321](https://www.ietf.org/rfc/rfc5321.txt) - Simple Mail Transfer Protocol (SMTP)
- [AWS SES Raw Email Documentation](https://docs.aws.amazon.com/ses/latest/dg/send-email-raw.html)

## Quy Định Về Giới Hạn Độ Dài Email (RFC 5321)

Do AWS SES tuân thủ chuẩn RFC 5321 (quy định của SMTP), trong RFC 5321 có quy định về giới hạn độ dài địa chỉ email ở mục **4.5.3.1 Size Limits and Minimums**:

- **Phần local** (trước dấu `@`): tối đa **64 ký tự**
- **Phần domain** (sau dấu `@`): tối đa **255 ký tự**
- **Tổng chiều dài** địa chỉ email (local + @ + domain): tạm tính tối đa là **320 ký tự** (64 + 1 + 255)

> **Lưu ý:** Khi gửi message đến mail server, nội dung sẽ được encode lại. Do vậy ký tự ở đây sẽ tính theo độ dài sau khi thực hiện encode, không được vượt quá 320 ký tự.

## Case Study - Lỗi Vượt Quá Giới Hạn

### Thông tin sender gốc

```text
FromDisplayName = 【SAAADDDDDDDDDDDDDDDDDDDDDDDSsdssdsadasdasdasdasdasdasdasdasdasqwrgsdgasdasdqweqasfadsasdqweqeSAAADDDDDDDDDDDDDDDDDDDDDDDSsdssdsadasdasdasdasdasdasdasdasdasqwrgsdgasdasdqweqasfadsasdqdsasdq】AAAAAAAAAAAAAAAAAAAAAAAA
```

### Sau khi encode (MIME)

```text
=?utf-8?q?=E3=80=90SAAADDDDDDDDDDDDDDDDDDDDDDDSsdssdsadasdasdasdasdasdasda?=
 =?utf-8?q?sdasdasqwrgsdgasdasdqweqasfadsasdqweqeSAAADDDDDDDDDDDDDDDDDDD?=
 =?utf-8?q?DDDDSsdssdsadasdasdasdasdasdasdasdasdasqwrgsdgasdasdqweqasfad?=
 =?utf-8?b?c2FzZHFkc2FzZHHjgJEtIOadseS6rOS6iOe0hOOCt+OCueODhuODoOmAmuefpQ==?=
 <noreply@admin.com>
```

### Thông báo lỗi

```text
Error when sending email:
Transaction failed: Address length is more than 320 characters long:
'=?utf-8?q?=E3=80=90SAAADDDDDDDDDDDDDDDDDDDDDDDSsdssdsadasdasdasdasdasdasda?= =?utf-8?q?sdasdasqwrgsdgasdasdqweqasfadsasdqweqeSAAADDDDDDDDDDDDDDDDDDD?= =?utf-8?q?DDDDSsdssdsadasdasdasdasdasdasdasdasdasqwrgsdgasdasdqweqasfad?= =?utf-8?b?c2FzZHFkc2FzZHHjgJEtIOadseS6rOS6iOe0hOOCt+OCueODhuODoOmAmuefpQ==?= <noreply@admin.com>'.
```

**Kết luận:** Tổng số ký tự sau khi encode đã vượt quá 320 ký tự nên hệ thống không thể gửi email và trả về lỗi như trên.

RFC 5321 là một tài liệu chuẩn quan trọng trong hệ thống thư điện tử Internet, và nó có mối liên hệ chặt chẽ với RFC 5322 (mà bạn đang mở). Hai RFC này bổ sung cho nhau nhưng giải quyết những khía cạnh khác nhau của email:

📩 Vai trò của RFC 5321
RFC 5321 định nghĩa SMTP (Simple Mail Transfer Protocol) – giao thức dùng để truyền tải và định tuyến email giữa các máy chủ.

Nó tập trung vào “phần phong bì” (envelope) của email: ai gửi, ai nhận, đường đi qua các máy chủ, cách xử lý lỗi, v.v.

Ví dụ: các lệnh MAIL FROM, RCPT TO, DATA trong SMTP đều được mô tả trong RFC 5321.

📝 Vai trò của RFC 5322
RFC 5322 định nghĩa Internet Message Format (IMF) – tức là cấu trúc nội dung thư: tiêu đề (headers) như From:, To:, Subject:, và phần thân (body).

Nó không quan tâm đến việc thư được vận chuyển thế nào, mà chỉ quan tâm đến hình thức và cú pháp của nội dung.

🔗 Mối liên hệ giữa RFC 5321 và RFC 5322
Khi một email được gửi đi:

SMTP (RFC 5321) lo việc vận chuyển: mở kết nối, gửi lệnh, chuyển dữ liệu.

IMF (RFC 5322) quy định dữ liệu được gửi đi phải có cấu trúc thế nào (headers + body).

Nói cách khác:

RFC 5321 = “bưu tá” (cách thư được gửi đi).

RFC 5322 = “nội dung lá thư” (cách thư được viết ra).

👉 Vì vậy, RFC 5321 và RFC 5322 là hai mảnh ghép không thể tách rời: một cái lo giao thức truyền tải, một cái lo định dạng thông điệp
