using System.Threading.Tasks;

namespace lab4.Interfaces
{
    public interface ISourceService
    {
        ValueTask<bool> SetDatabase();
    }
}