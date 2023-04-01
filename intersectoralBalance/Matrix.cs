using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intersectoralBalance
{
    static class Matrix
    {



        static double GetDeter2N(double[,] matr)
        {
            if (matr.GetLength(0) == 2 && 2 == matr.GetLength(1))
                return matr[0, 0] * matr[1, 1] - matr[1, 0] * matr[0, 1];
            else
            {
                throw new Exception("Размерность матрицы должна быть 2 на 2");
            }
        }


        public static double GetDeter(double[,] matr)
        {
            double sum = 0;

            if (matr.GetLength(0) == 2)
                return GetDeter2N(matr);
            for (int i = 0; i < matr.GetLength(0); i++)
            {
                var minor = GetMinor(matr, 0, i);

                if (i % 2 == 0)
                {
                    sum += matr[0, i] * GetDeter(minor);
                } else
                    sum += -matr[0, i] * GetDeter(minor);

            }
            return sum;
        }

        public static double[,] GetMinor(double[,] matrix, int rowNum, int colNum)
        {
            double[,] minor = new double[matrix.GetLength(0) - 1, matrix.GetLength(0) - 1];
            int qurI = 0;
            int qurJ = 0;


            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (i != rowNum)
                {
                    qurJ = 0;
                    for (int j = 0; j < matrix.GetLength(0); j++)
                        if (j != colNum)
                        {
                            minor[qurI, qurJ] = matrix[i, j];
                            qurJ++;
                        }

                    qurI++;
                }
            }

            return minor;
        }

        public static double GetAlgebraicComplement(double[,] matr, int rowNum, int colNum) {
            double det = GetDeter(GetMinor(matr, rowNum, colNum));
            double t = (int)Math.Pow(-1, rowNum + colNum) * det;
            return t;
        }

        public static double[,] GetReversMatrix (double[,] matr)
        {
            double[,] vs = new double[matr.GetLength(0), matr.GetLength(0)];

            double D = GetDeter(matr);
            if (D == 0)
                throw new Exception("Нет обратной матрицы т.к. det = 0");

            for (int i = 0; i < matr.GetLength(0); i++)
                for (int j = 0; j < matr.GetLength(0); j++)
                {
                    double a = GetAlgebraicComplement(matr, i, j);
                    double t =  a/ D;
                    vs[j, i] = t;
                }

            return vs;
        }
    }
}
