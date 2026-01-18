using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Employee.Application.Constants;
using Liberty.Reservation.Employee.Application.Contexts.Entities.Metas;
using Liberty.Reservation.Employee.Application.Contexts.Entities.Relations;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class EmployeeService(
    IEmployeeRepository employeeRepository,
    IFileRepository fileRepository,
    IFileEmployeeRepository fileEmployeeRepository,
    UserManager<Contexts.Entities.Employee> userManager
)
    : IEmployeeService
{
    public async Task<(int, Contexts.Entities.Employee[])> GetMany(
        string? employeeCode,
        string? name,
        string? email,
        int? take,
        int? skip
    )
    {
        var query = employeeRepository.GetQueryableWithAsNoTracking();

        var count = query.Count();

        if (skip is not null)
        {
            query = query.Skip(skip.Value);
        }

        if (take is not null)
        {
            query = query.Take(take.Value);
        }

        var data = await query.ToArrayAsync();

        return (count, data);
    }

    public async Task<Contexts.Entities.Employee> GetCreatedUser(
        string email
    )
    {
        // emailの重複は退会との兼ね合いで論理削除のためあり得るため、userManager.FindByEmailAsync を使用できない
        var query = employeeRepository
            .GetQueryableWithAsNoTracking()
            .Include(a => a.EmployeeMeta)
            .Where(a => a.Email == email)
            .Where(a => a.State == EmployeeStatus.Created);

        var employees = await query.ToListAsync();

        // 一件もなければnotfound
        if (employees is not { Count: > 0 })
        {
            throw new AppEmployeeNotfoundException(email);
        }

        // 一件以上あれば例外
        var employee = employees.Single();

        // 上記でステータスも見ているがuserManagerでもチェック
        // メール認証されていたらNG
        var user = await userManager.FindByIdAsync(employee.Id)
                   ?? throw new AppEmployeeNotfoundException(employee.Code);
        if (await userManager.IsEmailConfirmedAsync(user))
        {
            throw new Exception("created");
        }

        return user;
    }

    public async Task<Contexts.Entities.Employee> GetMailConfirmedUser(
        string email
    )
    {
        // emailの重複は退会との兼ね合いで論理削除のためあり得るため、userManager.FindByEmailAsync を使用できない
        var query = employeeRepository
            .GetQueryableWithAsNoTracking()
            .Include(a => a.EmployeeMeta)
            .Where(a => a.Email == email)
            .Where(a => a.State == EmployeeStatus.EmailConfirmed);

        var datas = await query.ToListAsync();

        // 一件もなければnotfound
        if (!datas.Any())
        {
            throw new AppEmployeeNotfoundException(email);
        }

        // 一件以上あれば例外
        var data = datas.Single();

        // 上記でステータスも見ているがuserManagerでもチェック
        // メール認証されていたらNG
        var user = await userManager.FindByIdAsync(data.Id)
                   ?? throw new Exception("Employee not found");
        if (!await userManager.IsEmailConfirmedAsync(user))
        {
            throw new Exception("not confirmed");
        }

        return user;
    }

    public async Task<(Contexts.Entities.Employee, string)> Create(
        string name,
        string email
    )
    {
        // 存在確認
        if (await employeeRepository
                .GetQueryableWithAsNoTracking()
                .Include(a => a.EmployeeMeta)
                .Where(a => a.Email == email)
                .Where(a => a.State != EmployeeStatus.Cancelled)
                .AnyAsync())
        {
            throw new AppEmployeeExistedException(email);
        }

        var password = Encryptor.GetInstance().Create();

        // var meta = new EmployeeMeta
        // {
        //     Code = EntityUtil.CreateCode(),
        //     Name = name
        // }

        var data = new Contexts.Entities.Employee
        {
            Code = EntityUtil.CreateCode(),
            UserName = name,
            Email = email,
            State = EmployeeStatus.Created,
            IsEnabled = true
        };

        //
        // var result = await userManager.CreateAsync(data, password);
        // if (!result.Succeeded)
        // {
        //     throw new Exception("password false");
        // }

        // await employeeRepository.AddAsync(data);
        //await _userManager.AddPasswordAsync(data, password);

        //await this._DataContext.SaveChangesAsync();

        return (data, password);
    }

    public async Task<(string, string, string)> GenerateEmailConfirmationTokenAsync(
        Contexts.Entities.Employee data,
        DateTime expired
    )
    {
        // メール認証コード、トークン、ハッシュ発行
        var emailConfirmationCode = "hogehoge"; // FIXME
        var token = await userManager.GenerateEmailConfirmationTokenAsync(data);
        var hash = Guid.NewGuid().ToString();
        {
            //
            var emailConfirmation = new EmployeeTemporarily.EmailConfirmation
            {
                Code = emailConfirmationCode,
                Token = token,
                Hash = hash,
                Expired = expired
            };

            var temporarily = data.EmployeeMeta?.Temporarily;
            if (temporarily is not null)
            {
                temporarily.EmailConfirmations = [emailConfirmation];
            }

            // jsonに設定
            if (data.EmployeeMeta is not null)
            {
                data.EmployeeMeta.Temporarily = temporarily;
            }
        }
        await employeeRepository.UpdateAsync(data);

        return (token, emailConfirmationCode, hash);
    }

    public async Task<(string, string, string)> GeneratePasswordResetTokenAsync(
        Contexts.Entities.Employee data,
        DateTime expired
    )
    {
        // メール認証コード、トークン、ハッシュ発行
        var emailConfirmationCode = "hogehoge"; // FIXME
        var token = await userManager.GeneratePasswordResetTokenAsync(data);
        var hash = Guid.NewGuid().ToString();
        {
            //
            var passwordReset = new EmployeeTemporarily.PasswordReset
            {
                Code = emailConfirmationCode,
                Token = token,
                Hash = hash,
                Expired = expired
            };

            var temporarily = data.EmployeeMeta?.Temporarily;
            if (temporarily is not null)
            {
                temporarily.PasswordResets = [passwordReset];
            }

            // jsonに設定
            if (data.EmployeeMeta is not null)
            {
                data.EmployeeMeta.Temporarily = temporarily;
            }
        }
        await employeeRepository.UpdateAsync(data);

        return (token, emailConfirmationCode, hash);
    }

    public async Task ConfirmEmailAsync(
        Contexts.Entities.Employee data,
        string token
    )
    {
        var result = await userManager.ConfirmEmailAsync(data, token);
        if (!result.Succeeded)
        {
            throw new Exception("can not confirm");
        }

        data.State = EmployeeStatus.EmailConfirmed;

        var meta = data.EmployeeMeta;
        if (meta?.Temporarily is not null)
        {
            meta.Temporarily.EmailConfirmations = [];
        }

        data.EmployeeMeta = meta;

        await employeeRepository.UpdateAsync(data);
    }

    public async Task ResetPasswordAsync(
        Contexts.Entities.Employee data,
        string token,
        string password
    )
    {
        var result = await userManager.ResetPasswordAsync(data, token, password);
        if (!result.Succeeded)
        {
            throw new Exception("can not confirm");
        }

        var meta = data.EmployeeMeta;
        if (meta?.Temporarily is not null)
        {
            meta.Temporarily.PasswordResets = [];
        }

        data.EmployeeMeta = meta;

        await employeeRepository.UpdateAsync(data);
    }

    public Task UpdateEmail(
        string employeeCode,
        string email
    )
    {
        // FIXME not implement
        throw new NotImplementedException();

        //var data = await _DataContext.Employees
        //    .Where(a => a.Code == code)
        //    .SingleOrDefaultAsync() ?? throw new AppEmployeeNotfoundException(code);

        //data.Email = email;
    }

    public Task UpdatePassword(
        string employeeCode,
        string password
    )
    {
        // FIXME not implement
        throw new NotImplementedException();
    }

    public async Task Delete(
        string employeeCode
    )
    {
        //// FIXME not implement
        //throw new NotImplementedException();

        var res = await employeeRepository
            .GetQueryableWithAsNoTracking()
            .Where(a => a.Code == employeeCode)
            .SingleOrDefaultAsync() ?? throw new AppEmployeeNotfoundException(employeeCode);

        await employeeRepository.DeleteAsync(res);
    }

    public async Task<(int, FileEmployee[])> GetFileMany(
        string employeeCode,
        string? fileCode,
        int? take,
        int? skip
    )
    {
        var query = fileEmployeeRepository
            .GetQueryableWithAsNoTracking()
            .Where(a => a.Employee!.Code == employeeCode);

        var count = query.Count();

        if (skip != null)
        {
            query = query.Skip(skip.Value);
        }

        if (take != null)
        {
            query = query.Take(take.Value);
        }

        var data = await query.ToArrayAsync();

        return (count, data);
    }

    public async Task<FileEmployee> CreateFile(
        string employeeCode,
        string fileCode,
        string secret,
        EmployeeFileTypes type
    )
    {
        var employee = await employeeRepository
            .GetQueryableWithAsNoTracking()
            .Where(a => a.Code == employeeCode)
            .SingleOrDefaultAsync() ?? throw new AppEmployeeNotfoundException(employeeCode);

        var file = await fileRepository.Create(fileCode, secret);
        var fileEmployee = new FileEmployee(file, employee) { Type = type };

        await fileEmployeeRepository.AddAsync(fileEmployee);
        // await unitOfWork.SaveChangesAsync();

        return fileEmployee;
    }

    public async Task DeleteFile(
        string employeeCode,
        string fileCode,
        string secret
    )
    {
        var res = await fileEmployeeRepository
            .GetQueryableWithAsNoTracking()
            .Include(a => a.File)
            .Where(a => a.Employee!.Code == employeeCode)
            .Where(a => a.File!.Code == fileCode)
            .SingleOrDefaultAsync() ?? throw new AppFileEmployeeNotfoundException(fileCode, employeeCode);

        if (res.File is not null)
        {
            fileRepository.Delete(res.File);
        }
    }

    public async Task<FileEmployee> CreateAvatar(
        string employeeCode,
        string fileCode,
        string secret
    )
    {
        // 現在のアバターは消す
        await DeleteAvatar(employeeCode);

        return await CreateFile(employeeCode, fileCode, secret, EmployeeFileTypes.Avatar);
    }

    public async Task DeleteAvatar(
        string employeeCode
    )
    {
        var res = await fileEmployeeRepository
            .GetQueryableWithAsNoTracking()
            .Include(a => a.File)
            .Where(a => a.Employee!.Code == employeeCode)
            .Where(a => a.Type == EmployeeFileTypes.Avatar)
            .ToListAsync();

        await fileRepository.DeleteRangeAsync(res.Select(a => a.File)!);
    }
}
