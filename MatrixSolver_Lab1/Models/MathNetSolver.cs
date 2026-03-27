using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace MatrixSolver_Lab1.Models
{
    public static class MathNetSolver
    {
        public static double[] Solve(double[,] a, double[] b)
        {
            var matrix = Matrix<double>.Build.DenseOfArray(a);
            var vector = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(b);

            var result = matrix.Solve(vector);

            return result.ToArray();
        }

        public static double[,] Inverse(double[,] a)
        {
            var matrix = Matrix<double>.Build.DenseOfArray(a);

            return matrix.Inverse().ToArray();
        }
    }
}
