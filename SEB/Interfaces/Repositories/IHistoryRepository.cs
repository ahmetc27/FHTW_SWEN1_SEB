using SEB.Models;

namespace SEB.Interfaces;
public interface IHistoryRepository
{
    public History? GetHistoryByUserId(int id);
}