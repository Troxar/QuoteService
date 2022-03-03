using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QuoteService.Server
{
    public class FileQuoteRepository : IQuoteRepository
    {
        private readonly string filename;
        private List<string> quotes;
        private readonly Random random;

        public FileQuoteRepository(string filename)
        {
            this.filename = filename ?? throw new ArgumentNullException(nameof(filename));
            random = new Random(Guid.NewGuid().GetHashCode());
        }

        public ICollection<string> Quotes
        {
            get { return quotes; }
        }
        
        public void ReadQuotes()
        {
            if (File.Exists(filename))
            {
                quotes = File.ReadAllLines(filename).ToList();
            }
        }

        public string GetRandomQuote()
        {
            if (quotes == null || quotes.Count == 0)
            {
                return "No quotes";
            }

            int index = random.Next(0, quotes.Count);
            return quotes[index];
        }
    }
}
