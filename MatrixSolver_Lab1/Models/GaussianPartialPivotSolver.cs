using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixSolver_Lab1.Models
{
    public static class GaussianPartialPivotSolver
    {
        public static double[] Solve(double[,] inputA, double[] inputB)
        {
            int n = inputB.Length;

            var a = (double[,])inputA.Clone();
            var b = (double[])inputB.Clone();

            for (int k = 0; k < n - 1; k++)
            {
                int maxRow = k;
                double maxValue = Math.Abs(a[k, k]);

                for (int i = k + 1; i < n; i++)
                {
                    if (Math.Abs(a[i, k]) > maxValue)
                    {
                        maxValue = Math.Abs(a[i, k]);
                        maxRow = i;
                    }
                }

                if (maxValue < 1e-12)
                    throw new InvalidOperationException("Матрица вырожденная или плохо обусловленная.");

                if (maxRow != k)
                {
                    SwapRows(a, b, k, maxRow);
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

            return BackSubstitution(a, b, n);
        }

        private static void SwapRows(double[,] a, double[] b, int r1, int r2)
        {
            int n = b.Length;

            for (int j = 0; j < n; j++)
            {
                (a[r1, j], a[r2, j]) = (a[r2, j], a[r1, j]);
            }

            (b[r1], b[r2]) = (b[r2], b[r1]);
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
