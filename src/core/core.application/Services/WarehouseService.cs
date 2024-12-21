using core.application.Contract.API.DTO.Warehouse;
using core.application.Contract.API.Interfaces;
using core.application.Contract.infrastructure;
using core.application.Contract.Infrastructure;
using core.domain.DomainModelDTOs.WarehouseDTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace core.application.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;

        public WarehouseService(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        public class UpdateFailed : Exception 
        {
            public UpdateFailed(string notUpdatedMessage) : base (notUpdatedMessage) {  }
        }

        public class UpdateSuccessfull : Exception
        {
            public UpdateSuccessfull(string updatedMessage) : base(updatedMessage) { }
        }

        // Create an item list asynchronously
        public async Task<IActionResult> CreateItemListAsync(CreateItemListDTO createItemListDTO)
        {
            var itemList = new ItemList
            {
                ItemListCode = createItemListDTO.ItemListCode,
                Group = createItemListDTO.Group,
                Name = createItemListDTO.Name,
                Unit = createItemListDTO.Unit
            };

            var result = await _warehouseRepository.CreateItemListAsync(itemList);
            if (result == null)
                return new BadRequestObjectResult("برای ایجاد یک رکورد برای لیست اقلام در این جدول شما دارای مشکل هستید.");

            return new OkObjectResult(result);
        }

        // Create the main Warehouse of the ListItems in every builing of Enjoylife team :
        public async Task<IActionResult> CreateWarehouse(CreateWarehouseDTO createWarehouseDTO)
        {
            // Prepare the warehouse item with serializable properties only
            var warehouseItems = new CreateWarehouseDTO
            {
                Id = createWarehouseDTO.Id,
                ItemCode = createWarehouseDTO.ItemCode,
                ItemCounts = createWarehouseDTO.ItemCounts,
                ItemDateRegistration = createWarehouseDTO.ItemDateRegistration,
                ItemDescription = createWarehouseDTO.ItemDescription,
                ItemGroup = createWarehouseDTO.ItemGroup,
                ItemMonth = createWarehouseDTO.ItemMonth,
                ItemName = createWarehouseDTO.ItemName,
                ItemStatus = createWarehouseDTO.ItemStatus,
                ItemUnit = createWarehouseDTO.ItemUnit,
            };

            // Call repository method to create the warehouse item
            var result = await _warehouseRepository.CreateWarehouseAsync(warehouseItems);

            // Return an appropriate response
            if (result != null)
            {
                return new OkObjectResult("Item has been created successfully!");
            }
            else
            {
                return new BadRequestObjectResult("برای ایجاد یک رکورد از لیست آیتم های اقلام در جدول انبار با مشکل مواجه شده اید.");
            }
        }


        // Get item list by code asynchronously
        public async Task<IActionResult> GetItemListByCodeAsync(int itemListCode)
        {
            var result = await _warehouseRepository.GetItemListByCodeAsync(itemListCode);
            if (result == null)
                return new NotFoundObjectResult($"Item list with code {itemListCode} not found");

            return new OkObjectResult(result);
        }

        // Get an item of records of table Warehouse
        public async Task<IActionResult> GetWarehouseItemByItemCode(int warehouseItemCode)
        {
            var result = _warehouseRepository.GetWarehouseItemByItemCodeAsync(warehouseItemCode);
            if (result == null)
                return new NotFoundObjectResult($"Item of Warehouse is not found with the code number {warehouseItemCode}");

            return new OkObjectResult(result);
        }

        // Gets all items
        public async Task<IActionResult> GetItems()
        {
            var result = await _warehouseRepository.GetItemsAsync();
            if (result == null)
                return new NotFoundObjectResult($"Items are NOT found !");

            return new OkObjectResult(result);
        }

        // get all Warehouse items :
        public async Task<IActionResult> GetWarehouses()
        {
            var result = await _warehouseRepository.GetWarehousesAsync();
            if (result == null)
                return new NotFoundObjectResult($"Items are NOT found !");

            return new OkObjectResult(result);
        }

        // Update item list by code asynchronously
        public async Task<ActionResult<ItemList>> UpdateItemListByCodeAsync(int itemListCode, CreateItemListDTO updateItemListDTO)
        {
            var updatedItemList = new ItemList
            {
                Group = updateItemListDTO.Group,
                ItemListCode = updateItemListDTO.ItemListCode,
                Name = updateItemListDTO.Name,
                Unit = updateItemListDTO.Unit
            };

            var result = await _warehouseRepository.UpdateItemListByCodeAsync(itemListCode, updateItemListDTO);
            if (result == null)
                return new BadRequestObjectResult($"Failed to update item list with code {itemListCode}");

            return result;
        }
        // Update item list by code asynchronously
        public async Task<ActionResult<EnjoylifeItems>> UpdateWarehouseAsync(int warehouseItemCode, 
                                                                                CreateWarehouseDTO updateWarehouseItemCode)
        {
             var updatedItemList = new EnjoylifeItems
            {
                ItemCode = updateWarehouseItemCode.ItemCode,
                ItemName = updateWarehouseItemCode.ItemName,
                ItemDescription = updateWarehouseItemCode.ItemDescription,
                ItemCounts = updateWarehouseItemCode.ItemCounts,
                ItemStatus = updateWarehouseItemCode.ItemStatus,
                ItemDateRegistration = updateWarehouseItemCode.ItemDateRegistration,
                ItemGroup = updateWarehouseItemCode.ItemGroup,
                ItemUnit = updateWarehouseItemCode.ItemUnit,
                ItemMonth = updateWarehouseItemCode.ItemMonth,
            };

            var result = await _warehouseRepository.UpdateWarehouseByCodeAsync(warehouseItemCode, updateWarehouseItemCode);
            if (result == null)
                return new BadRequestObjectResult($"Failed to update item list with code {updatedItemList}");

            return new OkResult();
        }

        // Delete item list by code asynchronously
        public async Task<IActionResult> DeleteItemListByCodeAsync(int itemListCode)
        {
            await _warehouseRepository.DeleteItemListByCodeAsync(itemListCode);
            return new NoContentResult();
        }

        // Delete an item from Table named Warehouse :
        public async Task<IActionResult> DeleteWarehouseByIdAsync(int warehouseId)
        {
            await _warehouseRepository.DeleteWarehouseByIdAsync(warehouseId); 
            return new NoContentResult();
        }


        // Create Units for the back-end resources of the forms :
        public async Task<IActionResult> CreateUnits([FromBody] CreateUnitsDTO createUnitsDTO)
        {
            try
            {
                var create = new CreateUnitsDTO
                {
                    Id = createUnitsDTO.Id,
                    Name = createUnitsDTO.Name,
                };

                var created = await _warehouseRepository.CreateUnitsAsync(createUnitsDTO);

                if (created == null)
                {
                    return new BadRequestObjectResult("It is not Saved in Database ... There is an error in Insertion into database .");
                } else
                {
                    return new OkObjectResult("Inserted inti Units table .");
                }

            } catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        // Create Units for the back-end resources of the forms :
        public async Task<IActionResult> CreateGroups([FromBody] CreateGroupsDTO createGroupsDTO)
        {
            try
            {
                var create = new CreateGroupsDTO
                {
                    Id = createGroupsDTO.Id,
                    Name = createGroupsDTO.Name,
                };

                var created = await _warehouseRepository.CreateGroupsAsync(createGroupsDTO);

                if (created == null)
                {
                    return new BadRequestObjectResult("It is not Saved in Database ... There is an error in Insertion into database .");
                }
                else
                {
                    return new OkObjectResult("Inserted inti Groups table .");
                }

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
        public async Task<IActionResult> GetUnits()
        {
            var result = await _warehouseRepository.GetUnitsAsync();
            if (result == null)
                return new NotFoundObjectResult($"Items are NOT found !");

            return new OkObjectResult(result);
        }
        public async Task<IActionResult> GetGroups()
        {
            var result = await _warehouseRepository.GetGroupsAsync();
            if (result == null)
                return new NotFoundObjectResult($"Items are NOT found !");

            return new OkObjectResult(result);
        }
        public async Task<IActionResult> GetUnitById(int unitId)
        {
            var result = await _warehouseRepository.GetUnitById(unitId);
            if (result == null)
                return new NotFoundObjectResult($"Unable to find {unitId}");

            return new OkObjectResult(result);
        }
        public async Task<IActionResult> GetGroupById(int groupId)
        {
            var result = await _warehouseRepository.GetGroupById(groupId);
            if ( result == null)
                return new NotFoundObjectResult($"Unable to find {groupId}");

            return new OkObjectResult(result);
        }
        public async Task<IActionResult> UpdateUnit(int unitId, [FromBody] ManageUnitsDTO units)
        {
            var result = await _warehouseRepository.UpdateUnit(unitId, units);

            if (result == null)
                return new NotFoundResult();
            if (result is NoContentResult)
                return new OkResult();

            return result;
        }
        public async Task<IActionResult> UpdateGroup(int groupId, [FromBody] ManageGroupsDTO groups)
        {
            var result = await _warehouseRepository.UpdateGroup(groupId, groups);

            if (result == null)
                return new NotFoundResult();
            if (result is NoContentResult)
                return new OkResult();

            return result;
        }
        public async Task<IActionResult> DeleteUG(int ugId, string formCheck)
        {
            if (string.IsNullOrEmpty(formCheck))
                return new BadRequestObjectResult("FormCheck parameter cannot be null or empty.");

            var result = await _warehouseRepository.DeleteUG(ugId, formCheck);
            return result;
        }
    }
}
