using Microsoft.EntityFrameworkCore;
using TaskManage.Models;
using TaskManage.Repositories;
using TaskManage.Repositories.Interfaces;

namespace projectver2
{
    public class services
    {

        private readonly AppDbContext _context;

        public services(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer("Data Source=Localhost; Initial Catalog=Datab; Integrated Security=true; TrustServerCertificate=True"));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

        }
    }
}