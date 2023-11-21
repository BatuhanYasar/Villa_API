using MagicVilla_API.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Villa> Villas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Villa>().HasData
                (
                  new Villa
                  {
                      Id = 1,
                      Name = "Royal Villa",
                      Details= "Deniz kıyısında harika manzaralı bir ev.",
                      ImageUrl = "https://cf.bstatic.com/xdata/images/hotel/max1024x768/225614958.jpg?k=e08cf4fc133be2c8acb6e5715547f622da895c5260a2d70bcc67ee359e13feb2&o=&hp=1",
                      Occupancy = 5,
                      Rate = 200,
                      Sqft = 550,
                      Amenity = "",
                      CreatedDate = DateTime.Now,
                  },

                  new Villa
                  {
                      Id = 2,
                      Name = "Candy Villa",
                      Details= "Şekerden yapılmış bir ev.",
                      ImageUrl = "https://static.dezeen.com/uploads/2016/07/house-over-water-corey-papadopoli-slide_dezeen_1568_6.jpg",
                      Occupancy = 3,
                      Rate = 300,
                      Sqft = 500,
                      Amenity = "",
                      CreatedDate = DateTime.Now,
                  },

                  new Villa
                  {
                      Id = 3,
                      Name = "Sunny Villa",
                      Details = "Güneş alan güzel bir ev.",
                      ImageUrl = "https://loveincorporated.blob.core.windows.net/contentimages/gallery/77da60a9-8492-47f5-b6f1-280017292bfa-12-incredible-homes-by-the-sea--british-virgin-islands.jpg",
                      Occupancy = 2,
                      Rate = 450,
                      Sqft = 700,
                      Amenity = "",
                      CreatedDate = DateTime.Now,
                  },

                  new Villa
                  {
                      Id = 4,
                      Name = "Diamond Villa",
                      Details = "Elmas gibi parlayan, çok şık, güzel bir ev.",
                      ImageUrl = "https://loveincorporated.blob.core.windows.net/contentimages/gallery/77da60a9-8492-47f5-b6f1-280017292bfa-12-incredible-homes-by-the-sea--british-virgin-islands.jpg",
                      Occupancy = 5,
                      Rate = 650,
                      Sqft = 900,
                      Amenity = "",
                      CreatedDate = DateTime.Now,
                  }

                );
        }
    }
}
