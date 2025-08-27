using Microsoft.EntityFrameworkCore;
using TaskManage.Models;
using TaskManage.Repositories.Interfaces;

namespace TaskManage.Repositories {
    public class CategoryRepository : ICategoryRepository
    {

        private readonly AppDbContext _context;
    
        public async Task<Category> AddCategoryAsync(int userId, Category category)
        {
            category.UserId = userId;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<List<Category>> GetCategoriesByUserIdAsync(int userId)
        {
            return await _context.Categories
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int userId, int categoryId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == categoryId);
        }

        public async Task<Category?> GetCategoryByNameAsync(int userId, string categoryName)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Name.ToLower() == categoryName.ToLower());
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }

}

