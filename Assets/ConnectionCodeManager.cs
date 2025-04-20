using System;
using System.Linq;
using System.Numerics;
using UnityEngine;

public static class ConnectionCodeManager
{
    private const string base62 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    //Encode IP + Port into short code
    public static string Encode(string ip, ushort port)
    {
        byte[] data = new byte[6];
        string[] ipParts = ip.Split('.');

        for (int i = 0; i < 4; i++)
            data[i] = byte.Parse(ipParts[i]);

        data[4] = (byte)(port >> 8);
        data[5] = (byte)(port & 0xFF);

        return ToBase62(data);
    }

    //Decode code into IP + Port
    public static void Decode(string code, out string ip, out ushort port)
    {
        byte[] data = FromBase62(code);

        ip = $"{data[0]}.{data[1]}.{data[2]}.{data[3]}";
        port = (ushort)((data[4] << 8) | data[5]);
    }

    private static string ToBase62(byte[] data)
    {
        BigInteger value = new BigInteger(data.Reverse().Concat(new byte[] { 0 }).ToArray());
        string result = "";

        while (value > 0)
        {
            value = BigInteger.DivRem(value, 62, out BigInteger rem);
            result = base62[(int)rem] + result;
        }

        return result;
    }

    private static byte[] FromBase62(string str)
    {
        BigInteger value = 0;

        foreach (char c in str)
        {
            value *= 62;
            value += base62.IndexOf(c);
        }

        byte[] fullBytes = value.ToByteArray();
        if (fullBytes[fullBytes.Length - 1] == 0)
            fullBytes = fullBytes.Take(fullBytes.Length - 1).ToArray();

        return fullBytes.Reverse().ToArray();
    }
}
