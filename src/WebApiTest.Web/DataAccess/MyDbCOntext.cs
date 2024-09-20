using Microsoft.EntityFrameworkCore;
using WebApiTest.Web.Entitys;

namespace WebApiTest.Web.DataAccess;

public class MyDbCOntext : DbContext
{
    public MyDbCOntext(DbContextOptions<MyDbCOntext> options) : base(options)
    {}
    DbSet<User> User {  get; set; }
}
