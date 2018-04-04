using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prcm.OptimusRisk.WCFService.Utility
{
    public class MathUtility
    {
        public const double DaysPerYear = 365.2497;
        public const int DaysInTenYears = 3652;
        public const int DaysInFiftyYears = 18162;

        public static double[,] CholeskyDecomposition(double[,] input)
        {
            int size = input.GetLength(0);
            if (input.GetLength(1) != size)
                throw new Exception("Input matrix must be square");
            double[] p = new double[size];
            double[,] result = new double[size, size];
            Array.Copy(input, result, input.Length);
            for (int i = 0; i < size; i++)
            {
                for (int j = i; j < size; j++)
                {
                    double sum = result[i, j];
                    for (int k = i - 1; k >= 0; k--)
                        sum -= result[i, k] * result[j, k];
                    if (i == j)
                    {
                        if (sum < 0.0)
                            throw new Exception("Matrix is not positive definite");
                        p[i] = Math.Sqrt(sum);
                    }
                    else
                        result[j, i] = sum / p[i];
                }
            }
            for (int r = 0; r < size; r++)
            {
                result[r, r] = p[r];
                for (int c = r + 1; c < size; c++)
                    result[r, c] = 0;
            }
            return result;
        }

        public static double Correlation(double[] array1, double[] array2)
        {
            if (array1 != null && array2 != null && array1.Length == array2.Length && array1.Length > 1)
            {
                double[] array_xy = new double[array1.Length];
                double[] array_xp2 = new double[array1.Length];
                double[] array_yp2 = new double[array1.Length];
                for (int i = 0; i < array1.Length; i++)
                    array_xy[i] = array1[i] * array2[i];
                for (int i = 0; i < array1.Length; i++)
                    array_xp2[i] = Math.Pow(array1[i], 2.0);
                for (int i = 0; i < array1.Length; i++)
                    array_yp2[i] = Math.Pow(array2[i], 2.0);
                double sum_x = 0;
                double sum_y = 0;
                foreach (double n in array1)
                    sum_x += n;
                foreach (double n in array2)
                    sum_y += n;
                double sum_xy = 0;
                foreach (double n in array_xy)
                    sum_xy += n;
                double sum_xpow2 = 0;
                foreach (double n in array_xp2)
                    sum_xpow2 += n;
                double sum_ypow2 = 0;
                foreach (double n in array_yp2)
                    sum_ypow2 += n;
                double Ex2 = Math.Pow(sum_x, 2.00);
                double Ey2 = Math.Pow(sum_y, 2.00);

                double Correl =
                    (array1.Length * sum_xy - sum_x * sum_y) /
                    Math.Sqrt((array1.Length * sum_xpow2 - Ex2) * (array1.Length * sum_ypow2 - Ey2));

                return Correl;
            }

            return 0;
        }

        public static double StandardDeviation(double[] data)
        {
            double ret = 0;
            double DataAverage = 0;
            double TotalVariance = 0;
            int Max = 0;

            try
            {
                Max = data.Length;

                if (Max == 0)
                {
                    return ret;
                }

                DataAverage = Average(data);

                for (int i = 0; i < Max; i++)
                {
                    TotalVariance += Math.Pow(data[i] - DataAverage, 2);
                }

                ret = Math.Sqrt(SafeDivide(TotalVariance, Max));
            }
            catch (Exception)
            {
            }
            return ret;
        }

        private static double Average(double[] data)
        {
            double DataTotal = 0;

            try
            {
                for (int i = 0; i < data.Length; i++)
                {
                    DataTotal += data[i];
                }

                return SafeDivide(DataTotal, data.Length);
            }
            catch (Exception)
            {
            }
            return 0;
        }

        private static double SafeDivide(double value1, double value2)
        {
            double ret = 0;

            try
            {
                if ((value1 == 0) || (value2 == 0))
                {
                    return ret;
                }

                ret = value1 / value2;
            }
            catch
            {
            }
            return ret;
        }


        public static bool ArrayLinest(double[] arrayX, double[] arrayY, int steps, out double[] arrayK,
            out double[] arrayB)
        {
            if (arrayX != null && arrayY != null && arrayX.Count() == arrayY.Count() && arrayX.Count() > 0 && steps > 0)
            {
                try
                {
                    double[] arraySquareX = ArrayMultiple(arrayX, arrayX);
                    double[] arraySquareSumX = ArraySum(arraySquareX, steps); // K-a
                    double[] arraySumX = ArraySum(arrayX, steps); // B-c
                    double[] arraySumSquareAverageX = ArrayMultipleAverage(arraySumX, arraySumX, steps); // K-b
                    double[] arrayMulXY = ArrayMultiple(arrayX, arrayY);
                    double[] arrayMulSumXY = ArraySum(arrayMulXY, steps); // K-c
                    double[] arraySumY = ArraySum(arrayY, steps); // B-b
                    double[] arraySumMulAverageXY = ArrayMultipleAverage(arraySumX, arraySumY, steps); // K-d
                    double[] arrayCSubD = ArraySub(arrayMulSumXY, arraySumMulAverageXY);
                    double[] arrayASubB = ArraySub(arraySquareSumX, arraySumSquareAverageX);
                    arrayK = ArrayDiv(arrayCSubD, arrayASubB); // B-a
                    double[] arrayAMulC = ArrayMultiple(arrayK, arraySumX);
                    double[] arrayBSubAMulC = ArraySub(arraySumY, arrayAMulC);
                    arrayB = ArrayDiv(arrayBSubAMulC, steps);

                    return true;
                }
                catch (Exception)
                {
                }
            }

            arrayK = null;
            arrayB = null;

            return false;
        }

        public static double[] ArrayLn(double[] array)
        {
            if (array != null && array.Count() > 0)
            {
                List<double> lnList = new List<double>(array.Count());

                for (int i = 0; i < array.Count(); i++)
                {
                    lnList.Add(Math.Log(array[i]));
                }

                return lnList.ToArray();
            }

            return null;
        }

        private static double[] ArrayDiv(double[] array, double baseValue)
        {
            if (array != null && array.Count() > 0 && Math.Abs(baseValue) > 0)
            {
                List<double> divList = new List<double>(array.Count());

                for (int i = 0; i < array.Count(); i++)
                {
                    divList.Add(array[i] / baseValue);
                }

                return divList.ToArray();
            }

            return null;
        }

        private static double[] ArrayDiv(double[] arrayX, double[] arrayY)
        {
            if (arrayX != null && arrayY != null && arrayX.Count() > 0 && arrayX.Count() == arrayY.Count())
            {
                List<double> divList = new List<double>(arrayX.Count());

                for (int i = 0; i < arrayX.Count(); i++)
                {
                    if (Math.Abs(arrayY[i]) > 0.0000001)
                    {
                        divList.Add(arrayX[i] / arrayY[i]);
                    }
                    else
                    {
                        divList.Add(0);
                    }
                }

                return divList.ToArray();
            }

            return null;
        }

        private static double[] ArraySub(double[] arrayX, double[] arrayY)
        {
            if (arrayX != null && arrayY != null && arrayX.Count() > 0 && arrayX.Count() == arrayY.Count())
            {
                List<double> subList = new List<double>(arrayX.Count());

                for (int i = 0; i < arrayX.Count(); i++)
                {
                    subList.Add(arrayX[i] - arrayY[i]);
                }

                return subList.ToArray();
            }

            return null;
        }

        private static double[] ArrayMultiple(double[] arrayX, double[] arrayY)
        {
            if (arrayX != null && arrayY != null && arrayX.Count() > 0 && arrayX.Count() == arrayY.Count())
            {
                List<double> mulList = new List<double>(arrayX.Count());

                for (int i = 0; i < arrayX.Count(); i++)
                {
                    mulList.Add(arrayX[i] * arrayY[i]);
                }

                return mulList.ToArray();
            }

            return null;
        }

        private static double[] ArrayMultipleAverage(double[] arrayX, double[] arrayY, double baseValue)
        {
            if (arrayX != null && arrayY != null && arrayX.Count() > 0 && arrayX.Count() == arrayY.Count())
            {
                List<double> mulList = new List<double>(arrayX.Count());

                for (int i = 0; i < arrayX.Count(); i++)
                {
                    mulList.Add(arrayX[i] * arrayY[i] / baseValue);
                }

                return mulList.ToArray();
            }

            return null;
        }

        private static double[] ArrayAdd(double[] arrayA, double[] arrayB)
        {
            if (arrayA != null && arrayB != null && arrayA.Count() > 0 && arrayA.Count() == arrayB.Count())
            {
                List<double> addList = new List<double>(arrayA.Count());

                for (int i = 0; i < arrayA.Count(); i++)
                {
                    addList.Add(arrayA[i] + arrayB[i]);
                }

                return addList.ToArray();
            }

            return null;
        }

        private static double[] ArraySum(double[] array, int steps)
        {
            if (array != null && steps > 0)
            {
                List<double> sumList = new List<double>(array.Count());

                double sum = 0;

                for (int i = 0; i < array.Count(); i++)
                {
                    if (i < steps - 1)
                    {
                        sum += array[i];
                        sumList.Add(0);
                    }
                    else
                    {
                        sum += array[i];
                        sumList.Add(sum);
                        sum -= array[i - steps + 1];
                    }
                }

                return sumList.ToArray();
            }

            return null;
        }

        private static double[] ArrayAverage(double[] array, int steps)
        {
            if (array != null && steps > 0)
            {
                List<double> averageList = new List<double>(array.Count());

                double sum = 0;

                for (int i = 0; i < array.Count(); i++)
                {
                    if (i < steps - 1)
                    {
                        sum += array[i];
                        averageList.Add(0);
                    }
                    else
                    {
                        sum += array[i];
                        averageList.Add(sum / steps);
                        sum -= array[i - steps + 1];
                    }
                }

                return averageList.ToArray();
            }

            return null;
        }

        private static double Average(double[] array, int startIdx, int count)
        {
            if (count < 0)
            {
                startIdx = startIdx + count + 1;
                count = -count;

                return Average(array, startIdx, count);
            }

            if (startIdx < 0 && startIdx + count > 0)
            {
                startIdx = 0;
                count = startIdx + count;
            }

            if (startIdx >= 0 && count > 0)
            {
                int endIdx = startIdx + count - 1;
                double sum = 0;
                for (int i = startIdx; i <= endIdx; i++)
                {
                    sum += array[i];
                }

                return sum / count;
            }

            return 0;
        }

        private static double Squariance(double[] array, int startIdx, int count)
        {
            if (count < 0)
            {
                startIdx = startIdx + count + 1;
                count = -count;

                return Average(array, startIdx, count);
            }

            if (startIdx < 0 && startIdx + count > 0)
            {
                startIdx = 0;
                count = startIdx + count;
            }

            if (startIdx >= 0 && count > 0)
            {
                int endIdx = startIdx + count - 1;
                double sum = 0;
                for (int i = startIdx; i <= endIdx; i++)
                {
                    sum += array[i] * array[i];
                }

                return sum;
            }

            return 0;
        }

        public static Dictionary<double, double> Interpolation(double[] knownXs, double[] knownYs, double[] Xs)
        {
            if (knownXs == null || knownYs == null || Xs == null || knownXs.Count() != knownYs.Count() ||
                knownXs.Count() <= 1 || knownYs.Count() <= 1)
            {
                return null;
            }

            Dictionary<double, double> dictRetValues = new Dictionary<double, double>();

            foreach (double x in Xs)
            {
                if (x < knownXs[0])
                {
                    dictRetValues[x] = knownYs[0] + (knownYs[1] - knownYs[0]) * (x - knownXs[0]) / (knownXs[1] - knownXs[0]);
                    break;
                }
                else
                {
                    for (int i = 0; i < knownXs.Length - 1; i++)
                    {
                        if (x >= knownXs[i] && x < knownXs[i + 1])
                        {
                            dictRetValues[x] = knownYs[i] + (knownYs[i + 1] - knownYs[i]) * (x - knownXs[i]) / (knownXs[i + 1] - knownXs[i]);
                            break;
                        }
                    }

                    if (!dictRetValues.ContainsKey(x))
                    {
                        dictRetValues[x] = knownYs[knownYs.Length - 2] + (knownYs[knownYs.Length - 1] - knownYs[knownYs.Length - 2]) * (x - knownXs[knownYs.Length - 2]) / (knownXs[knownYs.Length - 1] - knownXs[knownYs.Length - 2]);
                    }
                }
            }

            return dictRetValues;
        }
    }
}
