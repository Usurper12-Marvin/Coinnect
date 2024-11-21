using static Coinnect.Models.AllModels;

namespace Coinnect.Data
{
    public interface IUserRepository
    {
        Task<User> GetById(string id);
        Task DeleteA(string id);
    }
}
