using SEB.DTOs;
using SEB.Models;

namespace SEB.Interfaces;

public interface IHistoryService
{
    public List<History> GetUserHistoryData(string token);
    public History LogPushups(string token, HistoryRequest historyRequest);
}