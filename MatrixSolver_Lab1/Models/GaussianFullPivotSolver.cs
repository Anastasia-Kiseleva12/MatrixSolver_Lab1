using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixSolver_Lab1.Models
{
    public static class GaussianFullPivotSolver
    {
        public static double[] Solve(double[,] inputA, double[] inputB)
        {
            int n = inputB.Length;

            var a = (double[,])inputA.Clone();
            var b = (double[])inputB.Clone();

            var colPerm = new int[n];
            for (int i = 0; i < n; i++)
                colPerm[i] = i;

            for (int k = 0; k < n - 1; k++)
            {
                int maxRow = k;
                int maxCol = k;
                double maxValue = Math.Abs(a[k, k]);

                for (int i = k; i < n; i++)
                {
                    for (int j = k; j < n; j++)
                    {
                        if (Math.Abs(a[i, j]) > maxValue)
                        {
                            maxValue = Math.Abs(a[i, j]);
                            maxRow = i;
                            maxCol = j;
                        }
                    }
                }

                if (maxValue < 1e-12)
                    throw new InvalidOperationException("Матрица вырождена.");

                if (maxRow != k)
                    SwapRows(a, b, k, maxRow);

                if (maxCol != k)
                {
                    SwapColumns(a, k, maxCol);
                    (colPerm[k], colPerm[maxCol]) = (colPerm[maxCol], colPerm[k]);
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

            var x = BackSubstitution(a, b, n);

            var result = new double[n];
            for (int i = 0; i < n; i++)
            {
                result[colPerm[i]] = x[i];
            }

            return result;
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

        private static void SwapColumns(double[,] a, int c1, int c2)
        {
            int n = a.GetLength(0);

            for (int i = 0; i < n; i++)
            {
                (a[i, c1], a[i, c2]) = (a[i, c2], a[i, c1]);
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
