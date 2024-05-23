using Serilog;
using System.Security.Cryptography;

namespace DocumentManagement.Server.Helper
{
    public class FileEncryptionHelper
    {
        private static readonly byte[] _key = Convert.FromBase64String("ASNFZ4mrze/+3LqYdlQyEA==");
        private static readonly byte[] _iv = Convert.FromBase64String("ASNFZ4mrze8=");

        public static void EncryptStream(Stream inputStream, Stream outputStream)
        {
            try
            {
                using (TripleDES tripleDES = TripleDES.Create())
                {
                    tripleDES.Key = _key;
                    tripleDES.IV = _iv;

                    using (CryptoStream cryptoStream = new CryptoStream(outputStream, tripleDES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            cryptoStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while encrypting the stream.");
                throw;
            }
        }

        public static void DecryptStream(Stream inputStream, Stream outputStream)
        {
            try
            {
                using (TripleDES tripleDES = TripleDES.Create())
                {
                    tripleDES.Key = _key;
                    tripleDES.IV = _iv;

                    using (CryptoStream cryptoStream = new CryptoStream(inputStream, tripleDES.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outputStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while decrypting the stream.");
                throw;
            }
        }
    }
}
