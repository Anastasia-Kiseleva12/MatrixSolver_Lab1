using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixSolver_Lab1.Models
{
    public static class PerturbationExperimentService
    {
        private static readonly Random _random = new();

        public static string RunExperiment(LinearSystem baseSystem)
        {
            var sb = new System.Text.StringBuilder();

            // Базовое решение берём устойчивым методом
            var baseSolution = GaussianPartialPivotSolver.Solve(baseSystem.A, baseSystem.B);

            double[] epsilons = { 1e-8, 1e-6, 1e-4, 1e-2 };

            sb.AppendLine("Эксперимент с возмущениями");
            sb.AppendLine();
            sb.AppendLine("Базовое решение:");
            for (int i = 0; i < baseSolution.Length; i++)
            {
                sb.AppendLine($"x{i + 1} = {baseSolution[i]:E6}");
            }

            sb.AppendLine();
            sb.AppendLine("Результаты экспериментов:");
            sb.AppendLine();

            for (int experimentIndex = 0; experimentIndex < epsilons.Length; experimentIndex++)
            {
                double epsilon = epsilons[experimentIndex];

                var perturbedA = PerturbMatrix(baseSystem.A, epsilon);
                var perturbedB = PerturbVector(baseSystem.B, epsilon);

                try
                {
                    var perturbedSolution = GaussianPartialPivotSolver.Solve(perturbedA, perturbedB);

                    var deltaA = SubtractMatrices(perturbedA, baseSystem.A);
                    var deltaB = SubtractVectors(perturbedB, baseSystem.B);
                    var deltaX = SubtractVectors(perturbedSolution, baseSolution);

                    double relativeA = MatrixInfinityNorm(deltaA) / MatrixInfinityNorm(baseSystem.A);
                    double relativeB = VectorInfinityNorm(deltaB) / VectorInfinityNorm(baseSystem.B);
                    double relativeX = VectorInfinityNorm(deltaX) / VectorInfinityNorm(baseSolution);

                    sb.AppendLine($"Опыт {experimentIndex + 1}, epsilon = {epsilon:E2}");
                    sb.AppendLine($"||ΔA||∞ / ||A||∞ = {relativeA:E6}");
                    sb.AppendLine($"||Δb||∞ / ||b||∞ = {relativeB:E6}");
                    sb.AppendLine($"||Δx||∞ / ||x||∞ = {relativeX:E6}");
                    sb.AppendLine();
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"Опыт {experimentIndex + 1}, epsilon = {epsilon:E2}");
                    sb.AppendLine($"Ошибка при решении возмущённой системы: {ex.Message}");
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        private static double[,] PerturbMatrix(double[,] matrix, double epsilon)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            var result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double noise = RandomSymmetricNoise(epsilon);
                    result[i, j] = matrix[i, j] + noise;
                }
            }

            return result;
        }

        private static double[] PerturbVector(double[] vector, double epsilon)
        {
            var result = new double[vector.Length];

            for (int i = 0; i < vector.Length; i++)
            {
                double noise = RandomSymmetricNoise(epsilon);
                result[i] = vector[i] + noise;
            }

            return result;
        }

        private static double RandomSymmetricNoise(double epsilon)
        {
            return (_random.NextDouble() * 2.0 - 1.0) * epsilon;
        }

        private static double[,] SubtractMatrices(double[,] a, double[,] b)
        {
            int rows = a.GetLength(0);
            int cols = a.GetLength(1);
            var result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = a[i, j] - b[i, j];
                }
            }

            return result;
        }

        private static double[] SubtractVectors(double[] a, double[] b)
        {
            var result = new double[a.Length];

            for (int i = 0; i < a.Length; i++)
            {
                result[i] = a[i] - b[i];
            }

            return result;
        }

        private static double MatrixInfinityNorm(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            double maxRowSum = 0.0;

            for (int i = 0; i < rows; i++)
            {
                double rowSum = 0.0;

                for (int j = 0; j < cols; j++)
                {
                    rowSum += Math.Abs(matrix[i, j]);
                }

                if (rowSum > maxRowSum)
                {
                    maxRowSum = rowSum;
                }
            }

            return maxRowSum;
        }

        private static double VectorInfinityNorm(double[] vector)
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
