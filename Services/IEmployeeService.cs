using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using AutoHub.Models.Entities;

namespace AutoHub.Services;

/// <summary>
/// Business logic cho nhân viên: upload ảnh + ghi DB.
/// Controller chỉ gọi service, không tự xử lý file hay serialize.
/// </summary>
public interface IEmployeeService
{
    Task<Employee> CreateAsync(Employee employee, IFormFile? thumbnail, List<IFormFile>? images);
    Task UpdateAsync(Employee existing, Employee updated, IFormFile? thumbnail, List<IFormFile>? images);
}
