using SEB.Models;

namespace SEB.Interfaces;

public interface IHistoryService
{
    public History GetUserHistoryData(string token);
}