using System.Threading.Tasks;

namespace Authomation.Cipher.Interfaces
{
    public interface ICipher
    {
        byte[] Encrypt(byte[] text, byte[] userkey);
        byte[] Decrypt(byte[] text, byte[] userkey);
    }
}