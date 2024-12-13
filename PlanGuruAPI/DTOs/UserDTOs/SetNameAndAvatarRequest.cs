namespace PlanGuruAPI.DTOs.UserDTOs
{
    public record SetNameAndAvatarRequest(Guid userId, string name, string avatar);
}
