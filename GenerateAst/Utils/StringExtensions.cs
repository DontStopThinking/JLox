namespace GenerateAst.Utils;

public static class StringExtensions
{
    public static string PadBoth(this string str, int length, char paddingChar = ' ')
    {
        int spaces = length - str.Length;
        int padLeft = (spaces / 2) + str.Length;
        return str.PadLeft(padLeft, paddingChar).PadRight(length, paddingChar);
    }
}
