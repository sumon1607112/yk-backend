using AutoMapper;
using MediatR;
using YK.Application.Abstractions.Persistence;
using YK.Domain.Entities.Users;

namespace YK.Application.UseCases.Users
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UpdateUserHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserResponseDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            User updatedUser;

            if (request.UserData.Id == 0)
            {
                var newUserEntity = _mapper.Map<User>(request.UserData);
                updatedUser = await _userRepository.AddAsync(newUserEntity);
            }
            else
            {
                var existingUser = await _userRepository.GetByIdAsync((long)request.UserData.Id);
                if (existingUser == null)
                    throw new Exception($"User with ID {request.UserData.Id} not found.");

                _mapper.Map(request.UserData, existingUser);
                updatedUser = await _userRepository.UpdateAsync(existingUser);
            }

            return _mapper.Map<UserResponseDto>(updatedUser);
        }

    }
}
