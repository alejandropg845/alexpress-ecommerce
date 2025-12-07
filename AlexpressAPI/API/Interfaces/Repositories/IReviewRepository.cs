using API.DTOs.ReviewDTO;
using API.Entities;
using API.Responses;

namespace API.Interfaces.Repositories
{
    public interface IReviewRepository
    {
        void AddReviewsAsync(List<ReviewItem> reviewItems);
    }
}
