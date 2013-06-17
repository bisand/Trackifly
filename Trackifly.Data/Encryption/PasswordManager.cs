using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Trackifly.Data.Encryption
{
    public class PasswordManager
    {
        public byte[] HashPassword(string password, byte[] salt)
        {
            return HashPassword(Encoding.UTF8.GetBytes(password), salt);
        }

        public byte[] HashPassword(byte[] password, byte[] salt)
        {
            var saltedPassword = salt.Concat(password).ToArray();
            var hashedPassword = new SHA512Managed().ComputeHash(saltedPassword);
            var hashedSalt = new SHA512Managed().ComputeHash(salt);
            var saltedHash = hashedSalt.Concat(hashedPassword).ToArray();
            return saltedHash;
        }

        public bool ConfirmPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            var hash = HashPassword(password, passwordSalt);
            return passwordHash.SequenceEqual(hash);
        }
    }
}