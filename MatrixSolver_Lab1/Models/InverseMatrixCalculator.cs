using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixSolver_Lab1.Models
{
    public static class InverseMatrixCalculator
    {
        public static double[,] Inverse(double[,] a)
        {
            int n = a.GetLength(0);
            var inverse = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                var e = new double[n];
                e[i] = 1.0;

                var column = GaussianPartialPivotSolver.Solve(a, e);

                for (int j = 0; j < n; j++)
                {
                    inverse[j, i] = column[j];
                }
            }

            return inverse;
        }
    }
}
