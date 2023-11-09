using AutoMapper;
using cAPParel.API.Entities;
using cAPParel.API.Models;
using cAPParel.API.Services.Basic;

namespace cAPParel.API.Services.ItemServices
{
    public class ItemService : BasicService<ItemDto, Item, ItemFullDto, ItemForCreationDto, ItemForUpdateDto>, IItemService
    {
        private readonly IConfiguration _configuration;
        private readonly IItemRepository _itemRepository;

        public ItemService(IMapper mapper, IConfiguration configuration, IBasicRepository<Item> basicRepository, IItemRepository itemRepository) : base(mapper, basicRepository)
        {
            _configuration = configuration;
            _itemRepository = itemRepository;
        }

        public async Task<PieceDto> CreatePieceAsync(PieceForCreationDto pieceForCreationDto, int itemid)
        {
            var piece = _mapper.Map<Piece>(pieceForCreationDto);
            await _itemRepository.AddPieceAsync(piece,itemid);
            return _mapper.Map<PieceDto>(piece);
        }
    }
}
