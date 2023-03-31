using Authomation.Cipher.Interfaces;
using System;

namespace Authomation.RijndailCipher
{
    /// <summary>
    /// Класс используемый, для шифрования данных
    /// </summary>
    public class Rijndail : ICipher
    {
        private int _Nr;
        private int _Nb;
        private int _Nk;

        /// <summary>
        ///Таблица определяющая количество раундов
        /// </summary>
        readonly int[][] _NrTable = new int[3][]
        {
                new int[3] {10, 12, 14},
                new int[3] {12, 12, 14},
                new int[3] {14, 14, 14}
        };

        /// <summary>
        /// S-блоки
        /// </summary>
        readonly byte[] _sBox = new byte[256]
        {
                0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76,
                0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0,
                0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15,
                0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a, 0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75,
                0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84,
                0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf,
                0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8,
                0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2,
                0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73,
                0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88, 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb,
                0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79,
                0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08,
                0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a,
                0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e,
                0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf,
                0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68, 0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16
        };

        /// <summary>
        /// S-блоки для расшифрования
        /// </summary>
        readonly byte[] _sBoxReverse = new byte[256]
        {
                0x52, 0x09, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38, 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb,
                0x7c, 0xe3, 0x39, 0x82, 0x9b, 0x2f, 0xff, 0x87, 0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb,
                0x54, 0x7b, 0x94, 0x32, 0xa6, 0xc2, 0x23, 0x3d, 0xee, 0x4c, 0x95, 0x0b, 0x42, 0xfa, 0xc3, 0x4e,
                0x08, 0x2e, 0xa1, 0x66, 0x28, 0xd9, 0x24, 0xb2, 0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25,
                0x72, 0xf8, 0xf6, 0x64, 0x86, 0x68, 0x98, 0x16, 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92,
                0x6c, 0x70, 0x48, 0x50, 0xfd, 0xed, 0xb9, 0xda, 0x5e, 0x15, 0x46, 0x57, 0xa7, 0x8d, 0x9d, 0x84,
                0x90, 0xd8, 0xab, 0x00, 0x8c, 0xbc, 0xd3, 0x0a, 0xf7, 0xe4, 0x58, 0x05, 0xb8, 0xb3, 0x45, 0x06,
                0xd0, 0x2c, 0x1e, 0x8f, 0xca, 0x3f, 0x0f, 0x02, 0xc1, 0xaf, 0xbd, 0x03, 0x01, 0x13, 0x8a, 0x6b,
                0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 0xdc, 0xea, 0x97, 0xf2, 0xcf, 0xce, 0xf0, 0xb4, 0xe6, 0x73,
                0x96, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85, 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 0x6e,
                0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89, 0x6f, 0xb7, 0x62, 0x0e, 0xaa, 0x18, 0xbe, 0x1b,
                0xfc, 0x56, 0x3e, 0x4b, 0xc6, 0xd2, 0x79, 0x20, 0x9a, 0xdb, 0xc0, 0xfe, 0x78, 0xcd, 0x5a, 0xf4,
                0x1f, 0xdd, 0xa8, 0x33, 0x88, 0x07, 0xc7, 0x31, 0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f,
                0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 0x0d, 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef,
                0xa0, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5, 0xb0, 0xc8, 0xeb, 0xbb, 0x3c, 0x83, 0x53, 0x99, 0x61,
                0x17, 0x2b, 0x04, 0x7e, 0xba, 0x77, 0xd6, 0x26, 0xe1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0c, 0x7d
        };

        /// <summary>
        /// Таблица, использованная в MixColumns
        /// </summary>
        readonly byte[][] _MixColumnsConst = new byte[4][]
        {
                  new byte[4]{ 0x02, 0x03, 0x01, 0x01},
                  new byte[4]{ 0x01, 0x02, 0x03, 0x01},
                  new byte[4]{ 0x01, 0x01, 0x02, 0x03},
                  new byte[4]{ 0x03, 0x01, 0x01, 0x02 }
        };

        /// <summary>
        /// Таблица, использованная в InvMixColumns (при расшифровании)
        /// </summary>
        readonly byte[][] _InvMixColumnsConst = new byte[4][]
        {
            new byte[4] { 0x0e, 0x0b, 0x0d, 0x09 },
            new byte[4]{ 0x09, 0x0e, 0x0b, 0x0d},
            new byte[4]{ 0x0d, 0x09, 0x0e, 0x0b},
            new byte[4]{ 0x0b, 0x0d, 0x09, 0x0e }
        };

        /// <summary>
        /// Таблица, использованная для расширения ключа (KeyExpansion)
        /// </summary>
        readonly byte[][] _RCon = new byte[15][]
        {
            new byte[4] { 0, 0, 0, 0 },
            new byte[4] { 0x01, 0, 0, 0 },
            new byte[4]{ 0x02, 0, 0, 0},
            new byte[4]{ 0x04, 0, 0, 0},
            new byte[4]{ 0x08, 0, 0, 0 },
            new byte[4] { 0x10, 0, 0, 0 },
            new byte[4]{ 0x20, 0, 0, 0},
            new byte[4]{ 0x40, 0, 0, 0},
            new byte[4]{ 0x80, 0, 0, 0 },
            new byte[4]{ 0x1B, 0, 0, 0},
            new byte[4]{ 0x36, 0, 0, 0 },
            new byte[4]{ 0x4C, 0, 0, 0},
            new byte[4]{ 0xD8, 0, 0, 0 },
            new byte[4]{ 0xAB, 0, 0, 0},
            new byte[4]{ 0x4D, 0, 0, 0 }
        };

        /// <summary>
        /// Конструктор, который задаёт количество битов входного текста и входного ключа
        /// </summary>
        public Rijndail()
        {
            _Nb = 4;
            _Nk = 4;
            _Nr = _NrTable[0][0];
        }

        /// <summary>
        /// Шифрует данные и  показывает количество времени, затраченное на шифрование
        /// </summary>
        /// <param name="text"> Текст для шифрования </param>
        /// <param name="userKey"> Ключ пользователя (от 2 байт) </param>
        /// <param name="time"> Время шифрования </param>
        /// <returns> Зашифрованные биты </returns>
        public byte[] Encrypt(byte[] text, byte[] userKey)
        {
            if (userKey.Length < 2)
            {
                throw new ArgumentException(userKey.Length.ToString());
            }

            var needByteArray = GetAllBytesAndAppendInfoBlock(text);
            var textLength = _Nb * 4;
            var textArrayFixedLength = GetTextArray(needByteArray, textLength);
            var firstPartOfUserKey = new byte[userKey.Length / 2];
            var secondPartOfUserKey = new byte[userKey.Length - firstPartOfUserKey.Length];
            Array.Copy(userKey, firstPartOfUserKey, firstPartOfUserKey.Length);
            Array.Copy(userKey, firstPartOfUserKey.Length, secondPartOfUserKey, 0, secondPartOfUserKey.Length);
            var tmpKeys = KeyExpansion(firstPartOfUserKey);
            var roundKeys = GetTextArray(GetByteArrayFromTextArray(tmpKeys), textLength);

            var secondTmpKeys = KeyExpansionForSecondPart(secondPartOfUserKey);
            var secondPartRoundKeys = GetTextArray(GetByteArrayFromTextArray(secondTmpKeys), textLength);
            for (int i = 0; i < textArrayFixedLength.Length; i++)
            {
                textArrayFixedLength[i] = Cipher(textArrayFixedLength[i], roundKeys, secondPartRoundKeys);
            }

            return GetByteArrayFromTextArray(textArrayFixedLength);
        }

        /// <summary>
        /// Функция расширения ключа, которая генерирует "слова" по 4 байта (для правой части ключа)
        /// </summary>
        /// <param name="userKey"> Ключ пользователя </param>
        /// <returns> Набор слов (из которых будут состоять раундовые ключи) </returns>
        private byte[][] KeyExpansionForSecondPart(byte[] userKey)
        {
            var keyByteLength = _Nk * 4;
            var key = new byte[keyByteLength];
            for (int i = 0; i < userKey.Length; i++)
            {
                key[i] = userKey[i];
            }

            var words = new byte[(_Nr + 1) * _Nb][];
            // первые значения
            for (int i = 0; i < _Nk; i++)
            {
                words[i] = new byte[4];
                for (int j = 0; j < 4; j++)
                {
                    words[i][j] = key[j + 4 * i];
                }
            }

            for (int i = _Nk; i < words.Length; i++)
            {

                if (i % _Nk != 0)
                {
                    words[i] = XorWord(words[i - 1], words[i - _Nk], 4);
                }
                else
                {
                    words[i] = XorWord(XorWord(SubWord(words[i - 1]), _RCon[i / _Nb], 4), words[i - _Nk], 4);
                }
            }

            return words;
        }

        /// <summary>
        /// Расшифровывает данные и показывает количество времени, затраченное на расшифрование
        /// </summary>
        /// <param name="cipherText"> Текст для расшифрования </param>
        /// <param name="userKey"> Ключ пользователя (от 2 байт) </param>
        /// <param name="time"> Время расшифрования </param>
        /// <returns> Расшифрованные биты </returns>
        public byte[] Decrypt(byte[] cipherText, byte[] userKey)
        {
            if (userKey.Length < 2)
            {
                throw new ArgumentException(userKey.Length.ToString());
            }

            var textLength = _Nb * 4;
            var textArrayFixedLength = GetTextArray(cipherText, textLength);
            if (textArrayFixedLength.Length < 2)
            {
                return cipherText;
            }

            var firstPartOfUserKey = new byte[userKey.Length / 2];
            var secondPartOfUserKey = new byte[userKey.Length - firstPartOfUserKey.Length];
            Array.Copy(userKey, firstPartOfUserKey, firstPartOfUserKey.Length);
            Array.Copy(userKey, firstPartOfUserKey.Length, secondPartOfUserKey, 0, secondPartOfUserKey.Length);

            var tmpKeys = KeyExpansion(firstPartOfUserKey);
            var roundKeys = GetTextArray(GetByteArrayFromTextArray(tmpKeys), textLength);

            var secondTmpKeys = KeyExpansionForSecondPart(secondPartOfUserKey);
            var secondPartRoundKeys = GetTextArray(GetByteArrayFromTextArray(secondTmpKeys), textLength);

            for (int i = 0; i < textArrayFixedLength.Length; i++)
            {
                textArrayFixedLength[i] = Decipher(textArrayFixedLength[i], roundKeys, secondPartRoundKeys);
            }

            return GetRealBytesWithoutInfoBlock(GetByteArrayFromTextArray(textArrayFixedLength));
        }

        /// <summary>
        /// Возвращает все те же биты, но с добавленным блоком информации (о предыдущем блоке)
        /// </summary>
        /// <param name="text"> Начальные биты </param>
        /// <returns> Биты, кратные указанному количеству блока текста </returns>
        private byte[] GetAllBytesAndAppendInfoBlock(byte[] text)
        {
            var textByteLength = _Nb * 4;
            // сколько байт лишних       
            var missingSize = (textByteLength - text.Length % textByteLength) % textByteLength;
            // нужный нам текст
            var answer = new byte[text.Length + missingSize + textByteLength];
            Array.Copy(text, answer, text.Length);
            answer[text.Length + missingSize] = (byte)missingSize;
            return answer;
        }

        /// <summary>
        /// Возвращает все те же биты, но убирает лишние биты, с блоком информации.
        /// </summary>
        /// <param name="bytes"> Текст с инфо блоком в конце </param>
        /// <returns> Оригинальный текст (может быть не кратен указанному количеству блока текста) </returns>
        private byte[] GetRealBytesWithoutInfoBlock(byte[] bytes)
        {
            var textLength = _Nb * 4;
            var textArray = GetTextArray(bytes, textLength);
            var misingSize = textArray[textArray.Length - 1][0] % textLength;
            var answer = new byte[(textArray.Length - 1) * textLength - misingSize];
            Array.Copy(bytes, answer, answer.Length);
            return answer;

        }

        /// <summary>
        /// Функция шифрования одного блока
        /// </summary>
        /// <param name="text"> Блок текста </param>
        /// <param name="roundKeys"> Набор раундовых ключей левой части пользов. ключа </param>
        /// <param name="roundKeysForSecondPart"> Набор раундовых ключей правой части пользов. ключа </param>
        /// <returns> Зашифрованный блок </returns>
        private byte[] Cipher(byte[] text, byte[][] roundKeys, byte[][] roundKeysForSecondPart)
        {
            byte[][] stateMatrix = GetStateMatrix(text, _Nb);
            // далее уже алгоритм шифрования
            stateMatrix = AddRoundKey(stateMatrix, roundKeys[0]);
            stateMatrix = AddRoundKey(stateMatrix, roundKeysForSecondPart[0]);

            for (int i = 1; i < _Nr; i++)
            {
                stateMatrix = SubBytes(stateMatrix);
                stateMatrix = ShiftRows(stateMatrix);
                stateMatrix = MixColumns(stateMatrix);
                stateMatrix = AddRoundKey(stateMatrix, roundKeys[i]);
                stateMatrix = AddRoundKey(stateMatrix, roundKeysForSecondPart[i]);

                if (i % 2 == 0)
                {
                    stateMatrix = NewMixColumns(stateMatrix);
                }
            }
            stateMatrix = SubBytes(stateMatrix);
            stateMatrix = ShiftRows(stateMatrix);
            stateMatrix = AddRoundKey(stateMatrix, roundKeys[_Nr]);
            stateMatrix = AddRoundKey(stateMatrix, roundKeysForSecondPart[_Nr]);

            return GetByteArrayFromStateMatrix(stateMatrix);
        }

        /// <summary>
        /// Функция расшифрования одного блока
        /// </summary>
        /// <param name="text"> Блок текста </param>
        /// <param name="roundKeys"> Набор раундовых ключей левой части пользов. ключа </param>
        /// <param name="roundKeysForSecondPart"> Набор раундовых ключей правой части пользов. ключа </param>
        /// <returns> Расшифрованный блок </returns>
        private byte[] Decipher(byte[] text, byte[][] roundKeys, byte[][] roundKeysForSecondPart)
        {
            byte[][] stateMatrix = GetStateMatrix(text, _Nb);
            // далее уже алгоритм дешифрования
            stateMatrix = AddRoundKey(stateMatrix, roundKeysForSecondPart[_Nr]);
            stateMatrix = AddRoundKey(stateMatrix, roundKeys[_Nr]);
            stateMatrix = InvShiftRows(stateMatrix);
            stateMatrix = InvSubBytes(stateMatrix);

            for (int i = _Nr - 1; i >= 1; i--)
            {
                if (i % 2 == 0)
                {
                    stateMatrix = NewInvMixColumns(stateMatrix);
                }

                stateMatrix = AddRoundKey(stateMatrix, roundKeysForSecondPart[i]);
                stateMatrix = AddRoundKey(stateMatrix, roundKeys[i]);
                stateMatrix = InvMixColumns(stateMatrix);
                stateMatrix = InvShiftRows(stateMatrix);
                stateMatrix = InvSubBytes(stateMatrix);
            }

            stateMatrix = AddRoundKey(stateMatrix, roundKeysForSecondPart[0]);
            stateMatrix = AddRoundKey(stateMatrix, roundKeys[0]);

            return GetByteArrayFromStateMatrix(stateMatrix);
        }

        /// <summary>
        /// Сложению по модулю 2 с ключём
        /// </summary>
        /// <param name="stateMatrix"> Матрица состояния (блок текста) </param>
        /// <param name="roundKey"> Раундовый ключ </param>
        /// <returns> Результат сложения </returns>
        private byte[][] AddRoundKey(byte[][] stateMatrix, byte[] roundKey)
        {
            byte[][] answer = new byte[stateMatrix.Length][];
            for (int i = 0; i < stateMatrix.Length; i++)
            {
                answer[i] = new byte[stateMatrix[0].Length];
            }

            for (int i = 0; i < answer[0].Length; i++)
            {
                for (int j = 0; j < answer.Length; j++)
                {
                    answer[j][i] = (byte)(stateMatrix[j][i] ^ roundKey[i * answer.Length + j]);
                }
            }

            return answer;
        }

        /// <summary>
        /// Обратно смешивает данные матрицы состояний (новая)
        /// </summary>
        /// <param name="stateMatrix"> Матрица состояний </param>
        /// <returns> Трансформированную матрицу состояний </returns>
        private byte[][] NewInvMixColumns(byte[][] stateMatrix)
        {
            byte[][] matrixStateAfterInvMixColomns = new byte[4][];
            for (int j = 0; j < 4; j++)
            {
                matrixStateAfterInvMixColomns[j] = new byte[_Nb];
            }

            for (int j = 0; j < 4; j++)
            {
                matrixStateAfterInvMixColomns[j] = new byte[_Nb];
            }

            for (int i = 0; i < _Nb; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrixStateAfterInvMixColomns[j][i] = 0;
                    for (int p = 0; p < 4; p++)
                    {
                        matrixStateAfterInvMixColomns[j][i] = (byte)(matrixStateAfterInvMixColomns[j][i] ^
                            (byte)GF.Mod(GF.Multy(stateMatrix[p][i], _InvMixColumnsConst[j][p]), 0x133));
                    }

                }
            }

            return matrixStateAfterInvMixColomns;
        }

        /// <summary>
        /// Смешивает данные матрицы состояний (новая)
        /// </summary>
        /// <param name="stateMatrix"> Матрица состояний </param>
        /// <returns> Трансформированную матрицу состояний </returns>
        private byte[][] NewMixColumns(byte[][] stateMatrix)
        {
            byte[][] matrixStateAfterMixColomns = new byte[4][];
            for (int j = 0; j < 4; j++)
            {
                matrixStateAfterMixColomns[j] = new byte[_Nb];
            }

            for (int i = 0; i < _Nb; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrixStateAfterMixColomns[j][i] = 0;
                    for (int p = 0; p < 4; p++)
                    {
                        matrixStateAfterMixColomns[j][i] = (byte)(matrixStateAfterMixColomns[j][i] ^
                            (byte)GF.Mod(GF.Multy(stateMatrix[p][i], _MixColumnsConst[j][p]), 0x133));
                    }
                }
            }

            return matrixStateAfterMixColomns;
        }

        /// <summary>
        /// Функция расширения ключа, которая генерирует "слова" по 4 байта
        /// </summary>
        /// <param name="userKey"> Ключ пользователя </param>
        /// <returns> Набор слов (из которых будут состоять раундовые ключи) </returns>
        private byte[][] KeyExpansion(byte[] userKey)
        {
            int keyByteLength = _Nk * 4;
            byte[] key = new byte[keyByteLength];
            for (int i = 0; i < keyByteLength; i++)
            {
                key[i] = userKey[i % userKey.Length];
            }

            byte[][] words = new byte[(_Nr + 1) * _Nb][];

            // первые значения
            for (int i = 0; i < _Nk; i++)
            {
                words[i] = new byte[4];

                for (int j = 0; j < 4; j++)
                {
                    words[i][j] = key[j + 4 * i];
                }
            }

            for (int i = _Nk; i < words.Length; i++)
            {

                if (i % _Nk != 0)
                {
                    words[i] = XorWord(words[i - 1], words[i - _Nk], 4);
                }
                else
                {
                    words[i] = XorWord(XorWord(SubWord(RotWord(words[i - 1])), _RCon[i / _Nb], 4), words[i - _Nk], 4);
                }
            }

            return words;

        }

        /// <summary>
        /// Преобразует двумерный массив в одномерный (слева направо, сверху вниз)
        /// </summary>
        /// <param name="bivariateArray"> Двумерный массив </param>
        /// <returns> Одномерный массив  </returns>
        private byte[] GetByteArrayFromTextArray(byte[][] bivariateArray)
        {
            byte[] allBytes = new byte[bivariateArray.Length * bivariateArray[0].Length];
            for (int i = 0; i < bivariateArray.Length; i++)
            {
                Array.Copy(bivariateArray[i], 0, allBytes, i * bivariateArray[0].Length, bivariateArray[0].Length);
            }

            return allBytes;
        }

        /// <summary>
        /// Преобразует одномерный в двумерный массив с указанной блиной строк
        /// </summary>
        /// <param name="allBytes"> Байты одномерного массива </param>
        /// <param name="textLength"> Длина строки в байтах </param>
        /// <returns> Двумерный массив </returns>
        private byte[][] GetTextArray(byte[] allBytes, int textLength)
        {
            byte[][] textArray;
            // случай когда количество элементов allBytes не кратно нужной длине
            if (allBytes.Length % textLength != 0)
            {
                int lastBlockLength = allBytes.Length % textLength;
                textArray = new byte[allBytes.Length / textLength + 1][];

                for (int i = 0; i < textArray.Length - 1; i++)
                {
                    textArray[i] = new byte[textLength];
                    Array.Copy(allBytes, i * textLength, textArray[i], 0, textLength);
                }

                textArray[textArray.Length - 1] = new byte[textLength];
                Array.Copy(allBytes, (textArray.Length - 1) * textLength,
                    textArray[textArray.Length - 1], 0, lastBlockLength);
            }
            else
            {
                textArray = new byte[allBytes.Length / textLength][];
                for (int i = 0; i < textArray.Length; i++)
                {
                    textArray[i] = new byte[textLength];

                    Array.Copy(allBytes, i * textLength, textArray[i], 0, textLength);
                }
            }

            return textArray;
        }

        /// <summary>
        /// Xor слов
        /// </summary>
        /// <param name="word1"> слово 1</param>
        /// <param name="word2"> слово 2</param>
        /// <param name="wordLength"> длина слов </param>
        /// <returns> Результат xor </returns>
        private byte[] XorWord(byte[] word1, byte[] word2, int wordLength)
        {
            byte[] answer = new byte[wordLength];

            for (int i = 0; i < wordLength; i++)
            {
                answer[i] = (byte)(word1[i] ^ word2[i]);
            }

            return answer;
        }

        /// <summary>
        /// Циклическая перестановка слова
        /// </summary>
        /// <param name="bt"> Слово </param>
        /// <returns> Результат перестановки </returns>
        private byte[] RotWord(byte[] bt)
        {
            byte[] answer = new byte[bt.Length];

            for (int i = 1; i < bt.Length; i++)
            {
                answer[i - 1] = bt[i];
            }
            answer[answer.Length - 1] = bt[0];

            return answer;
        }

        /// <summary>
        /// Трансформация слова через константную таблицу S-box
        /// </summary>
        /// <param name="bt"> Слово </param>
        /// <returns> Результат трансформации </returns>
        private byte[] SubWord(byte[] bt)
        {
            byte[] answer = new byte[bt.Length];

            for (int i = 0; i < bt.Length; i++)
            {
                answer[i] = _sBox[bt[i]];
            }

            return answer;
        }

        /// <summary>
        /// Преобразует из одномерного массива в матрицу состояний (где слова: сверху вниз, слева направо)
        /// </summary>
        /// <param name="allBytes"> Массив </param>
        /// <param name="colomnCount"> Количество колонок в матрице </param>
        /// <returns> Возвращает матрицу состояния </returns>
        private byte[][] GetStateMatrix(byte[] allBytes, int colomnCount)
        {
            byte[][] matrixState = new byte[4][];

            for (byte j = 0; j < 4; j++)
            {
                matrixState[j] = new byte[colomnCount];
            }

            for (byte i = 0; i < colomnCount; i++)
            {
                for (byte j = 0; j < 4; j++)
                {
                    matrixState[j][i] = allBytes[4 * i + j];
                }
            }

            return matrixState;
        }

        /// <summary>
        /// Смешивает данные матрицы состояний
        /// </summary>
        /// <param name="stateMatrix"> Матрица состояний </param>
        /// <returns> Трансформированную матрицу состояний </returns>
        private byte[][] MixColumns(byte[][] stateMatrix)
        {
            byte[][] matrixStateAfterMixColomns = new byte[4][];

            for (int j = 0; j < 4; j++)
            {
                matrixStateAfterMixColomns[j] = new byte[_Nb];
            }

            for (int i = 0; i < _Nb; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrixStateAfterMixColomns[j][i] = 0;
                    for (int p = 0; p < 4; p++)
                    {
                        matrixStateAfterMixColomns[j][i] = (byte)(matrixStateAfterMixColomns[j][i] ^
                            (byte)GF.Mod(GF.Multy(stateMatrix[p][i], _MixColumnsConst[j][p]), 0x11B));
                    }
                }
            }

            return matrixStateAfterMixColomns;
        }

        /// <summary>
        /// Преобразует байты из матрицы состояний одномерный массив (где слова: сверху вниз, слева направо)
        /// </summary>
        /// <param name="stateMatrix"> Матрицы состояний </param>
        /// <returns> Одномерный массив </returns>
        private byte[] GetByteArrayFromStateMatrix(byte[][] stateMatrix)
        {
            byte[] answer = new byte[stateMatrix[0].Length * 4];

            for (int i = 0; i < stateMatrix[0].Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    answer[j + i * 4] = stateMatrix[j][i];
                }
            }

            return answer;
        }

        /// <summary>
        /// Обратно смешивает данные матрицы состояний
        /// </summary>
        /// <param name="stateMatrix"> Матрица состояний </param>
        /// <returns> Трансформированную матрицу состояний </returns>
        private byte[][] InvMixColumns(byte[][] stateMatrix)
        {
            byte[][] matrixStateAfterInvMixColomns = new byte[4][];

            for (int j = 0; j < 4; j++)
            {
                matrixStateAfterInvMixColomns[j] = new byte[_Nb];
            }

            for (int j = 0; j < 4; j++)
            {
                matrixStateAfterInvMixColomns[j] = new byte[_Nb];
            }

            for (int i = 0; i < _Nb; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrixStateAfterInvMixColomns[j][i] = 0;
                    for (int p = 0; p < 4; p++)
                    {
                        matrixStateAfterInvMixColomns[j][i] = (byte)(matrixStateAfterInvMixColomns[j][i] ^
                            (byte)GF.Mod(GF.Multy(stateMatrix[p][i], _InvMixColumnsConst[j][p]), 0x11B));
                    }
                }
            }

            return matrixStateAfterInvMixColomns;
        }

        /// <summary>
        /// Трансформация матрицы состояния через S-блоки
        /// </summary>
        /// <param name="stateMatrix"> Матрица состояния </param>
        /// <returns>Трансформированную матрицу состояний</returns>
        private byte[][] SubBytes(byte[][] stateMatrix)
        {
            byte[][] answer = new byte[stateMatrix.Length][];

            for (int i = 0; i < stateMatrix.Length; i++)
            {
                answer[i] = new byte[stateMatrix[0].Length];
            }

            for (int i = 0; i < stateMatrix.Length; i++)
            {
                for (int j = 0; j < stateMatrix[0].Length; j++)
                {
                    answer[i][j] = _sBox[stateMatrix[i][j]];
                }
            }

            return answer;
        }

        /// <summary>
        /// Обратная трансформация матрицы состояния через S-блоки
        /// </summary>
        /// <param name="stateMatrix"> Матрица состояния </param>
        /// <returns>Трансформированную матрицу состояний</returns>
        private byte[][] InvSubBytes(byte[][] stateMatrix)
        {
            byte[][] answer = new byte[stateMatrix.Length][];

            for (int i = 0; i < stateMatrix.Length; i++)
            {
                answer[i] = new byte[stateMatrix[0].Length];
            }

            for (int i = 0; i < stateMatrix.Length; i++)
            {
                for (int j = 0; j < stateMatrix[0].Length; j++)
                {
                    answer[i][j] = _sBoxReverse[stateMatrix[i][j]];
                }
            }

            return answer;
        }

        /// <summary>
        /// Смещение строк в матрице состояние 
        /// </summary>
        /// <param name="stateMatrix"> Матрица состояния </param>
        /// <returns> Смещённую матрицу состояния </returns>
        private byte[][] ShiftRows(byte[][] stateMatrix)
        {
            byte[][] copyStateMatrix = new byte[stateMatrix.Length][];

            for (int i = 0; i < stateMatrix.Length; i++)
            {
                copyStateMatrix[i] = (byte[])stateMatrix[i].Clone();
            }

            byte tmp;

            for (int i = 0; i < copyStateMatrix.Length; i++)
            {
                tmp = copyStateMatrix[i][0];
                for (int j = 0; j < copyStateMatrix[0].Length - 1; j++)
                {
                    copyStateMatrix[i][j] = copyStateMatrix[i][j + 1];
                }

                copyStateMatrix[i][copyStateMatrix[0].Length - 1] = tmp;
            }

            return copyStateMatrix;
        }

        /// <summary>
        /// Обратное смещение строк в матрице состояние 
        /// </summary>
        /// <param name="stateMatrix"> Матрица состояния </param>
        /// <returns> Смещённую матрицу состояния </returns>
        private byte[][] InvShiftRows(byte[][] stateMatrix)
        {
            byte[][] copyStateMatrix = new byte[stateMatrix.Length][];

            for (int i = 0; i < stateMatrix.Length; i++)
            {
                copyStateMatrix[i] = (byte[])stateMatrix[i].Clone();
            }

            byte tmp;
            for (int i = 0; i < copyStateMatrix.Length; i++)
            {
                tmp = copyStateMatrix[i][copyStateMatrix[0].Length - 1];
                for (int j = copyStateMatrix[0].Length - 1; j > 0; j--)
                {
                    copyStateMatrix[i][j] = copyStateMatrix[i][j - 1];
                }

                copyStateMatrix[i][0] = tmp;
            }

            return copyStateMatrix;
        }
    }
}