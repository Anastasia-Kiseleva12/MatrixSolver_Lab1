using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace MatrixSolver_Lab1.Models
{
    public sealed class LibraryEigenResult
    {
        public LibraryEigenResult(double minEigenValue, double maxEigenValue, double[] eigenValues)
        {
            MinEigenValue = minEigenValue;
            MaxEigenValue = maxEigenValue;
            EigenValues = eigenValues;
        }

        public double MinEigenValue { get; }
        public double MaxEigenValue { get; }
        public double[] EigenValues { get; }
    }

    public static class MathNetEigenSolver
    {
        public static LibraryEigenResult FindEigenValues(double[,] inputA)
        {
            var matrix = Matrix<double>.Build.DenseOfArray(inputA);

            var evd = matrix.Evd();

            var eigenValues = evd.EigenValues
                .Select(value => value.Real)
                .OrderBy(value => value)
                .ToArray();

            double minByAbs = eigenValues
                .OrderBy(value => Math.Abs(value))
                .First();

            double maxByAbs = eigenValues
                .OrderByDescending(value => Math.Abs(value))
                .First();

            return new LibraryEigenResult(minByAbs, maxByAbs, eigenValues);
        }
    }
}