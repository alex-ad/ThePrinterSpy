using System;
using System.ServiceProcess;

namespace ThePrinterSpyService
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main(string[] args)
        {
            //debug begin
            if (Environment.UserInteractive)
            {
                Service service1 = new Service();
                service1.TestStartupAndStop(args);
            }
            else
            {
                ServiceBase[] servicesToRun = { new Service() };
                ServiceBase.Run(servicesToRun);
            }
            //debug end

            /*ServiceBase[] servicesToRun = { new Service() };
            ServiceBase.Run(servicesToRun);*/
        }
    }
}
