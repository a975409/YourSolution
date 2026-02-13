using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Infrastructure.DTOs;
using YourSolution.Infrastructure.DTOs.Response;
using YourSolution.Infrastructure.Extensions;
using YourSolution.Infrastructure.Models;

namespace YourSolution.Infrastructure.Profiles
{
    public class ModelProfile : Profile
    {
        public ModelProfile()
        {
            CreateMap<RequestLogDto, RequestLog>();
        }
    }
}
