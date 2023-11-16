using AutoMapper;
using cAPParel.API.Entities;
using cAPParel.API.Filters;
using cAPParel.API.Helpers;
using cAPParel.API.Models;
using cAPParel.API.Services.Basic;
using Microsoft.OpenApi.Any;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

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

        public async Task<OperationResult<FileDataDto>> DeleteFile(int id )
        {
            return await _itemRepository.DeleteFile(id);
        }

        public async Task<FileDataDto> AddFileDataToItemAsync(int itemid, FileDataForCreationDto fileDataForCreationDto)
        {
            var fileData = _mapper.Map<FileData>(fileDataForCreationDto);
            var item = await _basicRepository.GetByIdWithEagerLoadingAsync(itemid);
            item.FileData.Add(fileData);
            await _basicRepository.SaveChangesAsync();
            return _mapper.Map<FileDataDto>(fileData);
        }

        public async Task<PieceDto> CreatePieceAsync(PieceForCreationDto pieceForCreationDto, int itemid)
        {
            var piece = _mapper.Map<Piece>(pieceForCreationDto);
            await _itemRepository.AddPieceAsync(piece, itemid);
            return _mapper.Map<PieceDto>(piece);
        }

        public async override Task<PagedList<ItemFullDto>> GetFullAllWithEagerLoadingAsync(IEnumerable<IFilter>? filters,
            ResourceParameters parameters,
            params Expression<Func<Item, 
                object>>[] includeProperties)
        {
            var listToReturn = _basicRepository.GetQueryableAllWithEagerLoadingAsync(includeProperties);
            var IdFilter = filters.FirstOrDefault(c => c.FieldName == "CategoryIds");
            if (IdFilter is not null)
            {
                List<int> ids = IdFilter.Value as List<int>;
                listToReturn = listToReturn.Where(c => ids.Any(id => c.CategoryId == id));
            }
            foreach (var filter in filters)
            {                    
                    if(filter.FieldName == "HasPieces")
                    listToReturn = listToReturn.Where(c => c.Pieces.Count > 0);
                    else
                if (filter.FieldName == "Size")
                    listToReturn = listToReturn.Where(i => i.Pieces.Any(p => p.Size == filter.Value.ToString()));
                else
                {
                    if (filter.FieldName != "CategoryIds")
                        listToReturn = FilterEntity(listToReturn, filter);
                }
            }

            if (!string.IsNullOrWhiteSpace(parameters.SearchQuery))
            {
                listToReturn = SearchEntityByProperty(listToReturn, parameters.SearchQuery);
            }

            listToReturn = ApplyOrdering(listToReturn, parameters.OrderBy);

            var finalList = await PagedList<Item>
                .CreateAsync(listToReturn,
                parameters.PageNumber,
                parameters.PageSize);
            var finalListToReturn = _mapper.Map<PagedList<ItemFullDto>>(finalList);
            return finalListToReturn;
        }

        public async override Task<PagedList<ItemDto>> GetAllWithEagerLoadingAsync(IEnumerable<IFilter>? filters,
            ResourceParameters parameters,
            params Expression<Func<Item,
                object>>[] includeProperties)
        {
            var listToReturn = _basicRepository.GetQueryableAllWithEagerLoadingAsync(includeProperties);
            var IdFilter = filters.FirstOrDefault(c => c.FieldName == "CategoryIds");
            if(IdFilter is not null)
            {
                List<int> ids = IdFilter.Value as List<int>;
                listToReturn = listToReturn.Where(c => ids.Any(id => c.CategoryId == id));
            }
            
                foreach (var filter in filters)
                {
                    if (filter.FieldName != "CategoryIds")
                        listToReturn = FilterEntity(listToReturn, filter);
                }

            if (!string.IsNullOrWhiteSpace(parameters.SearchQuery))
            {
                listToReturn = SearchEntityByProperty(listToReturn, parameters.SearchQuery);
            }

            listToReturn = ApplyOrdering(listToReturn, parameters.OrderBy);

            var finalList = await PagedList<Item>
                .CreateAsync(listToReturn,
                parameters.PageNumber,
                parameters.PageSize);
            var finalListToReturn = _mapper.Map<PagedList<ItemDto>>(finalList);
            return finalListToReturn;
        }



    }
}
