using E_Commerce_Food_API.Data;
using E_Commerce_Food_API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Food_API.DBinitializer
{
    public class InitialData : IInitialData
    {
        private readonly IWebHostEnvironment _environment;
        private readonly AppDbContext _db;
        public InitialData(IWebHostEnvironment environment , AppDbContext db)
        {
            _environment = environment;
            _db = db;
        }
        public List<MenuItem> GetInitialMenuItems()
        {
            var InitialImages = new InitialImage(_environment).GetInitialImages2();
            return new List<MenuItem>
            {
                new MenuItem
                {
                   
                    Name = "Spring Roll",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    Image= InitialImages[0],
                    Price = 7.99,
                    Category = "Appetizer",
                    SpecialTag = ""
                }, new MenuItem
                {
                    
                    Name = "Idli",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    Image= InitialImages[1],
                    Price = 8.99,
                    Category = "Appetizer",
                    SpecialTag = ""
                }, new MenuItem
                {
                    
                    Name = "Panu Puri",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    Image=InitialImages[2],
                    Price = 8.99,
                    Category = "Appetizer",
                    SpecialTag = "Best Seller"
                }, new MenuItem
                {
                   
                    Name = "Hakka Noodles",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    Image= InitialImages[3],
                    Price = 10.99,
                    Category = "Entrée",
                    SpecialTag = ""
                }, new MenuItem
                {
                    Name = "Malai Kofta",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    Image= InitialImages[4],
                    Price = 12.99,
                    Category = "Entrée",
                    SpecialTag = "Top Rated"
                }, new MenuItem
                {
                    
                    Name = "Paneer Pizza",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    Image= InitialImages[5],
                    Price = 11.99,
                    Category = "Entrée",
                    SpecialTag = ""
                }, new MenuItem
                {
                
                    Name = "Paneer Tikka",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    Image= InitialImages[6],
                    Price = 13.99,
                    Category = "Entrée",
                    SpecialTag = "Chef's Special"
                }, new MenuItem
                {
                   
                    Name = "Carrot Love",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    Image= InitialImages[7],
                    Price = 4.99,
                    Category = "Dessert",
                    SpecialTag = ""
                }, new MenuItem
                {
                    Name = "Rasmalai",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    Image= InitialImages[8],
                    Price = 4.99,
                    Category = "Dessert",
                    SpecialTag = "Chef's Special"
                }, new MenuItem
                {
                    Name = "Sweet Rolls",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    Image= InitialImages[9],
                    Price = 3.99,
                    Category = "Dessert",
                    SpecialTag = "Top Rated"
                }
            };
        }
        public void InitialDb()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }
            if(_db.MenuItems.Any() == false)
            {
                _db.MenuItems.AddRange(GetInitialMenuItems());
                _db.SaveChanges();
            }
        }
    }
}
