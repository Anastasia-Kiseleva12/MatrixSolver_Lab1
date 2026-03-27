using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixSolver_Lab1.Models
{
    public class LinearSystem
    {
        public double[,] A { get; }
        public double[] B { get; }

        public int Size => B.Length;

        public LinearSystem(double[,] a, double[] b)
        {
            A = a;
            B = b;
        }
    }
}
