using System.Collections.Generic;
using Common.Models;

namespace Common.Interfaces
{
    public interface ISearchService
    {
        IEnumerable<PropertyMnmgt> GetData(string searchTerm, string market, int size);
    }
}