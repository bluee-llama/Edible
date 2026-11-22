using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Edible.Application.Abstractions;
using Edible.Infrastructure.Data;

namespace Edible.Infrastructure;

public sealed class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly DbContextOptions<EdibleDbContext> _options;
    private readonly ILoggerFactory? _loggerFactory;
    private readonly IHariaPasswordHasher? _passwordHasher;
    private readonly IHariaTokenGenerator? _tokenGenerator;

    public UnitOfWorkFactory(DbContextOptions<EdibleDbContext> options, ILoggerFactory? loggerFactory = null, IHariaPasswordHasher? passwordHasher = null, IHariaTokenGenerator? tokenGenerator = null)
    {
        _options = options;
        _loggerFactory = loggerFactory;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    internal ILoggerFactory? LoggerFactory => _loggerFactory;

    internal IHariaPasswordHasher? PasswordHasher => _passwordHasher;

    internal IHariaTokenGenerator? TokenGenerator => _tokenGenerator;

    internal EdibleDbContext CreateDbContext() =>
        new(_options);

    public IUnitOfWork Create() =>
        new UnitOfWork(this);
}