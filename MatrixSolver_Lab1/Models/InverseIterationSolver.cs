using System;

namespace MatrixSolver_Lab1.Models
{
    public static class InverseIterationSolver
    {
        public static EigenResult FindMinByAbsEigenValue(
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
                var y = GaussianPartialPivotSolver.Solve(a, x);

                Normalize(y);

                double lambda = RayleighQuotient(a, y);

                if (Math.Abs(lambda - previousLambda) < eps)
                    return new EigenResult(lambda, y, iteration);

                x = y;
                previousLambda = lambda;
            }

            throw new InvalidOperationException("Метод обратных итераций не сошелся.");
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
            int n = x.Length;

            double numerator = 0;
            double denominator = 0;

            for (int i = 0; i < n; i++)
            {
                double axI = 0;

                for (int j = 0; j < n; j++)
                    axI += a[i, j] * x[j];

                numerator += axI * x[i];
                denominator += x[i] * x[i];
            }

            return numerator / denominator;
        }
    }
}