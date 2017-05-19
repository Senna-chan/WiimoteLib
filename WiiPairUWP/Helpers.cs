using System;

namespace WiiPairUWP
{
    public class Helpers
    {
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
        public static char[] GetCharArray(byte[] bytes)
        {
            char[] chars = new char[bytes.Length];
            var count = 0;
            foreach (var b in bytes)
            {
                chars[count] = (char) b;
                count++;
            }
            return chars;
        }
    }
}
