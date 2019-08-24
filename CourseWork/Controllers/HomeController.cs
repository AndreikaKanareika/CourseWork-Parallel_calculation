using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SurfaceLibrary;


namespace CourseWork.Controllers
{
    public class HomeController : Controller
    {
        Surface surface;
        Func<double, double, double, double, Func<double, double, double>, double> methodForCalculateIntegral;

        public HomeController(
            Surface surface, 
            Func<double, double, double, double, Func<double, double, double>, double> methodForCalculateIntegral)
        {
            this.surface = surface;
            this.methodForCalculateIntegral = methodForCalculateIntegral;
        }

        public IActionResult Index()
        {
            return View();
        }

           
        public IActionResult GetPoints(double xStart, double xEnd, double yStart, double yEnd, double step = 3)
        {

            surface.SetIntervals(xStart, xEnd, yStart, yEnd, step);
        
            return Json(new
            {
                status = true,
                result = surface.GetXYZ()
            });
        }
              
        public IActionResult GetArea(double xStart, double xEnd, double yStart, double yEnd, double step = 3)
        {
            List<(int maxCountThreads, long timeInMs, double areaResult)> results = new List<(int, long, double)>();
            Stopwatch stopwatch = new Stopwatch();
            
            surface.SetIntervals(xStart, xEnd, yStart, yEnd, step);
            
            for (int i = 1; i <= 10; i++)
            {
                stopwatch.Start();
                double s = surface.GetArea(methodForCalculateIntegral, i);
                stopwatch.Stop();
                
                results.Add((i, stopwatch.Elapsed.Milliseconds, s));
                stopwatch.Reset();
            }

            return Json(new
            {
                status = true,
                result = results.Select(_ => new
                {
                    _.maxCountThreads,
                    _.timeInMs,
                    _.areaResult
                })
            });
        }

    }
}
