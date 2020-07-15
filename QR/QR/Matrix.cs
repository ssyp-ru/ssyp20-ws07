﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QR
{
    class Matrix
    {
        public static int[,] CreateMatrix(int version)
        {
            int size = 21 + 4 * (version - 1);
            int[,] matrix = new int[size, size];
            matrix = AddFinderPattern(matrix, version, 0, 0);
            matrix = AddFinderPattern(matrix, version, 0, size - 7);
            matrix = AddFinderPattern(matrix, version, size - 7, 0);
            matrix = AddAlignment(matrix, version);
            matrix = AddSyncLine(matrix);

            DisplayMatrix(matrix);
            return matrix;
        }

        public static void DisplayMatrix(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    if (matrix[i, j] == 1) Console.Write("██");
                    else Console.Write("  "); // "  "
                    //Console.Write($"{matrix[i, j]} ");
                }
                Console.WriteLine();
            }
        }

        public static int[,] AddFinderPattern(int[,] matrix, int version, int x, int y)
        {
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
                matrix[cordsArr[i, 0], cordsArr[i, 1]] = 1;
                int x = cordsArr[i, 0] - 2;
                int y = cordsArr[i, 1] - 2;
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
                    matrix[i, 6] = 0;
                    matrix[6, i] = 0;
                }
            }

            return matrix;
        }
    }
}
