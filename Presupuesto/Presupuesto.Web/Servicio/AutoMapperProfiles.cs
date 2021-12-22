using AutoMapper;
using Presupuesto.Web.Models;

namespace Presupuesto.Web.Servicio
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Cuenta, CuentaCreacionViewModel>();
        }      
    }
}
