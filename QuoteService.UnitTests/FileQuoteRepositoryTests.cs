using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuoteService.Server;
using System;

namespace QuoteService.UnitTests
{
    [TestClass]
    public class FileQuoteRepositoryTests
    {
        [TestMethod]
        public void Can_Read_Quotes_When_File_Does_Not_Exist()
        {
            IQuoteRepository target = new FileQuoteRepository("no_such_file.txt");

            try
            {
                target.ReadQuotes();
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception, but got: {e.Message}");
            }
        }

        [TestMethod]
        public void Can_Get_Random_Quote_When_File_Does_Not_Exist()
        {
            IQuoteRepository target = new FileQuoteRepository("no_such_file.txt");
            target.ReadQuotes();

            string result = target.GetRandomQuote();

            Assert.AreEqual(result, "No quotes");
        }
    }
}
