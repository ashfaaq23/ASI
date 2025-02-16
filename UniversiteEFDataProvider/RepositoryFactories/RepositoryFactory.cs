using Microsoft.AspNetCore.Identity;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Repositories;
using UniversiteEFDataProvider.Entities;

namespace UniversiteEFDataProvider.RepositoryFactories;

public class RepositoryFactory : IRepositoryFactory
{
    private readonly UniversiteDbContext _context;
    private readonly UserManager<UniversiteUser> _userManager;
    private readonly RoleManager<UniversiteRole> _roleManager;

    private IParcoursRepository? _parcours;
    private IEtudiantRepository? _etudiants;
    private IUeRepository? _ues;
    private INoteRepository? _notes;
    private IUniversiteRoleRepository? _universiterole;
    private IUniversiteUserRepository? _universiteuser;

    public RepositoryFactory(UniversiteDbContext context, UserManager<UniversiteUser> userManager, RoleManager<UniversiteRole> roleManager)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
    }

    public IParcoursRepository ParcoursRepository()
    {
        return _parcours ??= new ParcoursRepository(_context);
    }

    public IEtudiantRepository EtudiantRepository()
    {
        return _etudiants ??= new EtudiantRepository(_context);
    }

    public IUeRepository UeRepository()
    {
        return _ues ??= new UeRepository(_context);
    }

    public INoteRepository NoteRepository()
    {
        return _notes ??= new NoteRepository(_context);
    }

    public IUniversiteRoleRepository UniversiteRoleRepository()
    {
        return _universiterole ??= new UniversiteRoleRepository(_context, _roleManager);
    }

    public IUniversiteUserRepository UniversiteUserRepository()
    {
        return _universiteuser ??= new UniversiteUserRepository(_context, _userManager, _roleManager);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task EnsureCreatedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task EnsureDeletedAsync()
    {
        await _context.Database.EnsureDeletedAsync();
    }
    public IEtudiantRepository GetEtudiantRepository()
    {
        return new EtudiantRepository(_context);
    }

    public INoteRepository GetNoteRepository()
    {
        return new NoteRepository(_context);
    }

    public IUeRepository GetUeRepository()
    {
        return new UeRepository(_context);
    }

    public IParcoursRepository GetParcoursRepository()
    {
        return new ParcoursRepository(_context);
    }
}
