// See https://aka.ms/new-console-template for more information

using EntityFramework;
using EntityFramework.Models;

Console.WriteLine("Hello, World!");


using var context = new FrameworkPerformanceMtContext();

var test = new SingleTableTests(context);
test.Run();