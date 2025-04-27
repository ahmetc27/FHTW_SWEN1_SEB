using SEB.Models;

namespace SEB.Interfaces;
public interface IHistoryRepository
{
    public List<History>? GetHistoryByUserId(int id);
    public History Add(int userid, History history);
}