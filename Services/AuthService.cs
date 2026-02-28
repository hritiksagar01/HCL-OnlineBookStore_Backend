using Online_BookStore__System.DTOs;
using Online_BookStore__System.Models;
using Online_BookStore__System.Repositories;

namespace Online_BookStore__System.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtService _jwtService;

    public AuthService(IUserRepository userRepository, JwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByUsernameAsync(dto.Username);
        if (existingUser != null) return null;

        var existingEmail = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingEmail != null) return null;

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "User"
        };

        user = await _userRepository.CreateAsync(user);

        var token = _jwtService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Role = user.Role,
            UserId = user.Id
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Username);
        if (user == null) return null;

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        var token = _jwtService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Role = user.Role,
            UserId = user.Id
        };
    }
}

