using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EmoTestApp
{
    internal class Cryptographer
    {
        static byte[] key_ = new byte[] { 0x5F, 0x3F, 0xA9, 0x5A, 0x0E, 0xAE, 0x35, 0x12 };
        static byte[] iv_ = new byte[] { 0x7A, 0xFE, 0x1E, 0x75, 0x8D, 0x05, 0x65, 0x67 };

        internal static byte[] Crypt(string value)
        {
            var des = new DESCryptoServiceProvider();
            var ct = des.CreateDecryptor(key_, iv_);
            //var stream = new MemoryStream();
            FileStream stream = new FileStream("D\\Pocket.data",
                FileMode.Open, FileAccess.Read);
            var cs = new CryptoStream(stream, ct, CryptoStreamMode.Read);

            cs.Write(Encoding.ASCII.GetBytes(value), 0, value.Length);
            cs.Close();
            return Encoding.Unicode.GetBytes(stream.ToString());

        }

        internal static string Decrypt(byte[] value)
        {
            var des = new DESCryptoServiceProvider();
            var ct = des.CreateDecryptor(key_, iv_);
            var stream = new MemoryStream();
            var cs = new CryptoStream(stream, ct, CryptoStreamMode.Write);

            cs.Write(value, 0, value.Length);
            cs.Close();

            return Encoding.Unicode.GetString(stream.ToArray());
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Encrypt a string.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="originalString">The original string.</param></span>
        /// <span class="code-SummaryComment"><returns>The encrypted string.</returns></span>
        /// <span class="code-SummaryComment"><exception cref="ArgumentNullException">This exception will be </span>
        /// thrown when the original string is null or empty.<span class="code-SummaryComment"></exception></span>
        internal static string Encrypt(string originalString)
        {
            if (String.IsNullOrEmpty(originalString))
            {
                throw new ArgumentNullException
                    ("The string which needs to be encrypted can not be null.");
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                cryptoProvider.CreateEncryptor(key_, iv_), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(originalString);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Decrypt a crypted string.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="cryptedString">The crypted string.</param></span>
        /// <span class="code-SummaryComment"><returns>The decrypted string.</returns></span>
        /// <span class="code-SummaryComment"><exception cref="ArgumentNullException">This exception will be thrown </span>
        /// when the crypted string is null or empty.<span class="code-SummaryComment"></exception></span>
        internal static string Decrypt(string cryptedString)
        {
            if (String.IsNullOrEmpty(cryptedString))
            {
                throw new ArgumentNullException
                    ("The string which needs to be decrypted can not be null.");
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream
                (Convert.FromBase64String(cryptedString));
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                cryptoProvider.CreateDecryptor(key_, iv_), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream);
            return reader.ReadToEnd();
        }
    }
}

