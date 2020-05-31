using System;
using System.ServiceProcess;
using ThePrinterSpyService.Core;

namespace ThePrinterSpyService
{
    public partial class Service : ServiceBase
    {
        readonly SpyOnSpool _printerSpy = new SpyOnSpool();
        public Service()
        {
            InitializeComponent();
        }

        internal void TestStartupAndStop(string[] args)
        {
            //debug method only
            this.OnStart(args);
            Console.ReadLine();
            this.OnStop();
        }

        protected override async void OnStart(string[] args)
        {
            await _printerSpy.RunAsync();
        }

        protected override void OnStop()
        {
            _printerSpy.Stop();
        }
    }
}
