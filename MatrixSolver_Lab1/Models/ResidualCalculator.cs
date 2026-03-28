using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixSolver_Lab1.Models
{
    public static class ResidualCalculator
    {
        public static double[] Calculate(double[,] a, double[] x, double[] b)
        {
            int n = b.Length;
            var residual = new double[n];

            for (int i = 0; i < n; i++)
            {
                double sum = 0.0;

                for (int j = 0; j < n; j++)
                {
                    sum += a[i, j] * x[j];
                }

                residual[i] = sum - b[i];
            }

            return residual;
        }

        public static double InfinityNorm(double[] vector)
        {
            double max = 0.0;

            for (int i = 0; i < vector.Length; i++)
            {
                double value = Math.Abs(vector[i]);
                if (value > max)
                {
                    max = value;
                }
            }

            return max;
        }
    }
}
