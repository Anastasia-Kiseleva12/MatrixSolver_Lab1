using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixSolver_Lab1.Models
{
    public static class ConditionNumberCalculator
    {
        public static double InfinityNorm(double[,] a)
        {
            int n = a.GetLength(0);
            double max = 0;

            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < n; j++)
                    sum += Math.Abs(a[i, j]);

                if (sum > max)
                    max = sum;
            }

            return max;
        }

        public static double ConditionNumberInfinity(double[,] a)
        {
            var inv = InverseMatrixCalculator.Inverse(a);

            return InfinityNorm(a) * InfinityNorm(inv);
        }

        public static double ConditionNumberOne(double[,] a)
        {
            var inv = InverseMatrixCalculator.Inverse(a);

            return OneNorm(a) * OneNorm(inv);
        }

        public static double OneNorm(double[,] a)
        {
            int rows = a.GetLength(0);
            int cols = a.GetLength(1);

            double max = 0.0;

            for (int j = 0; j < cols; j++)
            {
                double sum = 0.0;

                for (int i = 0; i < rows; i++)
                {
                    sum += Math.Abs(a[i, j]);
                }

                if (sum > max)
                {
                    max = sum;
                }
            }

            return max;
        }

        public static double ConditionNumberTwo(double[,] a)
        {
            var matrix = Matrix<double>.Build.DenseOfArray(a);

            var svd = matrix.Svd();

            double sigmaMax = svd.S.AbsoluteMaximum();
            double sigmaMin = svd.S.AbsoluteMinimum();

            if (Math.Abs(sigmaMin) < 1e-15)
            {
                throw new InvalidOperationException("Матрица вырожденная.");
            }

            return sigmaMax / sigmaMin;
        }
    }
}
