// config mapping profile for GroupMember to GroupMemberDto
using ChatApp.Application.DTOs.Common;
namespace ChatApp.Application.Mappings;

public class GroupMemberMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<GroupMember, GroupMemberDto>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.UserName, src => src.User.UserName)
            .Map(dest => dest.Email, src => src.User.Email)
            .Map(dest => dest.JoinedAt, src => src.JoinedAt)
            .Map(dest => dest.IsAdmin, src => src.IsAdmin);
    }
}