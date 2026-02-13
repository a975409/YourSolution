using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Domain.DTOs.Response;
using YourSolution.Domain.Models;
using YourSolution.Infrastructure.Extensions;

namespace YourSolution.Domain.Profiles
{
    /// <summary>
    /// DTO和Model互相轉換
    /// </summary>
    public class ModelProfile : Profile
    {
        public ModelProfile()
        {
            CreateMap<UserAccount, GetUserAccountDto>()
                .ForMember(x => x.CreateTime, y => y.MapFrom(y => y.CreateTime.ToSimpleDateTime()))
                .ForMember(x => x.UpdateTime, y => y.MapFrom(y => y.UpdateTime.ToSimpleDateTime()))
                .ForMember(x => x.LastLoginTime, y => y.MapFrom(y => y.LastLoginTime.ToSimpleDateTime()));
        }
    }
}
