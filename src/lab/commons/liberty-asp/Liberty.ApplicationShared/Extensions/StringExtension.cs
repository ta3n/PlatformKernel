namespace Liberty.ApplicationShared.Extensions;

public class FormatDictionary : Dictionary<string, object?>
{
    public FormatDictionary()
    {
    }

    public FormatDictionary(
        IEqualityComparer<string> comparer
    )
        : base(comparer)
    {
    }

    public FormatDictionary(
        IDictionary<string, object?> dictionary
    )
        : base(dictionary)
    {
    }

    public FormatDictionary(
        int capacity
    )
        : base(capacity)
    {
    }

    public FormatDictionary(
        IDictionary<string, object?> dictionary,
        IEqualityComparer<string> comparer
    )
        : base(dictionary, comparer)
    {
    }

    public FormatDictionary(
        int capacity,
        IEqualityComparer<string> comparer
    )
        : base(capacity, comparer)
    {
    }
}

public static class StringExtension
{
    #region 内部クラス

    private struct Formatter
    {
        private List<string>? _list;

        private List<object>? _args;
        private Dictionary<string, string>? _indexes;

        private string? _cache;

        public string Format(
            string str,
            IDictionary<string, object?> args,
            bool useOption
        )
        {
            Clear();

            // ディクショナリで定められたキー
            var argKeys = args.Keys;

            var length = str.Length;

            var begin = 0;
            var left = -1;
            var right = -1;
            var nameEnd = -1;

            // {key[,alignment][:formatString]}形式のパース
            for (var i = 0; i < length; ++i)
            {
                var c = str[i];

                switch (c)
                {
                    case '{':
                        // 左波括弧の連続はエスケープなのでそのまま通過
                        if (left >= 0 && left == i - 1)
                        {
                            left = -1;
                            continue;
                        }

                        left = i;
                        nameEnd = -1;
                        break;
                    case '}':
                        // 右波括弧の連続はエスケープなのでそのまま通過
                        if (right >= 0 && right == i - 1)
                        {
                            right = -1;
                            continue;
                        }

                        // フォーマット引数の処理
                        if (left >= 0)
                        {
                            right = i;

                            // 前方部分文字列の反映
                            Append(str.Substring(begin, left - begin + 1));

                            // 引数名の取得
                            var nameBegin = left + 1;
                            if (nameEnd < 0)
                            {
                                nameEnd = i;
                            }

                            var key = str.Substring(nameBegin, nameEnd - nameBegin);

                            // 引数名をインデックスに変換して反映
                            var index = ToIndex(key);
                            if (index is null)
                            {
                                try
                                {
                                    index = AddArg(key, args[key]);
                                }
                                catch (KeyNotFoundException)
                                {
                                    // キーがない場合
                                    index = AddArg(key, null);
                                }
                            }

                            Append(index);

                            // オプションがあれば反映
                            if (useOption && nameEnd < right)
                            {
                                var option = str.Substring(nameEnd, right - nameEnd);
                                Append(option);
                            }

                            // 閉じ括弧の反映
                            Append("}");

                            begin = i + 1;
                            left = right = nameEnd = -1;
                            continue;
                        }

                        right = i;
                        break;
                    case ',':
                    case ':':
                        // 引数名の処理中であれば引数名の終了位置として記憶
                        if (useOption && left >= 0 && nameEnd < 0)
                        {
                            nameEnd = i;
                        }

                        break;
                }
            }

            // 末尾の追加
            if (begin > 0 && begin < length)
            {
                Append(str.Substring(begin, length - begin));
            }

            // フォーマット実行
            if (_list != null)
            {
                // ディクショナリで与えられていないキーに対しては無効
                var list = _list
                    .Where(a => !argKeys.Contains(a));

                var format = string.Concat(list.ToArray());
                _cache = format.Format(GetArgs());
            }
            else
            {
                _cache = str.Format();
            }

            return _cache;
        }

        public override string? ToString()
        {
            return _cache;
        }

        private void Append(
            string str
        )
        {
            if (str.IsNullOrEmpty())
            {
                return;
            }

            // 必要になって初めてアロケート
            _list ??= [];

            _list.Add(str);
        }

        private string? ToIndex(
            string key
        )
        {
            if (key.IsNullOrEmpty())
            {
                return null;
            }

            if (_indexes is { Count: <= 0 })
            {
                return null;
            }

            return _indexes?.GetValueOrDefault(key);
        }

        private string AddArg(
            string key,
            object? value
        )
        {
            // 必要になって初めてアロケート
            _indexes ??= new Dictionary<string, string>();
            _args ??= [];

            _args.Add(value ?? string.Empty);

            var index = (_args.Count - 1).ToString();
            _indexes[key] = index;

            return index;
        }

        private object[]? GetArgs()
        {
            return _args?.ToArray();
        }

        private void Clear()
        {
            _list?.Clear();
            _args?.Clear();
            _indexes?.Clear();

            _cache = "";
        }
    }

    #endregion

    #region 叙述

    public static bool IsNullOrEmpty(
        this string str
    )
    {
        return string.IsNullOrEmpty(str);
    }

    #endregion

    #region 書式指定

    public static string Format(
        this string str,
        params object[]? args
    )
    {
        return args is { Length: > 0 }
            ? string.Format(str, args)
            : str;
    }

    public static string FormatByName(
        this string str,
        IDictionary<string, object?> args,
        bool useOption = true
    )
    {
        var formatter = new Formatter();
        return formatter.Format(str, args, useOption);
    }

    #endregion
}
