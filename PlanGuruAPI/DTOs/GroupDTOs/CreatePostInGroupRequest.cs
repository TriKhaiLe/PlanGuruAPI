namespace PlanGuruAPI.DTOs.GroupDTOs
{
    public record CreatePostInGroupRequest(
        string Title,
        string Description,
        Guid UserId,
        Guid GroupId,
        string ImageUrl,
        string Tag,
        string Background);
}
