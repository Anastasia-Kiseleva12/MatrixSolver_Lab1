using MathNet.Numerics.LinearAlgebra;

namespace MatrixSolver_Lab1.Models
{
    public static class MathNetInverseCalculator
    {
        public static double[,] Inverse(double[,] a)
        {
            var matrix = Matrix<double>.Build.DenseOfArray(a);
            return matrix.Inverse().ToArray();
        }
    }
}
