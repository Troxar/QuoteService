using System.Collections.Generic;

namespace QuoteService.Server
{
    public interface IQuoteRepository
    {
        ICollection<string> Quotes { get; }
        void ReadQuotes();
        string GetRandomQuote();
    }
}
