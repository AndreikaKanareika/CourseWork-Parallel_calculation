using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseWork.Models
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

        public void SetIntervals(double xStart, double xEnd, double yStart, double yEnd)
        {
            XStart = xStart;
            XEnd = xEnd;
            YStart = yStart;
            YEnd = yEnd;
        }

        public double GetS()
        {
            double s = 0;
            double x1 = XStart;
            double x2 = x1 + Step;

            while (x1 < XEnd)
            {
                double y1 = YStart;
                double y2 = y1 + Step;

                while (y1 < YEnd)
                {
                    x2 = x2 > XEnd ? XEnd : x2;
                    y2 = y2 > YEnd ? YEnd : y2;

                    s +=
                        (x2 - x1) / 6 * (y2 - y1) / 6 * (F(x1, y1) + 4 * F((x1 + x2) / 2, y1) + F(x2, y1) +
                        4 * (F(x1, (y1 + y2) / 2) + 4 * F((x1 + x2) / 2, (y1 + y2) / 2) + F(x2, (y1 + y2) / 2)) +
                        F(x1, y2) + 4 * F((x1 + x2) / 2, y2) + F(x2, y2));

                    y1 += Step;
                    y2 += Step;
                }

                x1 += Step;
                x2 += Step;
            }
            return s;
        }

        public IEnumerable<Point> GetXYZ()
        {
            double xVal = XStart;
            List<Task<List<Point>>> tasks = new List<Task<List<Point>>>();

            while (xVal <= XEnd)
            {
                var t = new Task<List<Point>>(() =>
                {
                    double localX = xVal;
                    double yVal = YStart;
                    List<Point> localPoints = new List<Point>();

                    while (yVal < YEnd)
                    {
                        localPoints.Add(new Point { X = localX, Y = yVal, Z = Z(localX, yVal) });
                        yVal += Step;

                    }
                    localPoints.Add(new Point { X = localX, Y = yVal, Z = Z(localX, yVal) });

                    return localPoints;
                });

                tasks.Add(t);


                xVal += Step;
                t.Start();
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


        public virtual double F(double x, double y)
        {
            return Math.Sqrt(1 +
                            Math.Pow(3 * a * x * x + 2 * c * Math.Sin(x) * Math.Cos(x), 2) +
                            Math.Pow(3 * b * y * y - 2 * d * Math.Cos(y) * Math.Sin(y), 2));
        }

        public virtual double Z(double x, double y)
        {
            return a * Math.Pow(x, 3) +
                    b * Math.Pow(y, 3) +
                    c * Math.Pow(Math.Sin(x), 2) +
                    d * Math.Pow(Math.Cos(y), 2);
        }

    }
}
