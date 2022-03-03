using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuoteService.Server;
using System;

namespace QuoteService.UnitTests
{
    [TestClass]
    public class QuoteServerTests
    {
        [TestMethod]
        public void Can_Start_And_Stop_Server()
        {
            IQuoteRepository quotes = new FileQuoteRepository("some_quotes.txt");
            QuoteServer target = new QuoteServer(quotes, 4567);

            try
            {
                target.Start();
                target.Stop();
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception, but got: {e.Message}");
            }
        }
    }
}