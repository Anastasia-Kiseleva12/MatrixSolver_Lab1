using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixSolver_Lab1.Models
{
    public static class MatrixGenerator
    {
        private static readonly Random _random = new();

        public static LinearSystem Generate(int n = 6, double minValue = -10, double maxValue = 10)
        {
            var a = new double[n, n];
            var b = new double[n];

            for (int i = 0; i < n; i++)
            {
                double rowSum = 0;

                for (int j = 0; j < n; j++)
                {
                    a[i, j] = NextDouble(minValue, maxValue);
                    rowSum += Math.Abs(a[i, j]);
                }

                a[i, i] += rowSum + 1.0;
                b[i] = NextDouble(minValue, maxValue);
            }

            return new LinearSystem(a, b);
        }

        private static double NextDouble(double min, double max)
        {
            return min + _random.NextDouble() * (max - min);
        }

        public static string MatrixToString(double[,] matrix)
        {
            var sb = new StringBuilder();
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    sb.Append($"{matrix[i, j],16:E4}");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static string VectorToString(double[] vector)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < vector.Length; i++)
            {
                sb.AppendLine($"{vector[i]:E6}");
            }

            return sb.ToString();
        }

        public static LinearSystem GenerateBadSystem()
        {
            var a = new double[,]
            {
                { 1e-10, 1, 1, 1, 1, 1 },
                { 1,     2, 1, 1, 1, 1 },
                { 1,     1, 2, 1, 1, 1 },
                { 1,     1, 1, 2, 1, 1 },
                { 1,     1, 1, 1, 2, 1 },
                { 1,     1, 1, 1, 1, 2 }
            };

            var b = new double[] { 6, 7, 7, 7, 7, 7 };

            return new LinearSystem(a, b);
        }

        public static LinearSystem GenerateVeryBadSystem()
        {
            int n = 6;
            var a = new double[n, n];
            var b = new double[n];

            // почти одинаковые строки = плохо обусловленная матрица
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    a[i, j] = 1.0;
                }
            }

            // делаем очень маленький элемент
            a[0, 0] = 1e-16;

            // чуть различаем строки
            for (int i = 1; i < n; i++)
            {
                a[i, i] = 1.000001;
            }

            for (int i = 0; i < n; i++)
            {
                b[i] = n + i;
            }

            return new LinearSystem(a, b);
        }
    }
}
