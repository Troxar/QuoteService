using System.ServiceProcess;

namespace QuoteService.Service
{
    static class Program
    {
        static void Main()
        {
            ServiceBase.Run(new RandomQuoteService());
        }
    }
}
