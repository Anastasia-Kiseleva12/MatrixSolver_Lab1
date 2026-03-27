using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixSolver_Lab1.Models
{
    public static class GaussianSolver
    {
        public static double[] Solve(double[,] inputA, double[] inputB)
        {
            int n = inputB.Length;

            var a = (double[,])inputA.Clone();
            var b = (double[])inputB.Clone();

            ForwardElimination(a, b, n);
            return BackSubstitution(a, b, n);
        }

        private static void ForwardElimination(double[,] a, double[] b, int n)
        {
            for (int k = 0; k < n - 1; k++)
            {
                if (Math.Abs(a[k, k]) < 1e-12)
                {
                    throw new InvalidOperationException(
                        $"Нулевой или слишком маленький ведущий элемент на шаге {k}. Простой метод Гаусса не может продолжить решение.");
                }

                for (int i = k + 1; i < n; i++)
                {
                    double factor = a[i, k] / a[k, k];

                    for (int j = k; j < n; j++)
                    {
                        a[i, j] -= factor * a[k, j];
                    }

                    b[i] -= factor * b[k];
                }
            }

            if (Math.Abs(a[n - 1, n - 1]) < 1e-12)
            {
                throw new InvalidOperationException(
                    "На последнем шаге получен нулевой или слишком маленький диагональный элемент.");
            }
        }

        private static double[] BackSubstitution(double[,] a, double[] b, int n)
        {
            var x = new double[n];

            for (int i = n - 1; i >= 0; i--)
            {
                double sum = 0;

                for (int j = i + 1; j < n; j++)
                {
                    sum += a[i, j] * x[j];
                }

                x[i] = (b[i] - sum) / a[i, i];
            }

            return x;
        }
    }
}
