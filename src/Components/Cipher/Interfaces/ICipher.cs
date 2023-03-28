using System.Threading.Tasks;

namespace Authomation.Cipher.Interfaces
{
    public interface ICipher
    {
        Task<byte[]> Encrypt(byte[] text, byte[] userkey);
        Task<byte[]> Decrypt(byte[] text, byte[] userkey);
    }
}