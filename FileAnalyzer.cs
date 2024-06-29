using System.Security.Cryptography;

namespace FolderSyncronization
{
    public class FileAnalyzer
    {
        
        public byte[] calculateMD5(string filePath)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        return md5.ComputeHash(stream);  
                    }
                }
            } catch (Exception)
            {
                throw;
            }
        }

        public bool CompareFiles(string filePath1, string filePath2)
        {
            try
            {
                return (calculateMD5(filePath1).SequenceEqual(calculateMD5(filePath2)));
            } catch(Exception)
            {
                throw;
            }
        }

    }
}
