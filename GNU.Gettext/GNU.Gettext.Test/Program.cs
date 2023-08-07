using NUnit.Core;
using System;
using System.Reflection;

namespace GNU.Gettext.Test
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CoreExtensions.Host.InitializeService();
            TestPackage package = new TestPackage("Test");
            package.Assemblies.Add(Assembly.GetExecutingAssembly().Location);
            SimpleTestRunner runner = new SimpleTestRunner();
            if (runner.Load(package))
            {
                TestResult result = runner.Run(new NullListener(), TestFilter.Empty, true, LoggingThreshold.Debug);
                if (!result.IsSuccess)
                    throw new Exception(result.Message);
            }
        }
    }
}

