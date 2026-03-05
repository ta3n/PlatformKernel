using Liberty.GmoPaymentGateway.Models;

namespace Liberty.GmoPaymentGateway.ErrorCodeGmo;

public static class GmoErrors
{
    public static readonly List<GmoErrorItem> GmoErrorItems =
    [
        #region G

        new()
        {
            Code = "G02",
            DetailCode = "42G020000",
            Message = "このカードでは取引をすることが出来ません。発行元カード会社にご確認ください。"
        },
        new()
        {
            Code = "G03",
            DetailCode = "42G030000",
            Message = "このカードでは取引をすることが出来ません。発行元カード会社にご確認ください。"
        },
        new()
        {
            Code = "G04",
            DetailCode = "42G040000",
            Message = "このカードでは取引をすることが出来ません。発行元カード会社にご確認ください。"
        },
        new()
        {
            Code = "G05",
            DetailCode = "42G050000",
            Message = "このカードでは取引をすることが出来ません。発行元カード会社にご確認ください。"
        },
        new()
        {
            Code = "G06",
            DetailCode = "42G060000",
            Message = "残高が不足しています。"
        },
        new()
        {
            Code = "G07",
            DetailCode = "42G070000",
            Message = "カード会社とのエラーが発生しました。"
        },
        new()
        {
            Code = "G12",
            DetailCode = "42G120000",
            Message = "このカードでは取引をすることが出来ません。発行元カード会社にご確認ください。"
        },
        new()
        {
            Code = "G22",
            DetailCode = "42G220000",
            Message = "このカードでは取引をすることが出来ません。発行元カード会社にご確認ください。"
        },
        new()
        {
            Code = "G30",
            DetailCode = "42G300000",
            Message = "このカードでは取引をすることが出来ません。発行元カード会社にご確認ください。"
        },
        new()
        {
            Code = "G42",
            DetailCode = "42G420000",
            Message = "カード情報に誤りがあります。カード情報をもう一度ご確認ください。"
        },
        new()
        {
            Code = "G44",
            DetailCode = "42G440000",
            Message = "カード情報に誤りがあります。カード情報をもう一度ご確認ください。"
        },
        new()
        {
            Code = "G45",
            DetailCode = "42G450000",
            Message = "カード会社とのエラーが発生しました。"
        },
        new()
        {
            Code = "G54",
            DetailCode = "42G540000",
            Message = "このカードでは取引をすることが出来ません。発行元カード会社にご確認ください。"
        },
        new()
        {
            Code = "G55",
            DetailCode = "42G550000",
            Message = "このカードでは取引をすることが出来ません。発行元カード会社にご確認ください。"
        },
        new()
        {
            Code = "G56",
            DetailCode = "42G560000",
            Message = "このカードでは取引をすることが出来ません。発行元カード会社にご確認ください。"
        },
        new()
        {
            Code = "G60",
            DetailCode = "42G600000",
            Message = "このカードでは取引をすることが出来ません。発行元カード会社にご確認ください。"
        },
        new()
        {
            Code = "G61",
            DetailCode = "42G610000",
            Message = "このカードでは取引をすることが出来ません。発行元カード会社にご確認ください。"
        },
        new()
        {
            Code = "G65",
            DetailCode = "42G650000",
            Message = "カード番号もしくは有効期限に誤りがあります。カード情報をもう一度ご確認ください。"
        },
        new()
        {
            Code = "G67",
            DetailCode = "42G670000",
            Message = "商品コードに誤りがあるために、決済を完了できませんでした。"
        },
        new()
        {
            Code = "G68",
            DetailCode = "42G680000",
            Message = "金額に誤りがあるために、決済を完了できませんでした。"
        },
        new()
        {
            Code = "G69",
            DetailCode = "42G690000",
            Message = "税送料に誤りがあるために、決済を完了できませんでした。"
        },
        new()
        {
            Code = "G70",
            DetailCode = "42G700000",
            Message = "ボーナス回数に誤りがあるために、決済を完了できませんでした。"
        },
        new()
        {
            Code = "G71",
            DetailCode = "42G710000",
            Message = "ボーナス月に誤りがあるために、決済を完了できませんでした。"
        },
        new()
        {
            Code = "G72",
            DetailCode = "42G720000",
            Message = "ボーナス額に誤りがあるために、決済を完了できませんでした。"
        },
        new()
        {
            Code = "G73",
            DetailCode = "42G730000",
            Message = "支払開始月に誤りがあるために、決済を完了できませんでした。"
        },
        new()
        {
            Code = "G74",
            DetailCode = "42G740000",
            Message = "分割回数に誤りがあるために、決済を完了できませんでした。"
        },
        new()
        {
            Code = "G75",
            DetailCode = "42G750000",
            Message = "分割金額に誤りがあるために、決済を完了できませんでした。"
        },
        new()
        {
            Code = "G76",
            DetailCode = "42G760000",
            Message = "初回金額に誤りがあるために、決済を完了できませんでした。"
        },
        new()
        {
            Code = "G77",
            DetailCode = "42G770000",
            Message = "業務区分に誤りがあるために、決済を完了できませんでした。"
        },
        new()
        {
            Code = "G78",
            DetailCode = "42G780000",
            Message = "支払区分に誤りがあるために、決済を完了できませんでした。"
        },
        new()
        {
            Code = "G79",
            DetailCode = "42G790000",
            Message = "照会区分に誤りがあるために、決済を完了できませんでした。"
        },
        new()
        {
            Code = "G80",
            DetailCode = "42G800000",
            Message = "取消区分に誤りがあるために、決済を完了できませんでした。"
        },
        new()
        {
            Code = "G81",
            DetailCode = "42G810000",
            Message = "取消取扱区分に誤りがあるために、決済を完了できませんでした。"
        },
        new()
        {
            Code = "G83",
            DetailCode = "42G830000",
            Message = "カード番号もしくは有効期限に誤りがあります。カード情報をもう一度ご確認ください。"
        },
        new()
        {
            Code = "G92",
            DetailCode = "42G920000",
            Message = "カード会社とのエラーが発生しました。"
        },
        new()
        {
            Code = "G95",
            DetailCode = "42G950000",
            Message = "カード会社とのエラーが発生しました。"
        },
        new()
        {
            Code = "G96",
            DetailCode = "42G960000",
            Message = "このカードでは取引をする事が出来ません。"
        },
        new()
        {
            Code = "G97",
            DetailCode = "42G970000",
            Message = "このカードでは取引をする事が出来ません。"
        },
        new()
        {
            Code = "G98",
            DetailCode = "42G980000",
            Message = "このカードでは取引をする事が出来ません。"
        },
        new()
        {
            Code = "G99",
            DetailCode = "42G990000",
            Message = "このカードでは取引をする事が出来ません。"
        },
        new()
        {
            Code = "G",
            DetailCode = "上記以外",
            Message = "カード会社とのエラーが発生しました。"
        },

        #endregion

        #region E

        new()
        {
            Code = "E01",
            DetailCode = "E01250010",
            Message = "カードパスワードが違います。"
        },
        new()
        {
            Code = "E01",
            DetailCode = "E01260010",
            Message = "指定されたカード番号または支払方法が正しくありません。"
        },
        new()
        {
            Code = "E01",
            DetailCode = "E01490005",
            Message = "未決 － 入力パラメータエラー/ 設定を確認してください。 利用金額・税送料の合計値が有効な範囲を超えています。"
        },
        new()
        {
            Code = "E11",
            DetailCode = "E11010100",
            Message = "利用できないカードが指定されました。"
        },
        new()
        {
            Code = "E21",
            DetailCode = "E21010001",
            Message = "3Dセキュア認証に失敗しました。もう一度やり直してください。"
        },
        new()
        {
            Code = "E21",
            DetailCode = "E21010007",
            Message = "3Dセキュア認証に失敗しました。"
        },
        new()
        {
            Code = "E21",
            DetailCode = "E21010202",
            Message = "3Dセキュア認証に失敗しました。3Dセキュア認証パスワード登録後にもう一度やり直してください。"
        },
        new()
        {
            Code = "E21",
            DetailCode = "E21020001",
            Message = "3Dセキュア認証に失敗しました。もう一度やり直してください。"
        },
        new()
        {
            Code = "E21",
            DetailCode = "E21020002",
            Message = "3Dセキュア認証がキャンセルされました。もう一度やり直してください。"
        },
        new()
        {
            Code = "E21",
            DetailCode = "E21020999",
            Message = "3Dセキュア認証に失敗しました。"
        },
        new()
        {
            Code = "E21",
            DetailCode = "E21010999",
            Message = "3Dセキュア認証に失敗しました。もう一度やり直してください。"
        },
        new()
        {
            Code = "E21",
            DetailCode = "E21020007",
            Message = "3Dセキュア認証に失敗しました。もう一度やり直してください。"
        },
        new()
        {
            Code = "E21",
            DetailCode = "E21030001",
            Message = "3Dセキュア認証に失敗しました。もう一度やり直してください。"
        },
        new()
        {
            Code = "E21",
            DetailCode = "E21030007",
            Message = "3Dセキュア認証されませんでした。時間を空けてもう一度やり直してください。時間をあけても解消しない場合、ご利用のカード会社へお問い合わせください。"
        },
        new()
        {
            Code = "E21",
            DetailCode = "E21030201",
            Message = "このカードでは取引をする事ができません。3Dセキュア認証に対応したカードをお使いください。"
        },
        new()
        {
            Code = "E21",
            DetailCode = "E21030202",
            Message = "3Dセキュア認証に失敗しました。認証失敗の理由については、カード会社へご確認ください。"
        },
        new()
        {
            Code = "E21",
            DetailCode = "E21040001",
            Message = "システムの内部エラーです。発生時刻や取引内容をご確認のうえ、お問い合わせください。"
        },
        new()
        {
            Code = "E21",
            DetailCode = "E21040002",
            Message = "システムの内部エラーです。発生時刻や取引内容をご確認のうえ、お問い合わせください。"
        },
        new()
        {
            Code = "E21",
            DetailCode = "E21010201",
            Message = "このカードでは取引をする事ができません。3Dセキュア認証に対応したカードをお使いください。"
        },
        new()
        {
            Code = "E41",
            DetailCode = "E41170099",
            Message = "カード番号に誤りがあります。再度確認して入力してください。"
        },
        new()
        {
            Code = "E61",
            DetailCode = "E61010001",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "E61",
            DetailCode = "E61010002",
            Message = "カード登録処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "E61",
            DetailCode = "E61010003",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "E90",
            DetailCode = "E90010001",
            Message = "現在処理を行っているため、もうしばらくお待ちください。"
        },
        new()
        {
            Code = "E91",
            DetailCode = "E91019999",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "E91",
            DetailCode = "E91029999",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "E91",
            DetailCode = "E91060001",
            Message = "システムエラーが発生しました。発生時刻や呼び出しパラメータをご確認のうえ、お問い合わせください。"
        },
        new()
        {
            Code = "E91",
            DetailCode = "E91099999",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "E",
            DetailCode = "上記以外",
            Message = "システムエラーが発生しました。"
        },

        #endregion

        #region EZ1

        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1000008",
            Message = "パラメータトークンの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1000010",
            Message = "パラメータトークンが一致しません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1001001",
            Message = "設定IDが指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1001005",
            Message = "設定IDが16桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1001008",
            Message = "設定IDの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1002001",
            Message = "取引共通情報が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1003001",
            Message = "オーダーIDが指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1003005",
            Message = "オーダーIDが27桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1003008",
            Message = "オーダーIDの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1004001",
            Message = "利用金額が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1004005",
            Message = "利用金額が12桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1004006",
            Message = "利用金額に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1004008",
            Message = "利用金額が不正です。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1005005",
            Message = "税送料が12桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1005006",
            Message = "税送料に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1006008",
            Message = "加盟店自由項目1の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1006012",
            Message = "加盟店自由項目1が100バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1007008",
            Message = "加盟店自由項目2の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1007012",
            Message = "加盟店自由項目2が100バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1008008",
            Message = "加盟店自由項目3の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1008012",
            Message = "加盟店自由項目3が100バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1009008",
            Message = "取引概要の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1009012",
            Message = "取引概要が64バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1010012",
            Message = "取引詳細が256桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1010008",
            Message = "取引詳細の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1011008",
            Message = "利用可能決済手段の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1011016",
            Message = "利用可能決済手段は利用可能な選択肢が存在しません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1012005",
            Message = "戻り先URLが256桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1013005",
            Message = "決済完了通知先メールアドレスが256桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1013008",
            Message = "決済完了通知先メールアドレスの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1014005",
            Message = "リトライ最大回数が2桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1014006",
            Message = "リトライ最大回数に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1015005",
            Message = "取引有効日数が2桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1015006",
            Message = "取引有効日数に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1018001",
            Message = "画面表示設定が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1019001",
            Message = "テンプレートIDが指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1019005",
            Message = "テンプレートIDが16桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1019008",
            Message = "テンプレートIDの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1020005",
            Message = "ロゴ画像のURLが256桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1021005",
            Message = "ショップ名が128桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1021008",
            Message = "ショップ名の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1023005",
            Message = "言語コードが2桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1023008",
            Message = "言語コードの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1024001",
            Message = "処理区分が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1024008",
            Message = "処理区分の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1024010",
            Message = "処理区分が一致しません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1025008",
            Message = "商品コードの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1026008",
            Message = "本人認証サービス利用フラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1026010",
            Message = "本人認証サービス利用フラグが一致しません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1027008",
            Message = "3Dセキュア表示店舗名の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1027012",
            Message = "3Dセキュア表示店舗名が25バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1028008",
            Message = "支払方法（単一指定）の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1028010",
            Message = "支払方法（単一指定）が一致しません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1029005",
            Message = "支払回数が2桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1029006",
            Message = "支払回数に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1029010",
            Message = "支払回数が一致しません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1031008",
            Message = "会員IDの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1036005",
            Message = "メールアドレスは半角256文字以内で入力してください。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1036008",
            Message = "メールアドレスに使用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1037012",
            Message = "氏名は半角40文字、全角20文字以内で入力してください。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1038008",
            Message = "フリガナに使用できない文字が含まれています。【使用可能文字：半角（スペース、英数字）※半角カタカナは不可、全角文字】"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1038012",
            Message = "入力できる文字数を超えています。【入力可能文字数：半角40文字（半角カタカナは不可）、全角20文字】"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1039005",
            Message = "電話番号は13桁以内で入力してください。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1039008",
            Message = "電話番号に使用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1040001",
            Message = "ショップIDが指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1040002",
            Message = "ショップIDが存在しません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1040008",
            Message = "ショップIDの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1042009",
            Message = "再実行回数が規定値を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1043015",
            Message = "URLの有効期限日を過ぎています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1045005",
            Message = "加盟店メールアドレスが256桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1045008",
            Message = "加盟店メールアドレスの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1046008",
            Message = "予約番号の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1046012",
            Message = "予約番号が20バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1047008",
            Message = "会員番号の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1047012",
            Message = "会員番号が20バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1048008",
            Message = "POSレジ表示欄1の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1048012",
            Message = "POSレジ表示欄1が32バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1049008",
            Message = "POSレジ表示欄2の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1049012",
            Message = "POSレジ表示欄2が32バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1050008",
            Message = "POSレジ表示欄3の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1050012",
            Message = "POSレジ表示欄3が32バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1051008",
            Message = "POSレジ表示欄4の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1051012",
            Message = "POSレジ表示欄4が32バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1052008",
            Message = "POSレジ表示欄5の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1052012",
            Message = "POSレジ表示欄5が32バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1053008",
            Message = "POSレジ表示欄6の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1053012",
            Message = "POSレジ表示欄6が32バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1054008",
            Message = "POSレジ表示欄7の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1054012",
            Message = "POSレジ表示欄7が32バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1055008",
            Message = "POSレジ表示欄8の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1055012",
            Message = "POSレジ表示欄8が32バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1056008",
            Message = "レシート表示欄1の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1056012",
            Message = "レシート表示欄1が60バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1057008",
            Message = "レシート表示欄2の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1057012",
            Message = "レシート表示欄2が60バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1058008",
            Message = "レシート表示欄3の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1058012",
            Message = "レシート表示欄3が60バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1059008",
            Message = "レシート表示欄4の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1059012",
            Message = "レシート表示欄4が60バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1060008",
            Message = "レシート表示欄5の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1060012",
            Message = "レシート表示欄5が60バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1061008",
            Message = "レシート表示欄6の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1061012",
            Message = "レシート表示欄6が60バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1062008",
            Message = "レシート表示欄7の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1062012",
            Message = "レシート表示欄7が60バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1063008",
            Message = "レシート表示欄8の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1063012",
            Message = "レシート表示欄8が60バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1064008",
            Message = "レシート表示欄9の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1064012",
            Message = "レシート表示欄9が60バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1065008",
            Message = "レシート表示欄10の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1065012",
            Message = "レシート表示欄10が60バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1066001",
            Message = "お問い合わせ先が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1066008",
            Message = "お問い合わせ先の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1066012",
            Message = "お問い合わせ先が60バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1067001",
            Message = "お問い合わせ先電話番号が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1067005",
            Message = "お問い合わせ先電話番号が12桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1067008",
            Message = "お問い合わせ先電話番号の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1068001",
            Message = "お問い合わせ先受付時間が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1068005",
            Message = "お問い合わせ先受付時間が11桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1068008",
            Message = "お問い合わせ先受付時間の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1069001",
            Message = "CSRFトークンが指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1070014",
            Message = "すでにオーダーIDが存在しています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1073001",
            Message = "会員編集情報が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1031001",
            Message = "会員IDが指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1074012",
            Message = "会員名が255バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1075008",
            Message = "カード番号取得元決済手段の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1076005",
            Message = "カード番号取得元オーダーIDが27桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1076008",
            Message = "カード番号取得元オーダーIDの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1077008",
            Message = "カラーパターンの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1084001",
            Message = "摘要が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1084008",
            Message = "摘要の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1084012",
            Message = "摘要が48バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1085001",
            Message = "表示サービス名が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1085008",
            Message = "表示サービス名の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1085012",
            Message = "表示サービス名が48バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1086001",
            Message = "表示電話番号が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1086005",
            Message = "表示電話番号が15桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1086008",
            Message = "表示電話番号の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1088012",
            Message = "ドコモ表示項目1が40バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1088013",
            Message = "ドコモ表示項目1に利用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1089012",
            Message = "ドコモ表示項目2が40バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1089013",
            Message = "ドコモ表示項目2に利用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1090012",
            Message = "利用店舗名が32バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1090013",
            Message = "利用店舗名に利用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1091005",
            Message = "連絡先電話番号が13桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1091008",
            Message = "連絡先電話番号の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1092005",
            Message = "メールアドレスが96桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1092008",
            Message = "メールアドレスの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1093012",
            Message = "問い合わせURLが96バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1093013",
            Message = "問い合わせURLに利用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1094012",
            Message = "連絡先情報が96バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1095001",
            Message = "カード編集番号が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1095008",
            Message = "カード編集番号に使用できない文字が含まれています。【使用可能文字：半角文字 ※半角カタカナおよび全角文字は不可】"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1095012",
            Message = "カード編集番号が27バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1097001",
            Message = "商品名が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1097012",
            Message = "商品名が4000バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1098012",
            Message = "商品画像URLが500バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1099012",
            Message = "LINE member IDが50バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1101012",
            Message = "言語コードが10バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1103012",
            Message = "認証連携トークンが256バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1104001",
            Message = "メルペイ商品情報が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1105001",
            Message = "メルペイ商品カテゴリIDが指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1105012",
            Message = "メルペイ商品カテゴリIDが4バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1106010",
            Message = "URLパラメータのJSON構造が不正です。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1107001",
            Message = "URLパラメータが指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1108001",
            Message = "送信元メールアドレス(FROM)、メール送信元名が指定されていません。両方設定するか、両方未設定にしてください。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1109001",
            Message = "送信先メールアドレス(TO)が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1109008",
            Message = "送信先メールアドレス(TO)の書式が不正です。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1109012",
            Message = "送信先メールアドレス(TO)が256バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1110001",
            Message = "顧客名が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1110005",
            Message = "顧客名が40桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1112001",
            Message = "テンプレートNo. が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1112008",
            Message = "テンプレートNo. の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1134008",
            Message = "決済URL案内メール送信フラグ／カード編集URL案内メール送信フラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1137008",
            Message = "送信先メールアドレス(BCC)の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1137012",
            Message = "送信先メールアドレス(BCC)が256バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1138001",
            Message = "商品IDが指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1138005",
            Message = "商品IDが100桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1138008",
            Message = "商品IDの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1139005",
            Message = "商品サブIDが77桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1139008",
            Message = "商品サブIDの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1140001",
            Message = "商品名が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1140012",
            Message = "商品名が255バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1141013",
            Message = "商品名に利用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1142013",
            Message = "商品説明に利用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1143009",
            Message = "商品画像URL一覧が9件を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1144012",
            Message = "商品画像URL1が256バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1145012",
            Message = "商品画像URL2が256バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1146012",
            Message = "商品画像URL3が256バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1147012",
            Message = "商品画像URL4が256バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1148012",
            Message = "商品画像URL5が256バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1149012",
            Message = "商品画像URL6が256バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1150012",
            Message = "商品画像URL7が256バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1151012",
            Message = "商品画像URL8が256バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1152012",
            Message = "商品画像URL9が256バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1153005",
            Message = "カテゴリ名が40桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1153013",
            Message = "カテゴリ名に利用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1154005",
            Message = "サイズが40桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1154013",
            Message = "サイズに利用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1155005",
            Message = "ブランド名が40桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1155013",
            Message = "ブランド名に利用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1156005",
            Message = "色が40桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1156013",
            Message = "色に利用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1157005",
            Message = "定価が7桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1157006",
            Message = "定価に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1157011",
            Message = "定価が1～1000000の範囲ではありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1158005",
            Message = "購入時価格が7桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1158006",
            Message = "購入時価格に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1158011",
            Message = "購入時価格が1～1000000の範囲ではありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1159005",
            Message = "購入数が4桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1159006",
            Message = "購入数に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1160005",
            Message = "製品管理コードが40桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1160013",
            Message = "製品管理コードに利用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1161011",
            Message = "JANコードが8～13文字の範囲ではありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1161013",
            Message = "JANコードに利用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1162010",
            Message = "税送料算出方法が一致しません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1163001",
            Message = "税送料計算URLが指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1163012",
            Message = "税送料計算URLが256バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1164010",
            Message = "個人情報利用フラグが一致しません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1165008",
            Message = "3DS2.0未対応時取り扱いの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1166008",
            Message = "カード会員最終更新日の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1167008",
            Message = "カード会員作成日の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1168008",
            Message = "カード会員パスワード変更日の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1169005",
            Message = "過去6ヶ月間の購入回数が4桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1169006",
            Message = "過去6ヶ月間の購入回数に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1170008",
            Message = "カード登録日の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1171005",
            Message = "過去24時間のカード追加の試行回数が3桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1171006",
            Message = "過去24時間のカード追加の試行回数に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1172008",
            Message = "配送先住所の初回使用日の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1173008",
            Message = "カード会員名と配送先名の一致/不一致の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1174008",
            Message = "カード会員の不審行為情報の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1175005",
            Message = "過去24時間の取引回数が3桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1175006",
            Message = "過去24時間の取引回数に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1176005",
            Message = "前年の取引回数が3桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1176006",
            Message = "前年の取引回数に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1188008",
            Message = "カード会員のメールアドレスの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1188012",
            Message = "メールアドレスは半角254文字以内で入力してください。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1188013",
            Message = "メールアドレスの書式が正しくないか、使用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1189005",
            Message = "自宅電話の国コードが3桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1189008",
            Message = "自宅電話の国コードに数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1190005",
            Message = "自宅電話番号が15桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1190008",
            Message = "自宅電話番号に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1191005",
            Message = "携帯電話の国コードが3桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1191008",
            Message = "携帯電話の国コードに数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1192005",
            Message = "携帯電話番号が15桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1192008",
            Message = "携帯電話番号に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1193005",
            Message = "職場電話の国コードが3桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1193008",
            Message = "職場電話の国コードに数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1194005",
            Message = "職場電話番号が15桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1194008",
            Message = "職場電話番号に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1202005",
            Message = "納品先電子メールアドレスが254桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1203008",
            Message = "商品納品時間枠の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1204005",
            Message = "プリペイドカードまたはギフトカードの総購入金額が15桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1204006",
            Message = "プリペイドカードまたはギフトカードの総購入金額に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1205005",
            Message = "購入されたプリペイドカードまたはギフトカード / コードの総数が2桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1205006",
            Message = "購入されたプリペイドカードまたはギフトカード / コードの総数に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1206005",
            Message = "購入されたプリペイドカードまたはギフトカードの通貨コードが3桁ではありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1207008",
            Message = "商品の発売予定日の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1208008",
            Message = "商品の販売時期情報の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1209008",
            Message = "商品の注文情報の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1213008",
            Message = "購入ありがとうメール送信フラグ／登録完了メール送信フラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1214008",
            Message = "送信元メールアドレス(FROM)の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1214012",
            Message = "送信元メールアドレス(FROM)が256バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1215005",
            Message = "送信元名が40桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1216008",
            Message = "決済可能期限の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1216015",
            Message = "URLの表示期限が切れています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1217008",
            Message = "編集可能期限の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1217015",
            Message = "URLの表示期限が切れています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1218001",
            Message = "ショップパスワードが指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1218008",
            Message = "ショップパスワードの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1219008",
            Message = "振込有効日数の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1219012",
            Message = "振込有効日数が2バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1220012",
            Message = "振込事由が64バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1220013",
            Message = "振込事由に利用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1221012",
            Message = "振込依頼者氏名が64バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1221013",
            Message = "振込依頼者氏名に利用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1222008",
            Message = "振込依頼者メールアドレスの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1222012",
            Message = "振込依頼者メールアドレスが256バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1223003",
            Message = "店舗名が100桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1224008",
            Message = "店舗IDの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1224012",
            Message = "店舗IDが32バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1225008",
            Message = "サービス種別の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1227003",
            Message = "楽天説明文が300文字を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1228008",
            Message = "優先バージョンの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1229008",
            Message = "結果画面スキップフラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1230008",
            Message = "セキュリティコード必須フラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1231008",
            Message = "編集ボタン非表示フラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1232008",
            Message = "削除ボタン非表示フラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1233008",
            Message = "追加ボタン非表示フラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1238008",
            Message = "セキュリティコード非表示フラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1238010",
            Message = "セキュリティコードが必須の場合、セキュリティコードを非表示に出来ません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1244012",
            Message = "会社名が32桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1244013",
            Message = "会社名の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1239008",
            Message = "口座名義任意名の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1239005",
            Message = "口座名義任意名が20桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1243008",
            Message = "振込依頼人メールアドレスの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1243005",
            Message = "振込依頼人メールアドレスが256桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1242013",
            Message = "振込依頼人氏名に利用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1242005",
            Message = "振込依頼人氏名が64桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1240011",
            Message = "取引有効日数が0～999の範囲ではありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1241013",
            Message = "取引事由に利用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1241005",
            Message = "取引事由が64桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1249001",
            Message = "SMS配信先携帯電話番号が指定されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1249006",
            Message = "SMS配信先携帯電話番号に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1249012",
            Message = "SMS配信先携帯電話番号が15バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1250005",
            Message = "本人認証質問文言1が50桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1251005",
            Message = "本人認証答え1が50桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1252001",
            Message = "本人認証質問1の指定が正しくありません。本人認証質問文言1/本人認証答え1のいずれかの省略はできません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1253005",
            Message = "本人認証質問文言2が50桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1254005",
            Message = "本人認証答え2が50桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1255001",
            Message = "本人認証質問2の指定が正しくありません。本人認証質問文言2/本人認証答え2のいずれかの省略はできません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1256005",
            Message = "本人認証リトライ最大回数が2桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1256006",
            Message = "本人認証リトライ最大回数に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1256009",
            Message = "本人認証に失敗しました。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1257008",
            Message = "決済後カード登録時会員IDの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1258008",
            Message = "会員ID非表示フラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1259008",
            Message = "会員名非表示フラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1260008",
            Message = "取引詳細初期表示フラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1261008",
            Message = "URL案内SMS送信フラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1262008",
            Message = "カード番号入力欄非表示フラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1263011",
            Message = "最大カード登録枚数の範囲が1～5の範囲ではありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1263017",
            Message = "登録済みのカードが最大カード登録枚数を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1264008",
            Message = "支払方法（複数指定）の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1264010",
            Message = "支払方法（複数指定）が一致しません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1266005",
            Message = "商品名が255桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1266013",
            Message = "商品名に利用できない文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1267008",
            Message = "確認画面スキップフラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1268005",
            Message = "完了時戻り先URLが256桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1268008",
            Message = "完了時戻り先URLの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1269005",
            Message = "キャンセル時戻り先URLが256桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1269008",
            Message = "キャンセル時戻り先URLの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1270009",
            Message = "ショップドメインが5件を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1271005",
            Message = "ショップドメイン1が253桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1271008",
            Message = "ショップドメイン1の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1272005",
            Message = "ショップドメイン2が253桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1272008",
            Message = "ショップドメイン2の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1273005",
            Message = "ショップドメイン3が253桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1273008",
            Message = "ショップドメイン3の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1274005",
            Message = "ショップドメイン4が253桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1274008",
            Message = "ショップドメイン4の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1275005",
            Message = "ショップドメイン5が253桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1275008",
            Message = "ショップドメイン5の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1278006",
            Message = "支払期限日数に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1278011",
            Message = "支払期限日数が有効な範囲ではありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1265011",
            Message = "支払期限が有効な範囲ではありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1279008",
            Message = "結果通知先メールアドレス必須フラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1019002",
            Message = "指定したオリジナルデザインテンプレートのデータが存在しません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1019010",
            Message = "指定したオリジナルデザインテンプレートに公開データが登録されていません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1280008",
            Message = "3Dセキュア画面新規タブフラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1281008",
            Message = "入金手続き完了通知メール送信無しフラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1282008",
            Message = "3Dセキュア認証用入力欄表示フラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1285001",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1285002",
            Message = "このURLはご利用いただけません。URLに間違いがないかご確認ください。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1285005",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1285008",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1285014",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1286008",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1286012",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1287008",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1287012",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1289001",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1289008",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1289012",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1290001",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1290008",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1291001",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1291008",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1292005",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1292008",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1293006",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1293007",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1293009",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1293010",
            Message = "リクエストされた金額が固定金額で設定された金額と一致しません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1293012",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1293018",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1294008",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1295008",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1296010",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1296011",
            Message = "URLの表示期間外です。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1297014",
            Message = "URL情報が無効のため、このURLはご利用いただけません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1298001",
            Message = "入力してください。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1298008",
            Message = "使用できない文字が含まれています。【使用可能文字：半角（スペース、英数字）※半角カタカナは不可、全角文字】"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1298012",
            Message = "入力できる文字数を超えています【入力可能文字数：半角100文字（半角カタカナは不可）、全角50文字】"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1299008",
            Message = "使用できない文字が含まれています。【使用可能文字：半角（スペース、英数字）※半角カタカナは不可、全角文字】"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1299012",
            Message = "入力できる文字数を超えています。【入力可能文字数：半角100文字（半角カタカナは不可）、全角50文字】"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1299014",
            Message = "入力してください。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1332001",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1332005",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1332008",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1332009",
            Message = "(管理画面からQR発行時のエラーコード)"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1332016",
            Message = "利用可能決済手段は利用可能な選択肢が存在しません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1301005",
            Message = "企業リンクURLが300桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1301008",
            Message = "企業リンクURLの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1302001",
            Message = "請求内容が指定されていません"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1302008",
            Message = "請求内容の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1302012",
            Message = "請求内容が30バイトを超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1303008",
            Message = "決済後強制カード登録フラグの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1304001",
            Message = "氏名（漢字）を入力してください。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1304005",
            Message = "氏名（漢字）が21桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1304008",
            Message = "氏名（漢字）の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1305005",
            Message = "氏名（カナ）が25桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1305008",
            Message = "氏名（カナ）の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1306001",
            Message = "郵便番号を入力してください。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1306008",
            Message = "郵便番号の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1307001",
            Message = "住所を入力してください。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1307005",
            Message = "住所が55桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1308005",
            Message = "会社名が30桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1308008",
            Message = "会社名の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1309005",
            Message = "部署名が30桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1309008",
            Message = "部署名の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1310001",
            Message = "携帯電話番号を入力してください。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1310008",
            Message = "携帯電話番号の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1311008",
            Message = "固定電話番号の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1312001",
            Message = "メールアドレスを入力してください。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1312005",
            Message = "メールアドレスが100桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1312008",
            Message = "メールアドレスの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1313005",
            Message = "メールアドレス2が100桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1313008",
            Message = "メールアドレス2の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1314008",
            Message = "性別の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1315008",
            Message = "誕生日の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1317005",
            Message = "支払回数が3桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1317006",
            Message = "支払回数に数字以外の文字が含まれています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1325009",
            Message = "明細詳細情報が15件を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1325010",
            Message = "明細詳細情報が不正です。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1326005",
            Message = "明細詳細情報_明細名が300桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1326008",
            Message = "明細詳細情報_明細名の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1327008",
            Message = "明細詳細情報_単価の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1328008",
            Message = "明細詳細情報_数量の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1329005",
            Message = "明細詳細情報_取引通番が4桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1329008",
            Message = "明細詳細情報_取引通番の書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1330005",
            Message = "明細詳細情報_ブランドが300桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1330008",
            Message = "明細詳細情報_ブランドの書式が正しくありません。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1331005",
            Message = "明細詳細情報_カテゴリが300桁を超えています。"
        },
        new()
        {
            Code = "EZ1",
            DetailCode = "EZ1331008",
            Message = "明細詳細情報_カテゴリの書式が正しくありません。"
        },

        #endregion

        #region EZ2

        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2001002",
            Message = "取引情報を確認できません。詳細はショップにご確認ください。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2001010",
            Message = "不正な操作が行われました。このURLはご利用いただけません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2001011",
            Message = "すでにお支払い手続きが完了しています。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2001012",
            Message = "お支払い手続きを実行できません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2001013",
            Message = "お支払い期限日を過ぎています。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2001014",
            Message = "すでに決済を開始しており、このURLはご利用いただけません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2001015",
            Message = "お支払いが完了しておりません。お支払い結果が反映されるまでにお時間をいただく場合がございます。支払いが確認できない場合は、ご利用されたショップへお問い合わせください"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2002002",
            Message = "クレジットカード決済取引情報が取得できません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2002010",
            Message = "クレジットカード決済取引情報が一致しません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2002014",
            Message = "すでにクレジットカード決済が進行しているため、本画面ではクレジットカード決済を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2003010",
            Message = "すでにコンビニ決済が進行しているため、本画面ではコンビニ決済を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2004002",
            Message = "カード登録情報を確認できません。詳細はショップにご確認ください。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2004010",
            Message = "不正な操作が行われました。このURLはご利用いただけません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2004011",
            Message = "すでにカード編集手続きが完了しています。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2004012",
            Message = "カード編集手続きを実行できません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2004013",
            Message = "カード編集期限日を過ぎています。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2005010",
            Message = "多通貨クレジットカード決済（DCC）取引情報が一致しません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2005014",
            Message = "すでに多通貨クレジットカード決済（DCC）が進行しているため、本画面では多通貨クレジットカード決済（DCC）を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2006010",
            Message = "すでにエポスかんたん決済が進行しているため、本画面ではエポスかんたん決済を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2007017",
            Message = "ショップ別レート照会に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2008010",
            Message = "すでにソフトバンクまとめて支払い決済が進行しているため、本画面ではソフトバンクまとめて支払い決済を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2009010",
            Message = "すでにau PAY（auかんたん決済）決済が進行しているため、本画面ではau PAY（auかんたん決済）を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2010010",
            Message = "すでにd払い決済が進行しているため、本画面ではd払い決済を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2011010",
            Message = "多通貨クレジットカード決済（MCP）取引情報が一致しません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2012010",
            Message = "すでにPay-easy決済が進行しているため、本画面ではPay-easy決済を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2013010",
            Message = "すでにLINE Pay決済が進行しているため、本画面ではLINE Pay決済を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2014010",
            Message = "すでにFamiPay決済が進行しているため、本画面ではFamiPay決済を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2015010",
            Message = "すでにメルペイ決済が進行しているため、本画面ではメルペイ決済を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2016010",
            Message = "すでに楽天ペイ決済が進行しているため、本画面では楽天ペイ決済を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2017010",
            Message = "すでにPayPay決済が進行しているため、本画面ではPayPay決済を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2018002",
            Message = "決済URL案内メールテンプレートが取得できません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2019002",
            Message = "カード編集URL案内メールテンプレートが取得できません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2020002",
            Message = "購入ありがとうメールテンプレートが取得できません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2021002",
            Message = "登録完了メールテンプレートが取得できません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2022010",
            Message = "すでに銀行振込（バーチャル口座）決済が進行しているため、本画面では銀行振込（バーチャル口座）決済を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2024010",
            Message = "ページが二重に表示された可能性があります。再表示するか、既に表示済みのページから操作を行ってください。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2025010",
            Message = "すでに楽天ペイ決済が進行しているため、本画面では楽天ペイ決済を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2069010",
            Message = "戻る・更新等の誤ったブラウザ操作を検知しました。初めからやり直すか、ほかのブラウザをご利用ください。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2228010",
            Message = "SMS配信が可能な設定ではありません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2229010",
            Message = "SMS配信が可能な設定ではありません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2230002",
            Message = "決済URL案内SMSテンプレートが取得できません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2231002",
            Message = "会員編集URL案内SMSテンプレートが取得できません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232001",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232002",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232003",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232004",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232005",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232006",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232007",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232008",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232009",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232010",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232011",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232012",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232013",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232014",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232015",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232016",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232017",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232018",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232019",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232020",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232021",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2232022",
            Message = "SMS配信に失敗しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2233016",
            Message = "利用可能な編集操作が存在しません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2233017",
            Message = "登録上限数を超えるカード情報の追加はできません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2263017",
            Message = "最大カード登録枚数を超えるカード情報の追加はできません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2023010",
            Message = "すでにau PAY（ネット支払い）決済が進行しているため、本画面ではau PAY（ネット支払い）決済を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2264010",
            Message = "すでにネット銀聯決済が進行しているため、本画面ではネット銀聯決済を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2265010",
            Message = "マイペイメント取引情報が一致しません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2266010",
            Message = "すでにAEON Pay決済が進行しているため、本画面ではAEON Pay決済を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2267010",
            Message = "すでにアトカラ決済が進行しているため、本画面ではアトカラ決済を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2268010",
            Message = "システムエラーが発生しました。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2269011",
            Message = "総合的判断によりアトカラをご利用いただけない結果となりました。別のお支払い方法をご検討ください。本決済URLは無効になります。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2278002",
            Message = "取引情報を確認できません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2278010",
            Message = "決済手続きを中止できない取引状態です。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2279002",
            Message = "カード登録情報を確認できません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2279010",
            Message = "カード編集手続きを中止できない取引状態です。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2026010",
            Message = "すでに銀行振込（バーチャル口座 あおぞら）決済が進行しているため、本画面では銀行振込（バーチャル口座 あおぞら）決済を行えません。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2300001",
            Message = "決済画面への遷移に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2400001",
            Message = "メンテナンス中のためコンビニ決済（ファミリーマート）は一時的に利用できません。しばらくお待ちのうえ再度お試しください。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2400002",
            Message = "メンテナンス中のためコンビニ決済（セブン-イレブン）は一時的に利用できません。しばらくお待ちのうえ再度お試しください。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2400003",
            Message = "メンテナンス中のためコンビニ決済は一時的に利用できません。しばらくお待ちのうえ再度お試しください。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2400004",
            Message = "メンテナンス中のためコンビニ決済は一時的に利用できません。しばらくお待ちのうえ再度お試しください。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2400005",
            Message = "メンテナンス中のためPay-easy（ペイジー）は一時的に利用できません。しばらくお待ちのうえ再度お試しください。"
        },
        new()
        {
            Code = "EZ2",
            DetailCode = "EZ2400009",
            Message = "メンテナンス中のため銀行振込（バーチャル口座 あおぞら）は一時的に利用できません。しばらくお待ちのうえ再度お試しください。"
        },

        #endregion

        #region EZ3

        new()
        {
            Code = "EZ3",
            DetailCode = "EZ3013001",
            Message = "LINE Pay決済がキャンセルされました。"
        },
        new()
        {
            Code = "EZ3",
            DetailCode = "EZ3014001",
            Message = "FamiPay決済がキャンセルされました。"
        },
        new()
        {
            Code = "EZ3",
            DetailCode = "EZ3001001",
            Message = "お取引がキャンセルされました。"
        },
        new()
        {
            Code = "EZ3",
            DetailCode = "EZ3267001",
            Message = "アトカラでの購入手続きが中断されました。決済をリトライする場合は、「再入力」ボタンを押下してください。"
        },

        #endregion

        #region EZ4

        new()
        {
            Code = "EZ4",
            DetailCode = "EZ4135014",
            Message = ""
        },
        new()
        {
            Code = "EZ4",
            DetailCode = "EZ4136014",
            Message = ""
        },
        new()
        {
            Code = "EZ4",
            DetailCode = "EZ4137014",
            Message = ""
        },

        #endregion

        #region EZ9

        new()
        {
            Code = "EZ9",
            DetailCode = "EZ9001999",
            Message = "通信エラーが発生致しました。"
        },
        new()
        {
            Code = "EZ9",
            DetailCode = "EZ9099999",
            Message = "予期せぬエラーが発生しました。"
        },

        #endregion

        #region M

        new()
        {
            Code = "M01",
            DetailCode = "M01004014",
            Message = "指定された取引は既に決済を依頼しています。"
        },
        new()
        {
            Code = "M01",
            DetailCode = "M01131010",
            Message = "入力されたカード番号は多通貨決済に対応していません。"
        },
        new()
        {
            Code = "M01",
            DetailCode = "M01311007",
            Message = "支払期限を過ぎています。詳細はショップにて問い合わせください。"
        },
        new()
        {
            Code = "M01",
            DetailCode = "M01558010",
            Message = "指定された商品カテゴリIDは定義されていません。詳細はショップにお問い合わせください。"
        },
        new()
        {
            Code = "M01",
            DetailCode = "M01010013",
            Message = "氏名に利用できない漢字・文字が含まれています。ひらがな、カナ、常用漢字、半角文字をご入力ください。"
        },
        new()
        {
            Code = "M",
            DetailCode = "その他",
            Message = "決済失敗しました。"
        },

        #endregion

        #region C

        new()
        {
            Code = "C01",
            DetailCode = "42C010000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C03",
            DetailCode = "42C030000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C12",
            DetailCode = "42C120000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C13",
            DetailCode = "42C130000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C14",
            DetailCode = "42C140000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C15",
            DetailCode = "42C150000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C50",
            DetailCode = "42C500000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C51",
            DetailCode = "42C510000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C53",
            DetailCode = "42C530000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C54",
            DetailCode = "42C540000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C55",
            DetailCode = "42C550000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C56",
            DetailCode = "42C560000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C57",
            DetailCode = "42C570000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C58",
            DetailCode = "42C580000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C60",
            DetailCode = "42C600000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C70",
            DetailCode = "42C700000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C71",
            DetailCode = "42C710000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C72",
            DetailCode = "42C720000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C73",
            DetailCode = "42C730000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C74",
            DetailCode = "42C740000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C75",
            DetailCode = "42C750000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C76",
            DetailCode = "42C760000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C77",
            DetailCode = "42C770000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C78",
            DetailCode = "42C780000",
            Message = "決済処理に失敗しました。申し訳ございませんが、しばらくした後にもう一度やり直してください。"
        },
        new()
        {
            Code = "C",
            DetailCode = "その他",
            Message = "カード会社との間で通信エラーが発生しました。"
        },

        #endregion

        #region W

        new()
        {
            Code = "W",
            DetailCode = "全て",
            Message = "決済失敗しました。"
        },

        #endregion

        #region P

        new()
        {
            Code = "P",
            DetailCode = "全て",
            Message = "決済失敗しました。"
        },

        #endregion

        #region DC1

        new()
        {
            Code = "DC1",
            DetailCode = "全て",
            Message = "決済失敗しました。"
        },

        #endregion

        #region SB1

        new()
        {
            Code = "SB1",
            DetailCode = "全て",
            Message = "決済失敗しました。"
        },

        #endregion

        #region FP1

        new()
        {
            Code = "FP1",
            DetailCode = "全て",
            Message = "決済失敗しました。"
        },

        #endregion

        #region AU1

        new()
        {
            Code = "AU1",
            DetailCode = "全て",
            Message = "決済失敗しました。"
        },

        #endregion

        #region AMP

        new()
        {
            Code = "AMP",
            DetailCode = "全て",
            Message = "決済失敗しました。"
        },

        #endregion

        #region LP

        new()
        {
            Code = "LP1",
            DetailCode = "LP1000001",
            Message = "LINE Pay決済にてエラーが発生しました。詳細はショップにて問い合わせください。"
        },
        new()
        {
            Code = "LP1",
            DetailCode = "LP1000002",
            Message = "LINE Pay決済にてエラーが発生しました。詳細はショップにて問い合わせください。"
        },
        new()
        {
            Code = "LP1",
            DetailCode = "LP1000003",
            Message = "LINE Pay決済にてエラーが発生しました。詳細はショップにて問い合わせください。"
        },
        new()
        {
            Code = "LP1",
            DetailCode = "LP1001163",
            Message = "LINE Pay決済にてエラーが発生しました。詳細はショップにて問い合わせください。"
        },
        new()
        {
            Code = "LP1",
            DetailCode = "LP1001177",
            Message = "LINE Pay決済にてエラーが発生しました。詳細はショップにて問い合わせください。"
        },
        new()
        {
            Code = "LP1",
            DetailCode = "LP1001180",
            Message = "LINE Pay決済にて支払の有効期限が経過しました。"
        },
        new()
        {
            Code = "LP1",
            DetailCode = "LP1009001",
            Message = "LINE Pay決済にてエラーが発生しました。詳細はショップにて問い合わせください。"
        },
        new()
        {
            Code = "LP1",
            DetailCode = "LP1009002",
            Message = "LINE Pay決済にてエラーが発生しました。詳細はショップにて問い合わせください。"
        },
        new()
        {
            Code = "LP1",
            DetailCode = "LP1009003",
            Message = "LINE Pay決済にてエラーが発生しました。詳細はショップにて問い合わせください。"
        },
        new()
        {
            Code = "LP2",
            DetailCode = "LP2009900",
            Message = "LINE Pay決済にてシステムエラーが発生しました。詳細はショップにて問い合わせください。"
        },
        new()
        {
            Code = "LP2",
            DetailCode = "LP2009999",
            Message = "LINE Pay決済にてシステムエラーが発生しました。詳細はショップにて問い合わせください。"
        },

        #endregion

        #region RP

        new()
        {
            Code = "RP1",
            DetailCode = "RP1000001",
            Message = "楽天ペイでシステムエラーが発生しました。しばらくお待ちのうえ再度お試しください。"
        },
        new()
        {
            Code = "RP1",
            DetailCode = "RP1000002",
            Message = "楽天ペイでタイムアウトが発生しました。再度お試しください。"
        },
        new()
        {
            Code = "RP1",
            DetailCode = "RP1000003",
            Message = "楽天ペイで処理中です。しばらくお待ちのうえ再度お試しください。"
        },
        new()
        {
            Code = "RP1",
            DetailCode = "RP1000004",
            Message = "利用カードを変更して再度お試しください。"
        },
        new()
        {
            Code = "RP1",
            DetailCode = "RP1000005",
            Message = "楽天ポイントの残高を確認して再度お試しください。"
        },
        new()
        {
            Code = "RP1",
            DetailCode = "RP1000006",
            Message = "楽天ペイでシステムエラーが発生しました。詳細はショップに問い合わせください。"
        },
        new()
        {
            Code = "RP1",
            DetailCode = "RP1000007",
            Message = "楽天ペイでシステムエラーが発生しました。しばらくお待ちのうえ再度お試しください。"
        },
        new()
        {
            Code = "RP1",
            DetailCode = "RP1000008",
            Message = "楽天ペイがアクセス過多により繋がりにくくなっています。しばらくお待ちのうえ再度お試しください。"
        },
        new()
        {
            Code = "RP1",
            DetailCode = "RP1000009",
            Message = "楽天ポイントが一時的に利用できません。利用ポイントを変更して再度お試しください。"
        },
        new()
        {
            Code = "RP1",
            DetailCode = "RP1000010",
            Message = "利用カードを変更して再度お試しください。"
        },
        new()
        {
            Code = "RP1",
            DetailCode = "RP1000011",
            Message = "楽天ペイはメンテナンス中のため一時的にご利用できません。しばらくお待ちのうえ再度お試しください。"
        },
        new()
        {
            Code = "RP1",
            DetailCode = "RP1000012",
            Message = "楽天ペイはメンテナンス中のため、一時的にご利用できません。"
        },
        new()
        {
            Code = "RP1",
            DetailCode = "RP1000013",
            Message = "楽天ペイでシステムエラーが発生しました。詳細はショップに問い合わせください。"
        },
        new()
        {
            Code = "RP1",
            DetailCode = "RP1000999",
            Message = "楽天ペイでシステムエラーが発生しました。"
        },

        #endregion

        #region AE

        new()
        {
            Code = "AE1",
            DetailCode = "AE1000001",
            Message = "AEON Payにてエラーが発生しました。"
        },
        new()
        {
            Code = "AE1",
            DetailCode = "AE1000002",
            Message = "AEON Payにてワンタイムコードが誤っているため、決済が失敗しました。"
        },
        new()
        {
            Code = "AE1",
            DetailCode = "AE1000003",
            Message = "AEON Payにてチャージ額不足のため、決済が失敗しました。"
        },
        new()
        {
            Code = "AE1",
            DetailCode = "AE1000004",
            Message = "AEON Payにて利用限度額設定の上限を超えているため、処理できませんでした。"
        },
        new()
        {
            Code = "AE1",
            DetailCode = "AE1000005",
            Message = "AEON Payにてユーザーのアカウント状態が異常であるため、決済が失敗しました。"
        },
        new()
        {
            Code = "AE1",
            DetailCode = "AE1000006",
            Message = "AEON Payにてポイント数不足のため、決済が失敗しました。"
        },
        new()
        {
            Code = "AE1",
            DetailCode = "AE1000007",
            Message = "AEON Payにて決済に失敗しました。"
        },
        new()
        {
            Code = "AE1",
            DetailCode = "AE1000009",
            Message = "AEON Payにてシステムエラーが発生しました。"
        },
        new()
        {
            Code = "AE1",
            DetailCode = "AE1001001",
            Message = "AEON Payにてお客様の操作により取引がキャンセルされました。"
        },
        new()
        {
            Code = "AE1",
            DetailCode = "AE1001002",
            Message = "AEON Payにてお客様の操作を起因としてエラーが発生しました。"
        },

        #endregion

        #region AT1

        new()
        {
            Code = "AT1",
            DetailCode = "AT1CT0009",
            Message = "購入者の氏名（漢字）の値が不正です。"
        },
        new()
        {
            Code = "AT1",
            DetailCode = "AT1CT0011",
            Message = "購入者の氏名（カナ）の値が不正です。"
        },
        new()
        {
            Code = "AT1",
            DetailCode = "AT1CT0014",
            Message = "購入者の郵便番号の値が不正です。"
        },
        new()
        {
            Code = "AT1",
            DetailCode = "AT1CT0017",
            Message = "購入者の住所の値が不正です。"
        },
        new()
        {
            Code = "AT1",
            DetailCode = "AT1CT0019",
            Message = "購入者の会社名の値が不正です。"
        },
        new()
        {
            Code = "AT1",
            DetailCode = "AT1CT0021",
            Message = "購入者の部署名の値が不正です。"
        },
        new()
        {
            Code = "AT1",
            DetailCode = "AT1CT0024",
            Message = "購入者の電話番号 1 の値が不正です。"
        },
        new()
        {
            Code = "AT1",
            DetailCode = "AT1CT0026",
            Message = "購入者の電話番号 2 の値が不正です。"
        },
        new()
        {
            Code = "AT1",
            DetailCode = "AT1CT0028",
            Message = "購入者のメールアドレス 1 の値が不正です。"
        },
        new()
        {
            Code = "AT1",
            DetailCode = "AT1CT0030",
            Message = "購入者のメールアドレス 2 の値が不正です。"
        },
        new()
        {
            Code = "AT1",
            DetailCode = "AT1CT0047",
            Message = "購入者の郵便番号と住所が一致しません。"
        },
        new()
        {
            Code = "AT1",
            DetailCode = "AT1CT0057",
            Message = "購入者の丁番地または建造物名／号室等を入力してください。"
        },
        new()
        {
            Code = "AT1",
            DetailCode = "AT1CT0058",
            Message = "購入者の電話番号が携帯電話番号ではありません。"
        },
        new()
        {
            Code = "AT1",
            DetailCode = "AT1CT0060",
            Message = "性別の値が不正です。"
        },
        new()
        {
            Code = "AT1",
            DetailCode = "AT1CT0062",
            Message = "誕生日の値が不正です。"
        },

        #endregion

        #region Other

        new()
        {
            Code = "その他",
            DetailCode = "ERR_EXEC",
            Message = "決済処理中にエラーが発生しました。"
        },
        new()
        {
            Code = "その他",
            DetailCode = "ERR_UNEXPECTED",
            Message = "予期せぬエラーが発生しました。"
        },
        new()
        {
            Code = "E00",
            DetailCode = "E00000001",
            Message = "－"
        },
        new()
        {
            Code = "E00",
            DetailCode = "E00000002",
            Message = "－"
        },
        new()
        {
            Code = "E00",
            DetailCode = "E00000003",
            Message = "－"
        },
        new()
        {
            Code = "E00",
            DetailCode = "E00000010",
            Message = "－"
        },

        #endregion
    ];
}
