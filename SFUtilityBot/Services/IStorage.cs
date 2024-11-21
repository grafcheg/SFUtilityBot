using SFUtilityBot.Models;

namespace SFUtilityBot.Services;

public interface IStorage
{
    Session GetSession(long chatId);
}