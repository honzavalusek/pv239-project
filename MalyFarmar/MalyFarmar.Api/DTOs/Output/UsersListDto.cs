namespace MalyFarmar.Api.DTOs.Output;

public class UsersListDto
{
    public List<UserListViewDto> Users { get; set; } = new List<UserListViewDto>();
}