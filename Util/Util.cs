using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Modding;
using UnityEngine;

namespace SFCore.Utils
{
    public static class Util
    {
        public static void SetAttr<TSelf, TVal>(this TSelf o, string fieldname, TVal value)
        {
            ReflectionHelper.SetAttr<TSelf, TVal>(o, fieldname, value);
        }
        public static TVal GetAttr<TSelf, TVal>(this TSelf o, string fieldname)
        {
            return ReflectionHelper.GetAttr<TSelf, TVal>(o, fieldname);
        }

        public static string GetVersion(Assembly asm)
        {
            var ver = asm.GetName().Version.ToString();
            var sha1 = SHA1.Create();
            var stream = File.OpenRead(asm.Location);
            var hashBytes = sha1.ComputeHash(stream);
            var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            stream.Close();
            sha1.Clear();
            return $"{ver}-{hash.Substring(0, 6)}";
        }
    }
}