using E_Commerce_Food_API.Models;

namespace E_Commerce_Food_API.DBinitializer
{
    public interface IInitialData
    {
        public List<MenuItem> GetInitialMenuItems();
        void InitialDb();
    }
}
