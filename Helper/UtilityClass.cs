using Pitstop.Models.Common;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Pitstop.Helper
{
    public class UtilityClass
    {

        public AppKeysOption _appKeys { get; set; }
        public ProfilesOption _profiles { get; set; }
        public UtilityClass(IOptions<AppKeysOption> appKeys, IOptions<ProfilesOption> profiles)
        {
            _appKeys = appKeys.Value;
            _profiles = profiles.Value;
        }

        public string GetDefaultLoginUrl() => _appKeys.DefaultLoginUrl;

        public ProfilesOption GetProfiles()
        {
            return _profiles;
        }

        public string EncryptData(string clearText)
        {
            //string result = EncryptAES(clearText);
            //result = Encrypt_Data_Internal(result);
            string result = Encrypt_Data_Internal(clearText);
            return result;
        }

        public string DecryptData(string cipherText)
        {
            string result = Decrypt_Data_Internal(cipherText);
            //result = DecryptAES(result);
            return result;
        }
        private string EncryptAES(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";

            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }

                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }

            return clearText;
        }

        private string DecryptAES(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";

            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }

                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return cipherText;
        }

        public string GetIPAddress(HttpContext context)
        {
            string ipAddress = context.Request.Query["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.Query["REMOTE_ADDR"];
        }


        private string Decrypt_Data_Internal(string Input)
        {
            Input = Input.Trim();
            if (Input != "")
            {
                byte[] bytes = Convert.FromBase64String(HttpUtility.UrlDecode(Input));
                return Encoding.UTF8.GetString(bytes);
            }

            return "";
        }

        private string Encrypt_Data_Internal(string Input)
        {
            Input = Input.Trim();
            if (Input.Trim() != "")
            {
                return HttpUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(Input)));
            }

            return "";
        }

    }
}
