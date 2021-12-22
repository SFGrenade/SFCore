using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Modding;

namespace SFCore.Utils
{
    /// <summary>
    ///     Misc utils to use.
    /// </summary>
    public static class Util
    {
        /// <summary>
        ///     Sets a nonpublic member of an Object to a specified value.
        /// </summary>
        /// <param name="o">The object that has the nonpublic member</param>
        /// <param name="fieldname">The name of the nonpublic member</param>
        /// <param name="value">The value that the nonpublic member should have</param>
        public static void SetAttr<TSelf, TVal>(this TSelf o, string fieldname, TVal value)
        {
            ReflectionHelper.SetField(o, fieldname, value);
        }
        /// <summary>
        ///     Gets the value of a nonpublic member of an Object.
        /// </summary>
        /// <param name="o">The object that has the nonpublic member</param>
        /// <param name="fieldname">The name of the nonpublic member</param>
        /// <returns>The value of the nonpublic member.</returns>
        public static TVal GetAttr<TSelf, TVal>(this TSelf o, string fieldname)
        {
            return ReflectionHelper.GetField<TSelf, TVal>(o, fieldname);
        }

        /// <summary>
        ///     Gets the version of an Assembly.
        /// </summary>
        /// <param name="asm">The Assembly of which to get the version</param>
        /// <returns>"{Assembly.Version}-{SHA1(assembly binary)}"</returns>
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