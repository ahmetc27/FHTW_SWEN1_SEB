using SEB.DTOs;
using SEB.Models;

namespace SEB.Interfaces;

public interface IHistoryService
{
    public History GetUserHistoryData(string token);
    public History LogPushups(string token, HistoryRequest historyRequest);
}