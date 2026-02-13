using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Domain.DTOs.Request;
using YourSolution.Domain.Enums;
using YourSolution.Domain.Models;
using YourSolution.Infrastructure.DTOs.Response;
using YourSolution.Infrastructure.DTOs;
using YourSolution.Infrastructure.Repositories;
using YourSolution.Domain.Data;
using YourSolution.Infrastructure.Extensions;

namespace YourSolution.Domain.Repositories
{
    public class UserAccountRepository
    {
        private readonly AppDBContext _context;
        private readonly PaginationQueryRepository _paginationQueryRepository;

        public UserAccountRepository(AppDBContext context, PaginationQueryRepository paginationQueryRepository)
        {
            _context = context;
            _paginationQueryRepository = paginationQueryRepository;
        }

        /// <summary>
        /// 管理者查詢使用者列表(不包含目前登入的使用者本身的資料)
        /// </summary>
        /// <param name="dto">查詢條件</param>
        /// <param name="flowId">目前登入的使用者Id</param>
        /// <param name="limit">單頁最大的資料筆數</param>
        /// <returns></returns>
        public Task<SearchPageResultDto<UserAccount>> GetUserAccountBySearchDtoAsync(UserAccountSearchDto dto, Guid flowId, int limit)
        {
            #region 設定查詢條件

            StringBuilder baseQuery = new StringBuilder(@"SELECT * FROM UserAccounts  
                                                          Where FlowId!=@FlowId ");

            var parameters = new DynamicParameters();
            parameters.Add("@FlowId", flowId);

            if (string.IsNullOrEmpty(dto.Account) == false)
            {
                baseQuery.AppendLine(" and Account like @Account");
                parameters.Add("@Account", $"%{dto.Account}%");
            }

            if (dto.AccountStatus >= 0)
            {
                if (dto.AccountStatus > 1)
                    dto.AccountStatus = 1;

                baseQuery.AppendLine(" and IsLock=@IsLock");
                parameters.Add("@IsLock", dto.AccountStatus);
            }

            if (string.IsNullOrEmpty(dto.Name) == false)
            {
                baseQuery.AppendLine(" and Name like @Name");
                parameters.Add("@Name", $"%{dto.Name}%");
            }

            if (dto.Role >= 0)
            {
                if (dto.Role > 1)
                    dto.Role = 1;

                baseQuery.AppendLine(" and Role=@Role");
                parameters.Add("@Role", dto.Role);
            }

            #endregion

            return _paginationQueryRepository.PaginationQueryAsync<UserAccount>(new PaginationQueryDto
            {
                BaseQuery = baseQuery.ToString(),
                SqlConnection = _context.Database.GetDbConnection().ConvertToSqlConnection(_context.Database.GetConnectionString() ?? string.Empty),
                Limit = limit,
                Page = dto.Page,
                Parameters = parameters
            });
        }

        /// <summary>
        /// 取得使用者資料
        /// </summary>
        /// <param name="flowid">PK</param>
        /// <returns></returns>
        public Task<UserAccount?> GetUserAccountAsync(Guid flowid)
        {
            return _context.UserAccounts.AsNoTracking().FirstOrDefaultAsync(x => x.FlowId == flowid);
        }

        /// <summary>
        /// 取得使用者資料
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="Pwd">密碼</param>
        /// <returns></returns>
        public Task<UserAccount?> GetUserAccountAsync(string Account, string Pwd)
        {
            return _context.UserAccounts.AsNoTracking().FirstOrDefaultAsync(x => x.Account == Account && x.Pwd == Pwd);
        }

        /// <summary>
        /// 取得使用者資料
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <returns></returns>
        public Task<UserAccount?> GetUserAccountAsync(string Account)
        {
            return _context.UserAccounts.AsNoTracking().FirstOrDefaultAsync(x => x.Account == Account);
        }

        /// <summary>
        /// 系統自動鎖定使用者資料
        /// </summary>
        /// <param name="userAccount">使用者資料</param>
        /// <param name="IsLockMsg">錯誤訊息</param>
        /// <returns></returns>
        public Task<int> UserAccountIsLockAsync(UserAccount userAccount, string IsLockMsg)
        {
            userAccount.IsLock = true;
            userAccount.ChangeLockStatusMsg = IsLockMsg;
            userAccount.ChangeLockStatusTime = DateTime.Now;
            userAccount.ChangeLockStatusUserFlowId = Guid.Empty; //預設為空，代表是由系統鎖定的
            return UpdateSetting(userAccount);
        }

        /// <summary>
        /// 管理者鎖定使用者資料
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="IsLockMsg"></param>
        /// <param name="RowVersion"></param>
        /// <param name="flowid"></param>
        /// <returns></returns>
        public Task<int> UserAccountIsLockAsync(UserAccount userAccount, string IsLockMsg, byte[] RowVersion, Guid flowid)
        {
            userAccount.IsLock = true;
            userAccount.ChangeLockStatusMsg = IsLockMsg;
            userAccount.ChangeLockStatusTime = DateTime.Now;
            userAccount.ChangeLockStatusUserFlowId = flowid;
            //return UpdateSetting(userAccount, RowVersion);
            return UpdateSetting(userAccount);
        }

        /// <summary>
        /// 管理者解除鎖定使用者資料
        /// </summary>
        /// <param name="userAccount">使用者資料</param>
        /// <returns></returns>
        public Task<int> UserAccountUnLockAsync(UserAccount userAccount, byte[] RowVersion, Guid flowid)
        {
            userAccount.IsLock = false;
            userAccount.ChangeLockStatusMsg = string.Empty;
            userAccount.ChangeLockStatusUserFlowId = flowid;
            userAccount.ChangeLockStatusTime = DateTime.Now;
            userAccount.LoginErrorCount = 0;
            //return UpdateSetting(userAccount, RowVersion);
            return UpdateSetting(userAccount);
        }

        /// <summary>
        /// 更新上次成功登入的時間
        /// </summary>
        /// <param name="userAccount"></param>
        /// <returns></returns>
        public async Task<int> UpdateLastLoginTimeAsync(UserAccount userAccount)
        {
            if (userAccount.IsLock)
                return 0;

            userAccount.LoginErrorCount = 0;
            userAccount.LastLoginTime = DateTime.Now;
            return await UpdateSetting(userAccount);
        }

        /// <summary>
        /// 登入失敗次數+1
        /// </summary>
        /// <param name="userAccount"></param>
        /// <returns></returns>
        public Task<int> UpdateLoginErrorCountAsync(UserAccount userAccount)
        {
            userAccount.LoginErrorCount++;
            return UpdateSetting(userAccount);
        }

        /// <summary>
        /// 新增使用者
        /// </summary>
        /// <param name="userAccount">使用者資料</param>
        /// <returns></returns>
        public Task<int> InsertUserAccountAsync(UserAccount userAccount)
        {
            userAccount.FlowId = Guid.NewGuid();
            _context.UserAccounts.Add(userAccount);
            return _context.SaveChangesAsync();
        }

        /// <summary>
        /// 使用者更新自己的資料
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<int> UpdateUserAccountAsync(UserAccount userAccount, UpdateUserAccountDto dto)
        {
            if (string.IsNullOrEmpty(dto.Pwd) == false)
                userAccount.Pwd = dto.Pwd.ToEncryptPassword();

            //userAccount.PsnNo = dto.PsnNo;
            userAccount.Name = dto.Name;
            return UpdateSetting(userAccount, dto.RowVersion);
            //return UpdateSetting(userAccount);
        }

        /// <summary>
        /// 系統管理者更新使用者資料
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<int> UpdateUserAccountManageAsync(UserAccount userAccount, UpdateUserAccountManageDto dto)
        {
            if (string.IsNullOrEmpty(dto.Pwd) == false)
                userAccount.Pwd = dto.Pwd.ToEncryptPassword();

            //userAccount.PsnNo = dto.PsnNo;
            userAccount.Name = dto.Name;
            userAccount.Role = (UserRole)dto.Role;
            //return UpdateSetting(userAccount, dto.RowVersion);
            return UpdateSetting(userAccount);
        }

        /// <summary>
        /// userAccount的更新設定
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="RowVersion"></param>
        private Task<int> UpdateSetting(UserAccount userAccount, byte[]? RowVersion = null)
        {
            if (RowVersion != null && RowVersion.Length > 0)
                userAccount.RowVersion = RowVersion;

            userAccount.UpdateTime = DateTime.Now;
            var entry = _context.Entry(userAccount);
            entry.State = EntityState.Modified;

            //將Id欄位標記為不可更新，因為該欄位是識別值(自動遞增)
            entry.Property(e => e.Id).IsModified = false;
            return _context.SaveChangesAsync();
        }
    }
}
