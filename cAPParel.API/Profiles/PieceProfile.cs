using AutoMapper;

namespace cAPParel.API.Profiles
{
    public class PieceProfile : Profile
    {
        public PieceProfile()
        {
            CreateMap<Entities.Piece, Models.PieceDto>();
            CreateMap<Models.PieceForCreationDto, Entities.Piece>();
        }
    }
}
