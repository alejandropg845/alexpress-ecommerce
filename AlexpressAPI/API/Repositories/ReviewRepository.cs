using API.DbContexts;
using API.DTOs.ReviewDTO;
using API.Entities;
using API.Interfaces.Repositories;
using API.Responses;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ReviewDbContext _context;
        public ReviewRepository(ReviewDbContext context)
        {
            _context = context;
        }

        public void AddReviewsAsync(List<ReviewItem> reviewItems)
        { foreach (var reviewItem in reviewItems) _context.ReviewItems.Add(reviewItem); }
        
    }
}
