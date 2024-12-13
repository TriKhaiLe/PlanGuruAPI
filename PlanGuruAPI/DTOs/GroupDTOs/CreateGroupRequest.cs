namespace PlanGuruAPI.DTOs.GroupDTOs
{
    public record CreateGroupRequest(string GroupName, Guid MasterUserId);
}
