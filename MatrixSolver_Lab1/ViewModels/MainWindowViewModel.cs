using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MatrixSolver_Lab1.Models;
using System;
using System.Text;

namespace MatrixSolver_Lab1.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string matrixText = "Матрица пока не сгенерирована.";

    [ObservableProperty]
    private string vectorText = "Вектор пока не сгенерирован.";

    [ObservableProperty]
    private string solutionText = "Решение пока не найдено.";

    private LinearSystem? _currentSystem;

    [RelayCommand]
    private void GenerateSystem()
    {
        _currentSystem = MatrixGenerator.Generate();

        MatrixText = MatrixGenerator.MatrixToString(_currentSystem.A);
        VectorText = MatrixGenerator.VectorToString(_currentSystem.B);
        SolutionText = "Система сгенерирована.";
    }

    [RelayCommand]
    private void GenerateBadSystem()
    {
        _currentSystem = MatrixGenerator.GenerateBadSystem();

        MatrixText = MatrixGenerator.MatrixToString(_currentSystem.A);
        VectorText = MatrixGenerator.VectorToString(_currentSystem.B);
        SolutionText = "Сгенерирована специальная плохо масштабированная система для дополнительного задания.";
    }

    [RelayCommand]
    private void RunPerturbationExperiment()
    {
        if (_currentSystem is null)
        {
            SolutionText = "Сначала сгенерируйте систему.";
            return;
        }

        try
        {
            SolutionText = PerturbationExperimentService.RunExperiment(_currentSystem);
        }
        catch (Exception ex)
        {
            SolutionText = $"Ошибка при выполнении эксперимента: {ex.Message}";
        }
    }

    [RelayCommand]
    private void SolveWithSimpleGauss()
    {
        if (_currentSystem is null)
        {
            SolutionText = "Сначала сгенерируйте систему.";
            return;
        }

        try
        {
            var solution = GaussianSolver.Solve(_currentSystem.A, _currentSystem.B);

            var sb = new StringBuilder();
            sb.AppendLine("Решение простым методом Гаусса:");
            sb.AppendLine();

            for (int i = 0; i < solution.Length; i++)
            {
                sb.AppendLine($"x{i + 1} = {solution[i]:F6}");
            }

            SolutionText = sb.ToString();
        }
        catch (Exception ex)
        {
            SolutionText = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    private void SolveWithPivotGauss()
    {
        if (_currentSystem is null)
        {
            SolutionText = "Сначала сгенерируйте систему.";
            return;
        }

        try
        {
            var solution = GaussianPartialPivotSolver.Solve(_currentSystem.A, _currentSystem.B);

            var sb = new StringBuilder();
            sb.AppendLine("Гаусс с выбором ведущего элемента по столбцу:");
            sb.AppendLine();

            for (int i = 0; i < solution.Length; i++)
            {
                sb.AppendLine($"x{i + 1} = {solution[i]:F6}");
            }

            SolutionText = sb.ToString();
        }
        catch (Exception ex)
        {
            SolutionText = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    private void SolveWithFullPivotGauss()
    {
        if (_currentSystem is null)
        {
            SolutionText = "Сначала сгенерируйте систему.";
            return;
        }

        try
        {
            var solution = GaussianFullPivotSolver.Solve(_currentSystem.A, _currentSystem.B);

            var sb = new StringBuilder();
            sb.AppendLine("Гаусс с выбором ведущего элемента по всей матрице:");
            sb.AppendLine();

            for (int i = 0; i < solution.Length; i++)
            {
                sb.AppendLine($"x{i + 1} = {solution[i]:F6}");
            }

            SolutionText = sb.ToString();
        }
        catch (Exception ex)
        {
            SolutionText = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    private void GenerateVeryBadSystem()
    {
        _currentSystem = MatrixGenerator.GenerateVeryBadSystem();

        MatrixText = MatrixGenerator.MatrixToString(_currentSystem.A);
        VectorText = MatrixGenerator.VectorToString(_currentSystem.B);
        SolutionText = "Сгенерирована ОЧЕНЬ плохо обусловленная система.";
    }

    [RelayCommand]
    private void SolveAll()
    {
        if (_currentSystem is null)
        {
            SolutionText = "Сначала сгенерируйте систему.";
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("Сравнение методов решения СЛАУ:");
        sb.AppendLine();

        SolveAndAppend(
            sb,
            "1. Простой метод Гаусса",
            () => GaussianSolver.Solve(_currentSystem.A, _currentSystem.B));

        SolveAndAppend(
            sb,
            "2. Метод Гаусса с выбором ведущего элемента по столбцу",
            () => GaussianPartialPivotSolver.Solve(_currentSystem.A, _currentSystem.B));

        SolveAndAppend(
            sb,
            "3. Метод Гаусса с выбором ведущего элемента по всей матрице",
            () => GaussianFullPivotSolver.Solve(_currentSystem.A, _currentSystem.B));
        SolveAndAppend(
            sb,
            "4. Решение через библиотеку MathNet",
            () => MathNetSolver.Solve(_currentSystem.A, _currentSystem.B));

        SolutionText = sb.ToString();
    }

    private void SolveAndAppend(StringBuilder sb, string title, Func<double[]> solver)
    {
        sb.AppendLine(title);

        try
        {
            var solution = solver();

            for (int i = 0; i < solution.Length; i++)
            {
                sb.AppendLine($"x{i + 1} = {solution[i]:F6}");
            }

            var residual = ResidualCalculator.Calculate(_currentSystem!.A, solution, _currentSystem.B);
            var residualNorm = ResidualCalculator.InfinityNorm(residual);

            sb.AppendLine($"||Ax - b||∞ = {residualNorm:E6}");
        }
        catch (Exception ex)
        {
            sb.AppendLine($"Ошибка: {ex.Message}");
        }

        sb.AppendLine();
    }

    [RelayCommand]
    private void ShowInverseAndConditionNumbers()
    {
        if (_currentSystem is null)
        {
            SolutionText = "Сначала сгенерируйте систему.";
            return;
        }

        try
        {
            var sb = new StringBuilder();

            var inverseCustom = InverseMatrixCalculator.Inverse(_currentSystem.A);
            var inverseMathNet = MathNetSolver.Inverse(_currentSystem.A);

            double cond1 = ConditionNumberCalculator.ConditionNumberOne(_currentSystem.A);
            double cond2 = ConditionNumberCalculator.ConditionNumberTwo(_currentSystem.A);
            double condInf = ConditionNumberCalculator.ConditionNumberInfinity(_currentSystem.A);

            sb.AppendLine("Обратная матрица (своя реализация):");
            sb.AppendLine();
            sb.AppendLine(MatrixGenerator.MatrixToString(inverseCustom));

            sb.AppendLine("Обратная матрица (MathNet):");
            sb.AppendLine();
            sb.AppendLine(MatrixGenerator.MatrixToString(inverseMathNet));

            sb.AppendLine("Числа обусловленности:");
            sb.AppendLine();
            sb.AppendLine($"cond₁(A)   = {cond1:E6}");
            sb.AppendLine($"cond₂(A)   = {cond2:E6}");
            sb.AppendLine($"cond∞(A)   = {condInf:E6}");

            SolutionText = sb.ToString();
        }
        catch (Exception ex)
        {
            SolutionText = $"Ошибка: {ex.Message}";
        }
    }

    private static void AppendIterativeResult(StringBuilder sb, string title, IterativeResult result)
    {
        sb.AppendLine(title);
        sb.AppendLine($"Количество итераций = {result.Iterations}");
        sb.AppendLine($"||Ax - b||∞ = {result.ResidualNorm:E6}");
        sb.AppendLine("Решение:");

        AppendVector(sb, result.Solution);

        sb.AppendLine();
    }

    private static void TryAppendIterativeMethod(
        StringBuilder sb,
        string title,
        Func<IterativeResult> solver)
    {
        sb.AppendLine(title);

        try
        {
            var result = solver();

            sb.AppendLine($"Количество итераций = {result.Iterations}");
            sb.AppendLine($"||Ax - b||∞ = {result.ResidualNorm:E6}");
            sb.AppendLine("Решение:");
            AppendVector(sb, result.Solution);
        }
        catch (Exception ex)
        {
            sb.AppendLine($"Ошибка: {ex.Message}");
        }

        sb.AppendLine();
    }

    private void TryAppendDirectMethod(
    StringBuilder sb,
    string title,
    Func<double[]> solver)
    {
        sb.AppendLine(title);

        try
        {
            var solution = solver();

            var residual = ResidualCalculator.Calculate(_currentSystem!.A, solution, _currentSystem.B);
            var residualNorm = ResidualCalculator.InfinityNorm(residual);

            sb.AppendLine($"||Ax - b||∞ = {residualNorm:E6}");
            sb.AppendLine("Решение:");
            AppendVector(sb, solution);
        }
        catch (Exception ex)
        {
            sb.AppendLine($"Ошибка: {ex.Message}");
        }

        sb.AppendLine();
    }

    private static void AppendVector(StringBuilder sb, double[] vector)
    {
        for (int i = 0; i < vector.Length; i++)
        {
            sb.AppendLine($"x{i + 1} = {vector[i]:E12}");
        }
    }

    [RelayCommand]
    private void GenerateSymmetricSystem()
    {
        _currentSystem = MatrixGenerator.GenerateSymmetric();

        MatrixText = MatrixGenerator.MatrixToString(_currentSystem.A);
        VectorText = MatrixGenerator.VectorToString(_currentSystem.B);
        SolutionText = "Сгенерирована симметричная матрица A и вектор b.";
    }

    [RelayCommand]
    private void SolveWithJacobi()
    {
        if (_currentSystem is null)
        {
            SolutionText = "Сначала сгенерируйте систему.";
            return;
        }

        try
        {
            var result1 = JacobiSolver.Solve(_currentSystem.A, _currentSystem.B, 1e-4);
            var result2 = JacobiSolver.Solve(_currentSystem.A, _currentSystem.B, 1e-12);

            var sb = new StringBuilder();

            sb.AppendLine("Метод Якоби");
            sb.AppendLine();

            AppendIterativeResult(sb, "eps = 1e-4", result1);
            AppendIterativeResult(sb, "eps = 1e-12", result2);

            SolutionText = sb.ToString();
        }
        catch (Exception ex)
        {
            SolutionText = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    private void SolveWithSeidel()
    {
        if (_currentSystem is null)
        {
            SolutionText = "Сначала сгенерируйте систему.";
            return;
        }

        try
        {
            var result1 = SeidelSolver.Solve(_currentSystem.A, _currentSystem.B, 1e-4);
            var result2 = SeidelSolver.Solve(_currentSystem.A, _currentSystem.B, 1e-12);

            var sb = new StringBuilder();

            sb.AppendLine("Метод Зейделя");
            sb.AppendLine();

            AppendIterativeResult(sb, "eps = 1e-4", result1);
            AppendIterativeResult(sb, "eps = 1e-12", result2);

            SolutionText = sb.ToString();
        }
        catch (Exception ex)
        {
            SolutionText = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    private void SolveIterativeMethods()
    {
        if (_currentSystem is null)
        {
            SolutionText = "Сначала сгенерируйте систему.";
            return;
        }

        var sb = new StringBuilder();

        sb.AppendLine("Сравнение итерационных методов");
        sb.AppendLine();

        TryAppendIterativeMethod(
            sb,
            "Метод Якоби, eps = 1e-4",
            () => JacobiSolver.Solve(_currentSystem.A, _currentSystem.B, 1e-4));

        TryAppendIterativeMethod(
            sb,
            "Метод Якоби, eps = 1e-12",
            () => JacobiSolver.Solve(_currentSystem.A, _currentSystem.B, 1e-12));

        TryAppendIterativeMethod(
            sb,
            "Метод Зейделя, eps = 1e-4",
            () => SeidelSolver.Solve(_currentSystem.A, _currentSystem.B, 1e-4));

        TryAppendIterativeMethod(
            sb,
            "Метод Зейделя, eps = 1e-12",
            () => SeidelSolver.Solve(_currentSystem.A, _currentSystem.B, 1e-12));

        TryAppendDirectMethod(
            sb,
            "Решение через библиотеку MathNet",
            () => MathNetSolver.Solve(_currentSystem.A, _currentSystem.B));

        SolutionText = sb.ToString();
    }

    [RelayCommand]
    private void SolveSpectralProblem()
    {
        if (_currentSystem is null)
        {
            SolutionText = "Сначала сгенерируйте систему.";
            return;
        }

        try
        {
            var maxEigen = PowerMethodSolver.FindMaxByAbsEigenValue(_currentSystem.A);
            var minEigen = InverseIterationSolver.FindMinByAbsEigenValue(_currentSystem.A);

            var libraryEigen = MathNetEigenSolver.FindEigenValues(_currentSystem.A);

            double customConditionNumber =
                Math.Abs(maxEigen.EigenValue) / Math.Abs(minEigen.EigenValue);

            double libraryConditionNumber =
                Math.Abs(libraryEigen.MaxEigenValue) / Math.Abs(libraryEigen.MinEigenValue);

            var sb = new StringBuilder();

            sb.AppendLine("Спектральная задача");
            sb.AppendLine();

            sb.AppendLine("1. Собственная реализация");
            sb.AppendLine();

            sb.AppendLine("Наибольшее по модулю собственное число:");
            sb.AppendLine($"lambda_max = {maxEigen.EigenValue:E12}");
            sb.AppendLine($"Количество итераций = {maxEigen.Iterations}");
            sb.AppendLine("Собственный вектор:");
            AppendVector(sb, maxEigen.EigenVector);
            sb.AppendLine();

            sb.AppendLine("Наименьшее по модулю собственное число:");
            sb.AppendLine($"lambda_min = {minEigen.EigenValue:E12}");
            sb.AppendLine($"Количество итераций = {minEigen.Iterations}");
            sb.AppendLine("Собственный вектор:");
            AppendVector(sb, minEigen.EigenVector);
            sb.AppendLine();

            sb.AppendLine("2. Решение с помощью библиотеки MathNet");
            sb.AppendLine();

            sb.AppendLine("Все собственные числа:");
            for (int i = 0; i < libraryEigen.EigenValues.Length; i++)
            {
                sb.AppendLine($"lambda_{i + 1} = {libraryEigen.EigenValues[i]:E12}");
            }

            sb.AppendLine();

            sb.AppendLine($"lambda_max(MathNet) = {libraryEigen.MaxEigenValue:E12}");
            sb.AppendLine($"lambda_min(MathNet) = {libraryEigen.MinEigenValue:E12}");
            sb.AppendLine();

            sb.AppendLine("3. Сравнение результатов");
            sb.AppendLine();

            sb.AppendLine($"lambda_max, степенной метод      = {maxEigen.EigenValue:E12}");
            sb.AppendLine($"lambda_max, MathNet              = {libraryEigen.MaxEigenValue:E12}");
            sb.AppendLine($"Погрешность lambda_max           = {Math.Abs(maxEigen.EigenValue - libraryEigen.MaxEigenValue):E12}");
            sb.AppendLine();

            sb.AppendLine($"lambda_min, обратные итерации    = {minEigen.EigenValue:E12}");
            sb.AppendLine($"lambda_min, MathNet              = {libraryEigen.MinEigenValue:E12}");
            sb.AppendLine($"Погрешность lambda_min           = {Math.Abs(minEigen.EigenValue - libraryEigen.MinEigenValue):E12}");
            sb.AppendLine();

            sb.AppendLine("4. Число обусловленности");
            sb.AppendLine();

            sb.AppendLine($"cond(A), собственная реализация  = {customConditionNumber:E12}");
            sb.AppendLine($"cond(A), MathNet                 = {libraryConditionNumber:E12}");
            sb.AppendLine($"Погрешность cond(A)              = {Math.Abs(customConditionNumber - libraryConditionNumber):E12}");

            SolutionText = sb.ToString();
        }
        catch (Exception ex)
        {
            SolutionText = $"Ошибка: {ex.Message}";
        }
    }
}