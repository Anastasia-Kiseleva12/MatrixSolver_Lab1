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

    private static void SolveAndAppend(StringBuilder sb, string title, Func<double[]> solver)
    {
        sb.AppendLine(title);

        try
        {
            var solution = solver();

            for (int i = 0; i < solution.Length; i++)
            {
                sb.AppendLine($"x{i + 1} = {solution[i]:F6}");
            }
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
}