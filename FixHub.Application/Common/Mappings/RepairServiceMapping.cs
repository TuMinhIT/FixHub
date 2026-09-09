using AutoMapper;
using FixHub.Application.Features.RepairServices.Commands.CreateRepairService;
using FixHub.Application.Features.RepairServices.Commands.UpdateRepairService;
using FixHub.Application.Features.RepairServices.DTOs;
using FixHub.Domain.Entities;

namespace FixHub.Application.Common.Mappings
{
    public class RepairServiceMapping : Profile
    {
        public RepairServiceMapping()
        {
            CreateMap<RepairService, RepairServiceResponse>();
            CreateMap<CreateRepairServiceCommand, RepairService>();
            CreateMap<UpdateRepairServiceCommand, RepairService>();
        }
    }
}
