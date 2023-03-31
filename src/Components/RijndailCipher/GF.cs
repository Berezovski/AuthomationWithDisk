using System;
using System.Collections.Generic;
using System.Text;

namespace Authomation.RijndailCipher
{
    /// <summary>
    /// Класс для работы с полем GF (Галуа)
    /// </summary>
    public static class GF
    {
        /// <summary>
        /// Взятие остатка от деления на mod
        /// </summary>
        /// <param name="number"> Число </param> 
        /// <param name="mod"> Модуль полинома </param>
        /// <returns> Остаток многочлена </returns>
        public static uint Mod(uint number, uint mod)
        {
            var numberLength = WorkWithBits.FindBinaryLength(number);
            var modLength = WorkWithBits.FindBinaryLength(mod);

            if (modLength > numberLength) // если число в модуле больше, то уравнение и есть остаток
            {
                return number;
            }

            uint activeDegree;
            // цикл самого деления
            while (true)
            {
                activeDegree = (uint)1 << numberLength - modLength;

                number = number ^ Multy(activeDegree, mod);
                numberLength = WorkWithBits.FindBinaryLength(number);

                if (number == 0)
                {
                    return number;
                }

                if (modLength > numberLength) // есть остаток
                {
                    return number;
                }

            }

        }

        /// <summary>
        /// Взятие целой части от деления многочлена на divider
        /// </summary>
        /// <param name="number"> Число </param> 
        /// <param name="divider"> Делитель полинома </param>
        /// <returns> Целая часть многочлена </returns>
        public static uint Dividing(uint number, uint divider)
        {
            var numberLength = WorkWithBits.FindBinaryLength(number);
            var dividerLength = WorkWithBits.FindBinaryLength(divider);

            if (dividerLength > numberLength) // если число в модуле больше, то уравнение и есть остаток
            {
                return number;
            }

            uint activeDegree;
            uint answer = 0;
            // цикл самого деления
            while (true)
            {
                activeDegree = (uint)1 << numberLength - dividerLength;

                number = number ^ Multy(activeDegree, divider);
                numberLength = WorkWithBits.FindBinaryLength(number);
                answer = answer ^ activeDegree;
                if (number == 0)
                {
                    return answer;
                }

                if (dividerLength > numberLength) // есть остаток
                {
                    return answer;
                }
            }
        }

        /// <summary>
        /// Расширенный алгоритм Евклида адаптированный под GF поле [ a*x + b*y = gcd(a,b) ]
        /// </summary>
        /// <param name="firstCoefficient"> Коэффициент 'a' при x </param> 
        /// <param name="secondCoefficient"> Коэффициент 'b' при x </param>
        /// <param name="x"> Выходное число x </param> 
        /// <param name="y"> Выходное число y </param>
        /// <returns> Возвращает НОД от firstCoefficient и secondCoefficient </returns>
        public static uint AdvancedGCD(uint firstCoefficient, uint secondCoefficient, out uint x, out uint y)
        {
            if (firstCoefficient == 0)
            {
                x = 0; y = 1;
                return secondCoefficient;
            }

            uint x1, y1;
            uint gcd = AdvancedGCD(Mod(secondCoefficient, firstCoefficient), firstCoefficient, out x1, out y1);
            x = y1 ^ Multy(Dividing(secondCoefficient, firstCoefficient), x1);
            y = x1;
            return gcd;
        }

        /// <summary>
        /// Нахождение мультипликативного обратного числа 
        /// </summary>
        /// <param name="number"> Полином </param> 
        /// <param name="module"> Модуль по которому осуществляется поиск обратного (полином) </param>
        /// <returns> Возвращает мультипликативный обратный от числа number </returns>
        public static uint MultiplicativeReverse(uint number, uint module)
        {
            number = Mod(number, module);
            uint x, y;
            // number*x + m*y = 1
            AdvancedGCD(number, module, out x, out y);
            return x;
        }

        /// <summary>
        /// Умножение полиномов
        /// </summary>
        /// <param name="firstNumber"> Первый полином </param> 
        /// <param name="secondNumber"> Второй полином </param>
        /// <returns> Возвращает умножение двух полиномов </returns>
        public static uint Multy(uint firstNumber, uint secondNumber)
        {
            var numberBinaryLengthFirstElement = WorkWithBits.FindBinaryLength(firstNumber);
            var numberBinaryLengthSecondElement = WorkWithBits.FindBinaryLength(secondNumber);

            uint answer = 0;
            uint tmpConjunction;
            uint tmpXor;
            for (int i = 0; i < numberBinaryLengthFirstElement; i++)
            {
                for (int j = 0; j < numberBinaryLengthSecondElement; j++)
                {
                    tmpConjunction = (uint)(WorkWithBits.PrintBit(firstNumber, i) & WorkWithBits.PrintBit(secondNumber, j));
                    tmpXor = (uint)WorkWithBits.PrintBit(answer, i + j) ^ tmpConjunction;
                    answer = (uint)WorkWithBits.SetOrRemove(answer, tmpXor, i + j);
                }
            }

            return answer;
        }

        /// <summary>
        /// Выводит полином в подобающем пользовательском виде 
        /// (степень x это нумерация бита справа налево)
        /// </summary>
        /// <param name="number"> Полином </param> 
        /// <returns> Возвращает строку в виде привычного полинома </returns>
        public static string PrintGfElement(uint number)
        {
            var numberBinaryLength = WorkWithBits.FindBinaryLength(number);
            var answer = new StringBuilder();
            for (int i = numberBinaryLength; i >= 0; i--)
            {
                if (WorkWithBits.PrintBit(number, i) == 1)
                {
                    if (i == 0)
                    {
                        answer.Append(1).Append(" + ");
                        break;
                    }
                    answer.Append("x^").Append(i).Append(" + ");
                }
            }

            answer.Remove(answer.Length - 3, 3);
            return answer.ToString();
        }
    }
}
