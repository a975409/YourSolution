using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Domain.DTOs.Request;
using YourSolution.Domain.DTOs.Response;
using YourSolution.Domain.Enums;
using YourSolution.Domain.Models;
using YourSolution.Domain.Repositories;
using YourSolution.Infrastructure.DTOs.Response;
using YourSolution.Infrastructure.Extensions;
using YourSolution.Infrastructure.Factories;

namespace YourSolution.Domain.Services
{
    public class UserAccountService
    {
        private readonly SystemSettingRepository _systemSettingRepository;
        private readonly UserAccountRepository _userAccountRepository;
        private readonly IMapper _iMapper;
        private readonly SearchPageResultDtoFactory _searchPageResultDtoFactory;

        /// <summary>
        /// 一頁顯示多少筆資料
        /// </summary>
        private readonly int _limit;

        public UserAccountService(SystemSettingRepository systemSettingRepository, UserAccountRepository accountRepository, IMapper iMapper, SearchPageResultDtoFactory searchPageResultDtoFactory)
        {
            _limit = 10;
            _systemSettingRepository = systemSettingRepository;
            _userAccountRepository = accountRepository;
            _iMapper = iMapper;
            _searchPageResultDtoFactory = searchPageResultDtoFactory;
        }

        /// <summary>
        /// 登入前檢查，如果判斷可以登入的話會回傳使用者資料
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="Pwd">密碼</param>
        /// <returns>使用者資料</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<UserAccount> GetLoginSuccessUserAccountAsync(string Account, string Pwd)
        {
            var userAccount = await _userAccountRepository.GetUserAccountAsync(Account);

            await CheckUserAccountCanLogin(userAccount, Pwd);

            return userAccount;
        }

        /// <summary>
        /// 查詢多筆使用者資料(僅限系統管理者使用)
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public async Task<SearchPageResultDto<GetUserAccountDto>> GetSearchPageResultDtoAsync(UserAccountSearchDto dto, Guid flowId)
        {
            var source = await _userAccountRepository.GetUserAccountBySearchDtoAsync(dto, flowId, _limit);

            var result = _searchPageResultDtoFactory.CopyToNewSearchPageResultDto<UserAccount, GetUserAccountDto>(source);

            foreach (var item in result.SearchResult)
            {
                item.AccountStatus = item.IsLock ? "帳戶已鎖定" : "正常";
                item.RoleText = Enum.GetName<UserRole>(item.Role) ?? string.Empty;
            }

            return result;
        }

        /// <summary>
        /// 取得使用者資料
        /// </summary>
        /// <param name="flowid"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<UserAccount> GetUserAccountByFlowIdAsync(Guid flowid)
        {
            var result = await _userAccountRepository.GetUserAccountAsync(flowid);

            if (result == null)
                throw new ArgumentNullException("", "使用者資料不存在");

            return result;
        }

        /// <summary>
        /// 取得使用者資料(前端顯示用)
        /// </summary>
        /// <param name="flowid"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<GetUserAccountDto> GetUserAccountByFlowIdForDisplayAsync(Guid flowid)
        {
            var result = await _userAccountRepository.GetUserAccountAsync(flowid);

            if (result == null)
                throw new ArgumentNullException("", "使用者資料不存在");

            var resultDto = _iMapper.Map<GetUserAccountDto>(result);

            resultDto.AccountStatus = result.IsLock ? "帳戶已鎖定" : "正常";
            resultDto.AccountIsLockMsg = result.ChangeLockStatusMsg;

            return resultDto;
        }

        /// <summary>
        /// 使用者更新自己的資料
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<int> UpdateUserAccountAsync(UpdateUserAccountDto dto)
        {
            var result = await _userAccountRepository.GetUserAccountAsync(dto.FlowId);

            if (result == null)
                throw new ArgumentNullException("", "使用者資料不存在");

            return await _userAccountRepository.UpdateUserAccountAsync(result, dto);
        }

        /// <summary>
        /// 系統管理者更新其他使用者的資料
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<int> UpdateUserAccountManageAsync(UpdateUserAccountManageDto dto)
        {
            var result = await _userAccountRepository.GetUserAccountAsync(dto.FlowId);

            if (result == null)
                throw new ArgumentNullException("", "使用者資料不存在");

            return await _userAccountRepository.UpdateUserAccountManageAsync(result, dto);
        }

        /// <summary>
        /// 透過AD登入帳號取得使用者資料
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="userName">人員姓名</param>
        /// <returns></returns>
        public async Task<UserAccount> GetUserAccountByADAsync(string Account, string userName)
        {
            var userAccount = await _userAccountRepository.GetUserAccountAsync(Account);

            if (userAccount == null)
            {
                userAccount = CreateUserAccount(Account, userName);
                await _userAccountRepository.InsertUserAccountAsync(userAccount);
            }

            return userAccount;
        }

        /// <summary>
        /// 更新上次成功登入的時間
        /// </summary>
        /// <param name="userAccount">帳號</param>
        /// <returns></returns>
        public Task<int> UpdateLastLoginTimeAsync(UserAccount userAccount)
        {
            return _userAccountRepository.UpdateLastLoginTimeAsync(userAccount);
        }

        /// <summary>
        /// 解除鎖定指定的使用者
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="flowid">使用者flowid</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task UserAccountUnLockAsync(UserAccountChangeLockStatusDto dto, Guid flowid)
        {
            var result = await _userAccountRepository.GetUserAccountAsync(dto.FlowId);

            if (result == null)
                throw new ArgumentNullException("", "使用者資料不存在");

            await _userAccountRepository.UserAccountUnLockAsync(result, dto.RowVersion, flowid);
        }

        /// <summary>
        /// 鎖定指定的使用者
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="flowid"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task UserAccountIsLockAsync(UserAccountChangeLockStatusDto dto, Guid flowid)
        {
            var result = await _userAccountRepository.GetUserAccountAsync(dto.FlowId);

            if (result == null)
                throw new ArgumentNullException("", "使用者資料不存在");

            await _userAccountRepository.UserAccountIsLockAsync(result, "管理者鎖定使用者資料", dto.RowVersion, flowid);
        }

        /// <summary>
        /// 判斷該使用者資料可否登入
        /// </summary>
        /// <param name="userAccount">使用者資料</param>
        /// <param name="Pwd">前端輸入的密碼</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task CheckUserAccountCanLogin(UserAccount? userAccount, string Pwd)
        {
            if (userAccount == null)
            {
                throw new ArgumentNullException(string.Empty, "該使用者不存在");
            }

            if (userAccount.IsLock)
                throw new InvalidOperationException("帳號已被系統鎖定，請管理者解除鎖定");

            int MaxLoginFailCount = await _systemSettingRepository.GetMaxLoginFailCountForScheduleAsync();

            if (userAccount.Pwd != Pwd.ToEncryptPassword())
            {
                await _userAccountRepository.UpdateLoginErrorCountAsync(userAccount);

                if (MaxLoginFailCount - userAccount.LoginErrorCount > 0)
                    throw new ArgumentNullException(string.Empty, $"使用者帳號或密碼錯誤，剩餘可輸入次數：{MaxLoginFailCount - userAccount.LoginErrorCount}");
            }

            if (userAccount.LoginErrorCount >= MaxLoginFailCount)
            {
                string errorMsg = "已達到最大登入失敗的次數，帳號已被系統鎖定，請管理者解除鎖定";
                await _userAccountRepository.UserAccountIsLockAsync(userAccount, errorMsg);
                throw new InvalidOperationException(errorMsg);
            }
        }

        /// <summary>
        /// 建立一個新的使用者(僅提供AD帳號使用)
        /// </summary>
        /// <param name="domainAccount">使用者AD帳號</param>
        /// <param name="userName">使用者名稱</param>
        /// <returns></returns>
        private UserAccount CreateUserAccount(string domainAccount, string userName)
        {
            return new UserAccount
            {
                Account = domainAccount,
                Pwd = "default_123456".ToEncryptPassword(),
                IsLock = false,
                ChangeLockStatusMsg = string.Empty,
                LastLoginTime = null,
                LoginErrorCount = 0,
                Name = userName,
                Role = UserRole.一般使用者,
                ChangeLockStatusUserFlowId = null,
                ChangeLockStatusTime = null
            };
        }
    }
}
