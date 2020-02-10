using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Business.View;
using UserService.Entities;

namespace UserService.DomainToView
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Usuario, UsuarioView>();
            CreateMap<UsuarioView, Usuario>();
        }
    }
}
