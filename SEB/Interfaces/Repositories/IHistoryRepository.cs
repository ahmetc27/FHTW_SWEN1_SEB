using SEB.Models;

namespace SEB.Interfaces;
public interface IHistoryRepository
{
    public History? GetHistoryByUserId(int id);
    public History Add(int userid, History history);
}