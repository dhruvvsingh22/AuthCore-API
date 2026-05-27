using UsersApi.Models;
using UsersApi.DTOs;
namespace UsersApi.Services;
// public interface IUserService
// {
//     List<UserResponseDTO> GetAll();
//     UserResponseDTO ?GetById(int id);
//     UserResponseDTO Create(User user);
//     UserResponseDTO ?Update(int id,User user);
//     bool Delete(int id);
// }

public interface IUserService
{
    Task<List<UserResponseDTO>> GetAllAsync();
    Task<UserResponseDTO?> GetByIdAsync(int id);
    Task<UserResponseDTO> CreateAsync(User user);
    Task<UserResponseDTO?> UpdateAsync(int id, User user);
    Task<bool> DeleteAsync(int id);
    Task<PageResponseDTO<UserResponseDTO>>GetPagedAsync(int page,int pageSize);
}