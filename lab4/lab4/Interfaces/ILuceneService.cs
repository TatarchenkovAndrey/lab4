using System.Collections.Generic;
using System.Threading.Tasks;

namespace lab4.Interfaces
{
    public interface ILuceneService
    {
        ValueTask<IEnumerable<string>> Search(string text, bool viaLucene);
    }
}