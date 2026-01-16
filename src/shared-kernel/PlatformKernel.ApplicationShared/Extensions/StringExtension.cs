namespace PlatformKernel.ApplicationShared.Extensions;

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

    private struct BraceContext
    {
        public int Begin;
        public int Left;
        public int Right;
        public int NameEnd;
    }

    private struct Formatter
    {
        private List<string>? _list;

        private List<object>? _args;
        private Dictionary<string, string>? _indexes;

        private string? _cache;

        public string ToFormat(
            string str,
            IDictionary<string, object?> args,
            bool useOption
        )
        {
            Clear();

            var argKeys = args.Keys;
            var length = str.Length;
            var braceContext = new BraceContext
            {
                Begin = 0,
                Left = -1,
                Right = -1,
                NameEnd = -1
            };

            for (var i = 0; i < length; ++i)
            {
                var c = str[i];

                if (c == '{')
                {
                    HandleLeftBrace(ref braceContext.Left, i);
                }
                else if (c == '}')
                {
                    HandleRightBrace(
                        str,
                        args,
                        useOption,
                        ref braceContext,
                        i
                    );
                }
                else if ((c == ',' || c == ':') && useOption && braceContext is { Left: >= 0, NameEnd: < 0 })
                {
                    braceContext.NameEnd = i;
                }
            }

            AppendRemainingString(str, braceContext.Begin, length);
            return ExecuteFormat(str, argKeys);
        }

        private static void HandleLeftBrace(
            ref int left,
            int i
        )
        {
            if (left >= 0 && left == i - 1)
            {
                left = -1;
            }
            else
            {
                left = i;
            }
        }

        private void HandleRightBrace(
            string str,
            IDictionary<string, object?> args,
            bool useOption,
            ref BraceContext context,
            int i
        )
        {
            if (context.Right >= 0 && context.Right == i - 1)
            {
                context.Right = -1;
                return;
            }

            if (context.Left >= 0)
            {
                context.Right = i;
                ProcessFormatArgument(str, args, useOption, ref context);
            }
            else
            {
                context.Right = i;
            }
        }

        private void ProcessFormatArgument(
            string str,
            IDictionary<string, object?> args,
            bool useOption,
            ref BraceContext context
        )
        {
            Append(str.Substring(context.Begin, context.Left - context.Begin + 1));

            var nameBegin = context.Left + 1;
            if (context.NameEnd < 0)
            {
                context.NameEnd = context.Right;
            }

            var key = str.Substring(nameBegin, context.NameEnd - nameBegin);
            var index = ToIndex(key) ?? AddArg(key, args.TryGetValue(key, out var value) ? value : null);

            Append(index);

            if (useOption && context.NameEnd < context.Right)
            {
                var option = str.Substring(context.NameEnd, context.Right - context.NameEnd);
                Append(option);
            }

            Append("}");
            context.Begin = context.Right + 1;
            context.Left = context.Right = context.NameEnd = -1;
        }

        private void AppendRemainingString(
            string str,
            int begin,
            int length
        )
        {
            if (begin > 0 && begin < length)
            {
                Append(str.Substring(begin, length - begin));
            }
        }

        private string ExecuteFormat(
            string str,
            ICollection<string> argKeys
        )
        {
            if (_list != null)
            {
                var list = _list.Where(a => !argKeys.Contains(a));
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
            _indexes ??= [];
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
        if (string.IsNullOrEmpty(str))
        {
            return string.Empty;
        }

        var formatter = new Formatter();
        return formatter.ToFormat(str, args, useOption);
    }

    #endregion
}
