using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SurfaceLibrary
{
    public class Surface
    {
        protected double a { get; set; }
        protected double b { get; set; }
        protected double c { get; set; }
        protected double d { get; set; }

        public double XStart { get; set; }
        public double XEnd { get; set; }
        public double YStart { get; set; }
        public double YEnd { get; set; }

        public double Step { get; set; }

        public Surface(double a, double b, double c, double d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public void SetIntervals(double xStart, double xEnd, double yStart, double yEnd, double step)
        {
            if (step <= 0)
            {
                step = xEnd - xStart < yEnd - yStart ?
                                                    (xEnd - xStart) / 10 :
                                                    (yEnd - yStart) / 10;
            }
            if (xStart > xEnd)
            {
                Functions.Swap(ref xStart, ref xEnd);
            }
            if (yStart > yEnd)
            {
                Functions.Swap(ref yStart, ref yEnd);
            }

            XStart = xStart;
            XEnd = xEnd;
            YStart = yStart;
            YEnd = yEnd;
            Step = step;
        }

        /// <summary>
        ///     <para>Method for calculate area of surface</para>
        /// </summary>
        /// <param name="methodForCalculateIntegral">
        ///     <para>Numerical method for calc integral.</para>             
        ///     <para>Params: x1, x2, y1, y2, integrand_function</para>   
        /// </param>
        public double GetArea(Func<double, double, double, double, Func<double, double, double>, double> methodForCalculateIntegral, int maxCountThreads = 1000)
        {
            double x1 = XStart;
            double x2 = x1 + Step;
            int countThreads = 0;
            List<Task<double>> tasks = new List<Task<double>>();


            while (x1 < XEnd)
            {
                var t = new Task<double>((state) =>
                {
                    double localX1 = (state as (double, double)?).Value.Item1;
                    double localX2 = (state as (double, double)?).Value.Item2;

                    double s = 0;
                    double y1 = YStart;
                    double y2 = y1 + Step;



                    localX2 = localX2 > XEnd ? XEnd : localX2;
                    while (y1 < YEnd)
                    {
                        y2 = y2 > YEnd ? YEnd : y2;

                        s += methodForCalculateIntegral(localX1, localX2, y1, y2, F);

                        y1 += Step;
                        y2 += Step;
                    }


                    Interlocked.Decrement(ref countThreads);
                    return s;
                }, (x1, x2));


                // Simple spinlock
                while (countThreads >= maxCountThreads)
                {
                    Thread.Sleep(1);
                }
                countThreads++;

                t.Start();
                tasks.Add(t);

                x1 += Step;
                x2 += Step;
            }

            Task.WaitAll(tasks.ToArray());
            return tasks.Select(x => x.Result).Sum();
        }


        /// <summary>
        ///     <para>Return IEnumerable of Point. Point = { x, y, z }</para>
        /// </summary>
        public IEnumerable<Point> GetXYZ()
        {
            double xVal = XStart;
            List<Task<List<Point>>> tasks = new List<Task<List<Point>>>();

            while (xVal <= XEnd)
            {
                var t = new Task<List<Point>>((stateX) =>
                {
                    double localX = (double)stateX;
                    double yVal = YStart;
                    List<Point> localPoints = new List<Point>();

                    while (yVal < YEnd)
                    {
                        localPoints.Add(new Point { X = localX, Y = yVal, Z = Z(localX, yVal) });
                        yVal += Step;

                    }
                    localPoints.Add(new Point { X = localX, Y = yVal, Z = Z(localX, yVal) });

                    return localPoints;
                }, xVal);

                t.Start();
                tasks.Add(t);

                xVal += Step;
            }

            Task.WaitAll(tasks.ToArray());

            var points = tasks
                .Select(t => t.Result)
                .Aggregate((x, y) =>
                {
                    x.AddRange(y.AsEnumerable());
                    return x;
                })
                .OrderBy(p => p.X)
                .ThenBy(p => p.Y);

            return points;
        }


        protected virtual double F(double x, double y)
        {
            return Math.Sqrt(1 +
                            Math.Pow(3 * a * x * x + 2 * c * Math.Sin(x) * Math.Cos(x), 2) +
                            Math.Pow(3 * b * y * y - 2 * d * Math.Cos(y) * Math.Sin(y), 2));
        }


        /// <summary>
        ///     Rule for calculate coordinate Z by X,Y.
        /// </summary>
        protected virtual double Z(double x, double y)
        {
            return a * Math.Pow(x, 3) +
                    b * Math.Pow(y, 3) +
                    c * Math.Pow(Math.Sin(x), 2) +
                    d * Math.Pow(Math.Cos(y), 2);
        }


    }

}
