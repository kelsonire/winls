using System;
using System.Collections.Generic;
using System.Text;

namespace winls
{
    public static class CharExtension
    {
        public static bool IsHalfWidth(this char c)
        {
            return IsAsciiChar(c) || IsHalfWidthKana(c);
        }

        private static bool IsAsciiChar(char c)
		{
            return ' ' <= c && c <= '~';
		}

        private static bool IsHalfWidthKana(char c)
		{
            return 'ｱ' <= c && c <= 'ﾝ';
		}

        public static bool IsFullWidth(this char c)
		{
            return !IsHalfWidth(c);
		}
    }
}
