using AuthService.Application.Abstractions;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Seeders;

public class DatabaseSeeder
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly string _adminEmail;
    private readonly string _adminPassword;

    public DatabaseSeeder(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IDateTimeProvider dateTimeProvider,
        ILogger<DatabaseSeeder> logger,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _adminEmail = configuration.GetValue<string>("Seed:AdminEmail") ?? "admin@kinetra.local";
        _adminPassword = configuration.GetValue<string>("Seed:AdminPassword") ?? "Admin#1234";
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var admin = await _userRepository.FindByEmailAsync(_adminEmail, cancellationToken);
        if (admin is not null)
        {
            _logger.LogInformation("Admin user already exists with email {Email}", _adminEmail);
            return;
        }

        var now = _dateTimeProvider.UtcNow;
        var user = new User
        {
            Email = _adminEmail.Trim().ToLowerInvariant(),
            FullName = "Platform Administrator",
            Role = Role.Admin,
            CreatedAt = now
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, _adminPassword);

        await _userRepository.AddAsync(user, cancellationToken);
        _logger.LogInformation("Seeded admin user with email {Email}", _adminEmail);
    }
}
