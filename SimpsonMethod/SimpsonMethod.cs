using System;

namespace SimpsonMethodLibrary
{
    public class SimpsonMethod
    {
        public double Calculate(double xStart, double xEnd, double yStart, double yEnd, Func<double, double, double> f)
        {
            return (xEnd - xStart) / 6 * (yEnd - yStart) / 6 * (f(xStart, yStart) + 4 * f((xStart + xEnd) / 2, yStart) + f(xEnd, yStart) +
                                                                4 * (f(xStart, (yStart + yEnd) / 2) + 4 * f((xStart + xEnd) / 2, (yStart + yEnd) / 2) + f(xEnd, (yStart + yEnd) / 2)) +
                                                                f(xStart, yEnd) + 4 * f((xStart + xEnd) / 2, yEnd) + f(xEnd, yEnd));
        }
    }
}
