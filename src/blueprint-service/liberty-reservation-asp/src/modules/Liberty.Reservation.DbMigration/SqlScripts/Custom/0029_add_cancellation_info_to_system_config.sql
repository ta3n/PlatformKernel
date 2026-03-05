BEGIN;

-- 1) Io10005TemplateFormat.Body
UPDATE "public"."system_config"
SET template_format_data = jsonb_set(
  template_format_data,
  '{Io10005TemplateFormat,Body}',
  E'"{facilityName} 様\\n[{applicationName}]よりキャンセルの通知です。\\n\\n以下の予約が{actor}の手続きによりキャンセルとなりましたのでお知らせ致します。\\n\\n\\n■予約施設情報－－－－－－－－－－－－－－－－－－－－－－－－\\n{facilityName}\\n{facilityAddress}\\n{facilityTel}\\n\\n■予約内容－－－－－－－－－－－－－－－－－－－－－－－－\\n受付日時：　{reservationDateTime}\\n予約番号：　{code}\\n宿泊者名：  {MainUserName}\\n\\nチェックイン日　：　{CheckInDate}　より {RestNumber}泊\\nチェックイン時刻     :  {CheckInTime}\\nチェックアウト日：　{CheckOutDate}\\nチェックアウト時刻  :  {CheckOutTime}\\n部屋数　　　　　：　{RoomNumber}部屋\\n{totalPersonDetail}\\n部屋タイプ　　　：　{RoomGroupName}\\nプラン名　　　　：　{PlanName}\\n\\n■キャンセルについて－－－－－－－－－－－－－－－－－－－－－－－－\\n{cancellationInfo}\\n\\n■予約明細－－－－－－－－－－－－－－－－－－－－－－－－\\n{reservationDetail}\\n\\n■宿泊者情報－－－－－－－－－－－－－－－－－－－－－－－－\\n宿泊者氏名       :  {MainUserName} \\n宿泊者連絡先   : {MainUserTel}\\n宿泊者ご住所   : {MainUserAddress}\\n\\n予約者氏名       :  {ReserverName}\\n予約者連絡先   :  {ReserverTel}\\n予約者ご住所   :  {ReserverAddress}\\n\\n■精算情報－－－－－－－－－－－－－－－－－－－－－－－－\\n{payOff}\\n\\n■キャンセルについて－－－－－－－－－－－－－－－－－－－－－\\n{cancelRuleName}\\n{cancellingDescription}\\n\\n＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊[{applicationName}]\\n※本メールアドレスは送信専用です。ご返信頂きましても、対応しかねます。\\n\\nオンラインからの予約操作に関しては「施設管理画面」よりご確認下さい\\n<a href={ManagementUrl}>{ManagementUrl}</a>\\n＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊"'::jsonb,
  false
                           )
WHERE "is_deleted" = FALSE;

-- 2) Io10006TemplateFormat.Body
UPDATE "public"."system_config"
SET template_format_data = jsonb_set(
  template_format_data,
  '{Io10006TemplateFormat,Body}',
  E'"{ReserverName} 様\\n[{applicationName}]よりキャンセルの通知です。\\n\\n以下の予約のキャンセルが完了しましたのでお知らせ致します。\\n{managerCanceledDescription}\\nまたのご利用お待ちしております。\\n\\n\\n■予約施設情報－－－－－－－－－－－－－－－－－－－－－－－－\\n{facilityName}\\n{facilityAddress}\\n{facilityTel}\\n\\n■予約内容－－－－－－－－－－－－－－－－－－－－－－－－\\n受付日時：　{reservationDateTime}\\n予約番号：　{code}\\n宿泊者名：  {MainUserName}\\n\\nチェックイン日　：　{CheckInDate}　より {RestNumber}泊\\nチェックイン時刻     :  {CheckInTime}\\nチェックアウト日：　{CheckOutDate}\\nチェックアウト時刻  :  {CheckOutTime}\\n部屋数　　　　　：　{RoomNumber}部屋\\n{totalPersonDetail}\\n部屋タイプ　　　：　{RoomGroupName}\\nプラン名　　　　：　{PlanName}\\n\\n■キャンセルについて－－－－－－－－－－－－－－－－－－－－－－－－\\n{cancellationInfo}\\n\\n■予約明細－－－－－－－－－－－－－－－－－－－－－－－－\\n{reservationDetail}\\n\\n■精算情報－－－－－－－－－－－－－－－－－－－－－－－－\\n{payOff}\\n\\n＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊[{applicationName}]\\n※本メールアドレスは送信専用です。ご返信頂きましても、対応しかねます。\\n\\nオンラインからの予約操作に関しては「ユーザー管理画面」よりご確認下さい\\n<a href={UserUrl}>{UserUrl}</a>\\n＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊"'::jsonb,
  false
                           )
WHERE "is_deleted" = FALSE;

-- 3) Io10006EnTemplateFormat.Body
UPDATE "public"."system_config"
SET template_format_data = jsonb_set(
  template_format_data,
  '{Io10006EnTemplateFormat,Body}',
  E'"Dear {ReserverName},\\n\\nThis is a cancellation notification from [{applicationName}].\\nThe following reservation has been successfully canceled.\\n{managerCanceledDescription}\\nWe look forward to serving you again in the future.\\n\\n■ Reservation Facility Information－－－－－－－－－－－－－－－－－－－－－－－－\\n\\n{facilityName}\\n{facilityAddress}\\n{facilityTel}\\n\\n■ Reservation Details－－－－－－－－－－－－－－－－－－－－－－－－\\n\\nReception Date & Time: {reservationDateTime}\\nReservation Number: {code}\\nGuest Name: {MainUserName}\\n\\nCheck-in Date: {CheckInDate} for {RestNumber} night(s)\\nCheck-in Time: {CheckInTime}\\nCheck-out Date: {CheckOutDate}\\nCheck-out Time: {CheckOutTime}\\nNumber of Rooms: {RoomNumber} room(s)\\n{totalPersonDetail}\\nRoom Type: {RoomGroupName}\\nPlan Name: {PlanName}\\n\\n■About cancellation－－－－－－－－－－－－－－－－－－－－－－－－\\n{cancellationInfo}\\n\\n■ Reservation Details－－－－－－－－－－－－－－－－－－－－－－－－\\n\\n{reservationDetail}\\n\\n■ Payment Information－－－－－－－－－－－－－－－－－－－－－－－－\\n\\n{payOff}\\n\\n＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊\\n[{applicationName}]\\n※ This email address is for notification purposes only. Replies will not be received.\\n\\nFor reservation operations online, please check the User Management Page:\\n{UserUrl}\\n＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊"'::jsonb,
  false
                           )
WHERE "is_deleted" = FALSE;

-- 4) Io10011TemplateFormat.Body
UPDATE "public"."system_config"
SET template_format_data = jsonb_set(
  template_format_data,
  '{Io10011TemplateFormat,Body}',
  E'"{ReserverName} 様\\n[{applicationName}]よりキャンセルの通知です。\\n\\n以下の予約のキャンセルが完了しましたのでお知らせ致します。\\n{managerCanceledDescription}\\n\\nまたのご利用お待ちしております。\\n\\n\\n■予約施設情報－－－－－－－－－－－－－－－－－－－－－－－－\\n{facilityName}\\n{facilityAddress}\\n{facilityTel}\\n\\n■予約内容－－－－－－－－－－－－－－－－－－－－－－－－\\n受付日時：　{reservationDateTime}\\n予約番号：　{code}\\n宿泊者名：  {MainUserName}\\n\\nチェックイン日　：　{CheckInDate}　より {RestNumber}泊\\nチェックイン時刻     :  {CheckInTime}\\nチェックアウト日：　{CheckOutDate}\\nチェックアウト時刻  :  {CheckOutTime}\\n部屋数　　　　　：　{RoomNumber}部屋\\n{totalPersonDetail}\\n部屋タイプ　　　：　{RoomGroupName}\\nプラン名　　　　：　{PlanName}\\n\\n■キャンセルについて－－－－－－－－－－－－－－－－－－－－－－－－\\n{cancellationInfo}\\n\\n■予約明細－－－－－－－－－－－－－－－－－－－－－－－－\\n{reservationDetail}\\n\\n■精算情報－－－－－－－－－－－－－－－－－－－－－－－－\\n{payOff}\\n\\n＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊[{applicationName}]\\n※本メールアドレスは送信専用です。ご返信頂きましても、対応しかねます。\\n\\n＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊"'::jsonb,
  false
                           )
WHERE "is_deleted" = FALSE;

-- 5) Io10011EnTemplateFormat.Body
UPDATE "public"."system_config"
SET template_format_data = jsonb_set(
  template_format_data,
  '{Io10011EnTemplateFormat,Body}',
  E'"Dear {ReserverName},\\n\\nThis is a cancellation notification from [{applicationName}].\\n\\nThe cancellation process for the following reservation has been completed.\\n{managerCanceledDescription}\\n\\nWe look forward to serving you again in the future.\\n\\n■ Reservation Facility Information －－－－－－－－－－－－－－－－－－－－－－－－\\n{facilityName}\\n{facilityAddress}\\n{facilityTel}\\n\\n■ Reservation Details －－－－－－－－－－－－－－－－－－－－－－－－\\nReception Date and Time: {reservationDateTime}\\nReservation Number: {code}\\nGuest Name: {MainUserName}\\nCheck-in Date: {CheckInDate}, {RestNumber} nights\\nCheck-in Time: {CheckInTime}\\nCheck-out Date: {CheckOutDate}\\nCheck-out Time: {CheckOutTime}\\nNumber of Rooms: {RoomNumber} rooms\\n{totalPersonDetail}\\nRoom Type: {RoomGroupName}\\nPlan Name: {PlanName}\\n\\n■About cancellation－－－－－－－－－－－－－－－－－－－－－－－－\\n{cancellationInfo}\\n\\n■ Reservation Details －－－－－－－－－－－－－－－－－－－－－－－－\\n{reservationDetail}\\n\\n■ Payment Information －－－－－－－－－－－－－－－－－－－－－－－－\\n{payOff}\\n\\n＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊[{applicationName}]\\n※ This email address is for sending only. Replies to this email will not be received.\\n＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊"'::jsonb,
  false
                           )
WHERE "is_deleted" = FALSE;

COMMIT;
