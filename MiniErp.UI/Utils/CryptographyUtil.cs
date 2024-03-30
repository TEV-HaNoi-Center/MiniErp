using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.UI.Utils
{
    public static class CryptographyUtil
    {
        public static string SHA256Hash(string input)
        {
            var hash = SHA256.Create();
            var hashValue = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Encoding.ASCII.GetString(hashValue);
        }
    }
}
