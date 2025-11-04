using ChatApp.Application.DTOs.Common;
using ChatApp.Domain.Exceptions;

namespace ChatApp.Application.Queries.Users.GetUser;

public class GetUserHandler(IUserRepository userRepository)
    : IQueryHandler<GetUserQuery, AppResponse<UserDto>>
{
    public async Task<AppResponse<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken: cancellationToken);

            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                LastName = user.LastName ?? "",
                FirstName = user.FirstName ?? "",
            };

            return AppResponse<UserDto>.Success(userDto);
        }
        catch (NotFoundException)
        {
            return AppResponse<UserDto>.Fail("User not found");
        }
    }
}