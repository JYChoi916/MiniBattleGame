using System;

public static class Utility
{
    static Random dice = new Random();
    public static int GetFullCharCount(string str)
    {
        int count = 0;
        for (int i = 0; i < str.Length; ++i)
        {
            char c = str[i];
            int byteCount = System.Text.Encoding.UTF8.GetByteCount(new char[] { c });
            if (byteCount > 1)
                count++;
        }

        return count;
    }

    public static int GetRandom(int min, int max)
    {
        return dice.Next(min, max);
    }

    public static string Full2Half(string sFull)
    {
        char[] ch = sFull.ToCharArray(0, sFull.Length);
        for (int i = 0; i < sFull.Length; ++i)
        {
            if (ch[i] > 0xff00 && ch[i] <= 0xff5e)
                ch[i] -= (char)0xfee0;
            else if (ch[i] == 0x3000)
                ch[i] = (char)0x20;
        }
        return (new string(ch));
    }

    public static string Half2Full(string sHalf)
    {
        char[] ch = sHalf.ToCharArray(0, sHalf.Length);
        for (int i = 0; i < sHalf.Length; ++i)
        {
            if (ch[i] > 0x21 && ch[i] <= 0x7e)
                ch[i] += (char)0xfee0;
            else if (ch[i] == 0x20)
                ch[i] = (char)0x3000;
        }
        return (new string(ch));
    }

    public static void MakeSameLengthStrings(List<string> strings)
    {
        int longestLength = strings.Max(s => s.Length);
        string longestString = strings.FirstOrDefault(s => s.Length == longestLength);

        int fullCharCount = GetFullCharCount(longestString);
        for (int i = 0; i < strings.Count; ++i)
        {
            var info = strings[i];
            if (info.Length < longestLength)
            {
                int diff = longestLength - info.Length;
                int fc = GetFullCharCount(info);
                int fcDiff = fullCharCount - fc;
                for (int j = 0; j <  + diff + (fcDiff); ++j)
                {
                    info += " ";
                }
                strings[i] = info;
            }
        }
    }
}
