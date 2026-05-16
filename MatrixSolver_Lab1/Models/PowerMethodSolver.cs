using System;

namespace MatrixSolver_Lab1.Models
{
    public sealed class EigenResult
    {
        public EigenResult(double eigenValue, double[] eigenVector, int iterations)
        {
            EigenValue = eigenValue;
            EigenVector = eigenVector;
            Iterations = iterations;
        }

        public double EigenValue { get; }
        public double[] EigenVector { get; }
        public int Iterations { get; }
    }

    public static class PowerMethodSolver
    {
        public static EigenResult FindMaxByAbsEigenValue(
            double[,] a,
            double eps = 1e-10,
            int maxIterations = 100000)
        {
            int n = a.GetLength(0);

            var x = new double[n];

            for (int i = 0; i < n; i++)
                x[i] = 1.0;

            Normalize(x);

            double previousLambda = 0;

            for (int iteration = 1; iteration <= maxIterations; iteration++)
            {
                var y = Multiply(a, x);
                Normalize(y);

                double lambda = RayleighQuotient(a, y);

                if (Math.Abs(lambda - previousLambda) < eps)
                    return new EigenResult(lambda, y, iteration);

                x = y;
                previousLambda = lambda;
            }

            throw new InvalidOperationException("Степенной метод не сошелся.");
        }

        private static double[] Multiply(double[,] a, double[] x)
        {
            int n = x.Length;
            var result = new double[n];

            for (int i = 0; i < n; i++)
            {
                double sum = 0;

                for (int j = 0; j < n; j++)
                    sum += a[i, j] * x[j];

                result[i] = sum;
            }

            return result;
        }

        private static void Normalize(double[] x)
        {
            double norm = 0;

            for (int i = 0; i < x.Length; i++)
                norm += x[i] * x[i];

            norm = Math.Sqrt(norm);

            if (norm < 1e-15)
                throw new InvalidOperationException("Нулевой вектор нельзя нормировать.");

            for (int i = 0; i < x.Length; i++)
                x[i] /= norm;
        }

        private static double RayleighQuotient(double[,] a, double[] x)
        {
            var ax = Multiply(a, x);

            double numerator = 0;
            double denominator = 0;

            for (int i = 0; i < x.Length; i++)
            {
                numerator += ax[i] * x[i];
                denominator += x[i] * x[i];
            }

            return numerator / denominator;
        }
    }
}