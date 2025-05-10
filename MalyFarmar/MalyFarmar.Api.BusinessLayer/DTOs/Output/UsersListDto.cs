namespace MalyFarmar.Api.BusinessLayer.DTOs.Output;

public class UsersListDto
{
    public List<UserListViewDto> Users { get; set; } = new List<UserListViewDto>();
}