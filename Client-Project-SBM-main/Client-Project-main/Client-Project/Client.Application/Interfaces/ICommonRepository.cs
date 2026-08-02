using System.Data;
using System.Threading.Tasks;

namespace Client.Application.Interfaces
{
    public interface ICommonRepository
    {
        Task<DataSet> GetBackupDataAsync(int companyId);
    }
}
