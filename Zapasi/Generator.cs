using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapasi
{
    delegate double lambda_f(double t);
    static class Generator
    {
        /// <summary>
        /// Создаем однородную последовательность
        /// </summary>
        /// <param name="T">Максимальное время</param>
        /// <param name="lambda">параметр лямбда - частота</param>
        /// <returns></returns>
        public static List<double> CreateUniform(double T, double lambda)
        {
            var r = new Random(/*DateTime.Now.Millisecond*/);
            double t = 0;
            List<double> S = new List<double>();

            while (t < T)
            {
                var U = r.NextDouble();

                t -= Math.Log(U, 2) / lambda;
                if (t < T)
                    S.Add(t);
            }
            return S;
        }
        /// <summary>
        /// Создание неоднородной последовательности
        /// </summary>
        /// <param name="T">Время</param>
        /// <param name="lambda">частота</param>
        /// <param name="lambda_t">функция для генерации</param>
        /// <returns></returns>

        public static List<double> CreateNotUniform(double T, double lambda, lambda_f lambda_t)
        {
            var r = new Random(DateTime.Now.Millisecond);
            double t = 0;
            List<double> S = new List<double>();
            while (t < T)
            {
                var U1 = r.NextDouble();
                t -= 1 / lambda * Math.Log(U1, 2);
                if (t <= T)
                {
                    Random r2 = new Random(DateTime.Now.Millisecond);
                    var U2 = r2.NextDouble();
                    if (U2 < lambda_t(t) / lambda)
                        S.Add(t);
                }
            }
            return S;
        }

    }


}
