using Microsoft.EntityFrameworkCore;


namespace testwebapi.DataLayer
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
            dbContextOptions = options;
        }

        private readonly DbContextOptions<MyDbContext> dbContextOptions;

    }
}
