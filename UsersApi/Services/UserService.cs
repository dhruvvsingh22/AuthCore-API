using UsersApi.Data;
using UsersApi.Models;
using UsersApi.DTOs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace UsersApi.Services;
public class UserService : IUserService
{
    private readonly AppDbContext _context;
    // private readonly IMemoryCache _cache; 
    private readonly IRedisCacheService _cache;
    private const string UsersCacheKey = "users_all";
    public UserService(AppDbContext context,IRedisCacheService cache)
    {
        _context = context;
        _cache = cache;
    }
    private static UserResponseDTO MapToDto(User user)
    {
        return new UserResponseDTO
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Age = user.Age,
        };
    }
    public async Task<List<UserResponseDTO>> GetAllAsync()
    {
        // return _context.Users.ToList();
        // return _context.Users.Select(u => MapToDto(u)).ToList();
        // if (_cache.TryGetValue(UsersCacheKey, out List<UserResponseDTO>? cachedUsers))
        // {
        //     return cachedUsers!;
        // }
        // var users = _context.Users.Select(u => MapToDto(u)).ToList();
        // _cache.Set(UsersCacheKey, users, TimeSpan.FromMinutes(5));
        // return users;

        var cached = await _cache.GetAsync<List<UserResponseDTO>>(UsersCacheKey);
        if (cached != null)
        {
            return cached;
        }
        var users = await _context.Users.Select(u => MapToDto(u)).ToListAsync();
        await _cache.SetAsync(UsersCacheKey,users,TimeSpan.FromMinutes(5));
        return users;
    }

    public async Task<UserResponseDTO?> GetByIdAsync(int id)
    {
        // return _context.Users.FirstOrDefault(u => u.Id == id);
        //  var user = _context.Users.FirstOrDefault(u => u.Id == id);
        // if (user == null) return null;
        // return MapToDto(user);

        var cacheKey = $"user_{id}";
         var cached = await _cache.GetAsync<UserResponseDTO>(UsersCacheKey);
        if (cached != null)
        {
            return cached;
        }
         var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return null;
        }
        var dto = MapToDto(user);
        await _cache.SetAsync(cacheKey,dto,TimeSpan.FromMinutes(5));
        return dto;
    }
    public async Task<UserResponseDTO> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        await _cache.RemoveAsync(UsersCacheKey);
        // return user;
        return MapToDto(user);
    }

    public async Task<UserResponseDTO?> UpdateAsync(int id,User updateUser)
    {
        // var user = _context.Users.FirstOrDefault(u => u.Id == id);
        // if (user == null)
        // {
        //     return null;
        // }
        // user.Name = updateUser.Name;
        // user.Email = updateUser.Email;
        // user.Age = updateUser.Age;

        // _context.SaveChanges();
        // _cache.Remove(UsersCacheKey);
        // return MapToDto(user);

        var user = await _context.Users.FindAsync(id);
        if(user==null)
        {
            return null;
        }
        user.Name = updateUser.Name;
        user.Email = updateUser.Email;
        user.Age = updateUser.Age;
        await _context.SaveChangesAsync();

        await _cache.RemoveAsync(UsersCacheKey);
        await _cache.RemoveAsync($"user_{id}");
        return MapToDto(user);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // var user = _context.Users.FirstOrDefault(u => u.Id == id );
        var user = await _context.Users.FindAsync(id);
        if(user==null){
            return false;
        }
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        await _cache.RemoveAsync(UsersCacheKey);
        await _cache.RemoveAsync($"user_{id}");
        return true;
    }

    public async Task<PageResponseDTO<UserResponseDTO>>GetPagedAsync(int page,int pageSize)
    {
        var totalCount = await _context.Users.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount/(double)pageSize);
        var users = await _context.Users
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(u => MapToDto(u))
        .ToListAsync();

        return new PageResponseDTO<UserResponseDTO>
        {
            Data = users,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        };
    }
}