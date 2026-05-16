using System;
using System.Text;

namespace MatrixSolver_Lab1.Models
{
    public sealed class IterativeResult
    {
        public IterativeResult(double[] solution, int iterations, double residualNorm)
        {
            Solution = solution;
            Iterations = iterations;
            ResidualNorm = residualNorm;
        }

        public double[] Solution { get; }
        public int Iterations { get; }
        public double ResidualNorm { get; }
    }

    public static class JacobiSolver
    {
        public static IterativeResult Solve(
            double[,] a,
            double[] b,
            double eps,
            int maxIterations = 100000)
        {
            int n = b.Length;

            var x = new double[n];
            var xNew = new double[n];

            for (int iteration = 1; iteration <= maxIterations; iteration++)
            {
                for (int i = 0; i < n; i++)
                {
                    double sum = 0;

                    for (int j = 0; j < n; j++)
                    {
                        if (j != i)
                            sum += a[i, j] * x[j];
                    }

                    if (Math.Abs(a[i, i]) < 1e-15)
                        throw new InvalidOperationException("На диагонали найден нулевой элемент.");

                    xNew[i] = (b[i] - sum) / a[i, i];
                }

                double diffNorm = InfinityNormDifference(xNew, x);

                Array.Copy(xNew, x, n);

                var residual = ResidualCalculator.Calculate(a, x, b);
                double residualNorm = ResidualCalculator.InfinityNorm(residual);

                if (diffNorm < eps || residualNorm < eps)
                    return new IterativeResult(x, iteration, residualNorm);
            }

            throw new InvalidOperationException("Метод Якоби не сошелся за заданное число итераций.");
        }

        private static double InfinityNormDifference(double[] x, double[] y)
        {
            double max = 0;

            for (int i = 0; i < x.Length; i++)
            {
                max = Math.Max(max, Math.Abs(x[i] - y[i]));
            }

            return max;
        }
    }
}