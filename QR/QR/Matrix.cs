﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace QR
{
    class Matrix
    {
        public static int[,] CreateMatrix(string encodedLine, int version, int correctionLevel)
        {
            int size = 21 + 4 * (version - 1);
            int[,] matrix = new int[size, size];
            matrix = AddFinderPattern(matrix, version, 0, 0);
            matrix = AddFinderPattern(matrix, version, 0, size - 7);
            matrix = AddFinderPattern(matrix, version, size - 7, 0);
            matrix = AddAlignment(matrix, version);
            matrix = AddSyncLine(matrix);
            matrix = AddVersion(matrix, version);
            matrix = AddData(matrix, encodedLine, correctionLevel, 0); // ПОСЛЕДНИЙ АРГУМЕНТ НУЖНО ПОМЕНЯТЬ В ЗАВИСИМОСТИ ОТ ТОГО, КАКАЯ МАСКА ПОДХОДИТ.
            matrix = AddMask(matrix, correctionLevel);

            DisplayMatrix(matrix);
            return matrix;
        }

        public static void DisplayMatrix(int[,] matrix)
        {
            Console.Write("\n\n\n\n");

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                Console.Write("        ");
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    if (matrix[i, j] == 1) Console.Write("██"); //
                    else Console.Write("  "); // "  "
                    //Console.Write($"{matrix[i, j]} ");
                }
                Console.WriteLine();
            }
            Console.Write("\n\n\n\n");
        }

        public static int[,] AddFinderPattern(int[,] matrix, int version, int x, int y)
        {

            for (int i = -2; i < 9; i++)
            {
                for (int j = -2; j < 9; j++)
                {
                    if (x == matrix.GetLength(0) - 7 && y == 0 && j == -2) continue;
                    if (x == 0 && y == matrix.GetLength(0) - 7 && i == -2) continue;

                    if ((y + i >= 0 && x + j >= 0) && (y + i < matrix.GetLength(0) && x + j < matrix.GetLength(0)))
                    matrix[y + i, x + j] = -1;
                }
            }

            for (int i = 0; i < 7; i++)
            {
                matrix[y, x + i] = 1;
                matrix[y + 6, x + i] = 1;
            }

            for (int i = 0; i < 7; i++)
            {
                matrix[y + i, x] = 1;
                matrix[y + i, x + 6] = 1;
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    matrix[y + 2 + i, x + 2 + j] = 1;
                }
            }



            return matrix;
        }

        public static int[,] AddAlignment(int[,] matrix, int version)
        {
            MainClass main = new MainClass();
            int[,] alignmentArr = new int[40, 7];
            alignmentArr = main.ReadAlignment();
            int[] cords = new int[7];
            for (int i = 0; i < 7; i++)
            {
                cords[i] = alignmentArr[version - 1, i];
            }
            int[,] cordsArr = new int[cords.Length * cords.Length, 2];
            int length = cords.Count(e => e != 0);
            for (int i = 0; i < (cords.Length * cords.Length); i++)
            {
                if (cords[i] == 0) break;
                for (int j = 0; j < cords.Length; j++)
                {
                    if (cords[j] == 0) break;
                    cordsArr[j + length * i, 0] = cords[i];
                    cordsArr[j + length * i, 1] = cords[j];
                }
            }

            for (int i = 0; i < (cords.Length * cords.Length); i++)
            {
                if (cordsArr[i, 0] == 0 || cordsArr[i, 1] == 0) break;
                if ((cordsArr[i, 0] == 6 && cordsArr[i, 1] + 7 >= matrix.GetLength(0)) ||
                    (cordsArr[i, 0] == 6 && cordsArr[i, 1] - 7 <= 0) ||
                    (cordsArr[i, 0] + 7 >= matrix.GetLength(0) && cordsArr[i, 1] - 7 <= 0)) continue;

                int x = cordsArr[i, 0] - 2;
                int y = cordsArr[i, 1] - 2;
                for (int j = 0; j < 3; j++)
                {
                    matrix[y+1, x+1 + j] = -1;
                    matrix[y + 3, x+1 + j] = -1;
                }
                for (int j = 0; j < 3; j++)
                {
                    matrix[y+1 + j, x+1] = -1;
                    matrix[y+1 + j, x + 3] = -1;
                }

                matrix[cordsArr[i, 0], cordsArr[i, 1]] = 1;



                for (int j = 0; j < 5; j++)
                {
                    matrix[y, x + j] = 1;
                    matrix[y + 4, x + j] = 1;
                }

                for (int j = 0; j < 5; j++)
                {
                    matrix[y + j, x] = 1;
                    matrix[y + j, x + 4] = 1;
                }

            }

            return matrix;
        }

        public static int[,] AddSyncLine(int[,] matrix)
        {
            for (int i = 8; i < matrix.GetLength(0) - 8; i++)
            {
                if (i % 2 == 0)
                {
                    matrix[i, 6] = 1;
                    matrix[6, i] = 1;
                }
                else
                {
                    matrix[i, 6] = -1;
                    matrix[6, i] = -1;
                }
            }

            return matrix;
        }

        public static int[,] AddVersion(int[,] matrix, int version)
        {
            if (version >= 7)
            {
                MainClass main = new MainClass();
                string[,] versionArr = new string[34, 3];
                versionArr = main.ReadVersionCode();
                int size = matrix.GetLength(0);

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if (versionArr[version - 7, i][j] == '1')
                        {
                            matrix[size - 11 + i, j] = 1;
                            matrix[j, size - 11 + i] = 1;
                        }
                        else
                        {
                            matrix[size - 11 + i, j] = -1;
                            matrix[j, size - 11 + i] = -1;
                        }
                    }
                }

            }
            return matrix;
        }

        public static int[,] AddMask(int[,] matrix, int correctionLevel)
        {
            MainClass main = new MainClass();
            string[,] maskArr = new string[4, 8];
            maskArr = main.ReadMaskCode();
            string mask = maskArr[correctionLevel - 1, 0]; // ЗАМЕНИТЬ 0 НА НЕОБХОДИМЫЙ НОМЕР ПОСЛЕ ВЫЧИСЛЕНИЯ ОПТИМАЛЬНОЙ
            int size = matrix.GetLength(0);
            // Левый поисковой модуль
            for (int i = 0; i < 6; i++)
            {
                if (mask[i] == '1') matrix[8, i] = 1;
                else matrix[8, i] = -1;
            }

            for (int i = 0; i < 2; i++)
            {
                if (mask[6+i] == '1') matrix[8, 7+i] = 1;
                else matrix[8, 7+i] = -1;
            }

            if (mask[8] == '1') matrix[7, 8] = 1;
            else matrix[7, 8] = -1;

            for (int i = 0; i < 6; i++)
            {
                if (mask[9 + i] == '1') matrix[5-i, 8] = 1;
                else matrix[5-i, 8] = -1;
            }

            // Нижний поисковой модуль
            for (int i = 0; i < 7; i++)
            {
                if (mask[i] == '1') matrix[size - 1 - i, 8] = 1;
                else matrix[size - 1 - i, 8] = -1;
            }
            matrix[size - 8, 8] = 1;

            // Правый поисковой модуль
            for (int i = 0; i < 8; i++)
            {
                if (mask[7+i] == '1') matrix[8, size - 8 + i] = 1;
                else matrix[8, size - 8 + i] = -1;
            }

            return matrix;
        }

        

        public static int[,] AddData(int[,] matrix, string encodedLine, int correctionLevel, int mask)
        {
            string data = encodedLine;
            int size = matrix.GetLength(0);
            int x = size - 1;
            int y = size - 1;

            while (x >= 0)
            {
                data = GoUpwards(matrix, data, size, x, y, mask);
                x -= 2;
                if (x == 6) x -= 1;
                data = GoDownwards(matrix, data, size, x, y, mask);
                x -= 2;

            }
            Console.WriteLine(matrix[9, 0]);
            static string GoUpwards(int[,] matrix, string data, int size, int x, int y, int mask)
            {
                while (y >= 0)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        x -= j;
                        if (x < 0) break;
                        if (matrix[y, x] == 1 || matrix[y, x] == -1) continue; // Все служебные белые клетки должны стать -1 

                        if (data.Length == 0)
                        {
                            matrix[y, x] = 0;
                            continue;
                        }
                        if (data[0] == '1') matrix[y, x] = 1;
                        data = data.Remove(0, 1);

                        if (Dicts.Mask(mask, x, y) == 0)
                        {
                            if (matrix[y, x] == 0) matrix[y, x] = 1;
                            else matrix[y, x] = 0;
                        } // Убрать коментарии, чтобы вернуть всё на место

                        //matrix[y, x] = 1;
                        //DisplayMatrix(matrix);
                        //Thread.Sleep(15);

                    }
                    y -= 1;
                    x += 1;
                }
                if (x == 5) x -= 1;

                return data;
            }

            static string GoDownwards(int[,] matrix, string data, int size, int x, int y, int mask)
            {
                y = 0;
                while (y < size) 
                {
                    for (int j = 0; j < 2; j++)
                    {
                        x -= j;
                        if (x < 0) break;

                        if (matrix[y, x] == 1 || matrix[y, x] == -1) continue;

                        if (data.Length == 0)
                        {
                            matrix[y, x] = 0;
                            continue;
                        }
                        if (data[0] == '1') matrix[y, x] = 1;
                        data = data.Remove(0, 1);

                        if (Dicts.Mask(mask, x, y) == 0)
                        {
                            if (matrix[y, x] == 0) matrix[y, x] = 1;
                            else matrix[y, x] = 0;
                        }

                        //matrix[y, x] = 1;
                        //DisplayMatrix(matrix);
                        //Thread.Sleep(15);

                    }
                    y += 1;
                    x += 1;
                }
                return data;
            }

            return matrix;
        } 
    }
}
