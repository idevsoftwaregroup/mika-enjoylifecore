using core.application.Contract.API.DTO.Warehouse;
using core.application.Contract.infrastructure;
using core.domain.DomainModelDTOs.WarehouseDTOs;
using core.domain.entity.financialModels.valueObjects;
using core.domain.entity.financialModels;
using core.infrastructure.Data.persist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using core.application.Framework;

namespace core.infrastructure.Data.repository
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly EnjoyLifeContext _context;

        public WarehouseRepository(EnjoyLifeContext context)
        {
            _context = context;
        }

        #region Warehouse
        // Create a new ItemList and return its related inserted CODE
        public async Task<ItemList> CreateItemListAsync(ItemList createItemListDTO)
        {
            if (createItemListDTO == null)
            {
                throw new ArgumentNullException(nameof(createItemListDTO), "ItemList data cannot be null");
            }

            var itemList = new ItemList
            {
                // Assuming ItemList has the same properties as CreateItemListDTO
                ItemListCode = createItemListDTO.ItemListCode,
                Name = createItemListDTO.Name,
                Group = createItemListDTO.Group,
                Unit = createItemListDTO.Unit,
                // Map other fields from CreateItemListDTO as needed
            };

            await _context.WarehouseItemLists.AddAsync(itemList);
            await _context.SaveChangesAsync();

            return itemList;
        }

        // Method to insert into the Warehouse table
        public async Task<CreateWarehouseDTO> CreateWarehouseAsync(CreateWarehouseDTO createWarehouseDTO)
        {
            if (createWarehouseDTO == null)
            {
                throw new Exception("Warehouse data cannot be NULL.");
            }

            // Validation checks for required fields
            if (createWarehouseDTO.Id == null)
                throw new Exception("The Id field cannot be null.");
            if (createWarehouseDTO.ItemCode == null)
                throw new Exception("The ItemCode field cannot be null.");
            if (createWarehouseDTO.ItemMonth == null)
                throw new Exception("The ItemMonth field cannot be null.");

            // Map CreateWarehouseDTO to EnjoylifeItems
            var enjoylifeItem = new EnjoylifeItems
            {
                Id = createWarehouseDTO.Id, // Assuming Id is nullable in the DTO but required in the entity
                ItemCode = createWarehouseDTO.ItemCode,
                ItemCounts = createWarehouseDTO.ItemCounts,
                ItemDateRegistration = createWarehouseDTO.ItemDateRegistration,
                ItemDescription = createWarehouseDTO.ItemDescription,
                ItemGroup = createWarehouseDTO.ItemGroup,
                ItemMonth = createWarehouseDTO.ItemMonth,
                ItemName = createWarehouseDTO.ItemName,
                ItemStatus = createWarehouseDTO.ItemStatus,
                ItemUnit = createWarehouseDTO.ItemUnit
            };

            // Save the entity to the database
            await _context.Warehouse.AddAsync(enjoylifeItem);
            await _context.SaveChangesAsync();

            // Return the original DTO after saving
            return createWarehouseDTO;
        }


        public async Task<IActionResult> GetItemListByCodeAsync(int itemListCode)
        {
            try
            {
                var itemList = await _context.WarehouseItemLists.FindAsync(itemListCode);

                if (itemList == null)
                {
                    // Returning a simple object instead of NotFoundResult
                    return new JsonResult(new { error = "Item not found" })
                    {
                        StatusCode = StatusCodes.Status404NotFound // Optional: specify the status code
                    };
                }

                // Create the response object
                var response = new
                {
                    itemListCode = itemList.ItemListCode,
                    group = itemList.Group,
                    name = itemList.Name,
                    unit = itemList.Unit
                };

                // Return the response directly
                return new JsonResult(response)
                {
                    StatusCode = StatusCodes.Status200OK // Explicitly set the status code
                };
            }
            catch (Exception ex)
            {
                // Log the error
                // _logger.LogError(ex, "An error occurred while retrieving item list with code {ItemListCode}", itemListCode);

                return new JsonResult(new { error = "Internal server error" })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<IActionResult> GetWarehouseItemByItemCodeAsync(int warehouseItemCode)
        {
            try
            {
                // Await the asynchronous FindAsync call
                var warehouseItem = await _context.Warehouse.FindAsync(warehouseItemCode);

                if (warehouseItem == null)
                {
                    // Returning a JSON result with 404 Not Found status
                    return new JsonResult(new { error = "Item not found" })
                    {
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                // Map the entity to the response DTO
                var response = new CreateWarehouseDTO
                {
                    Id = warehouseItem.Id,
                    ItemCode = warehouseItem.ItemCode,
                    ItemName = warehouseItem.ItemName,
                    ItemDescription = warehouseItem.ItemDescription,
                    ItemCounts = warehouseItem.ItemCounts,
                    ItemStatus = warehouseItem.ItemStatus,
                    ItemDateRegistration = warehouseItem.ItemDateRegistration,
                    ItemGroup = warehouseItem.ItemGroup,
                    ItemUnit = warehouseItem.ItemUnit,
                    ItemMonth = warehouseItem.ItemMonth
                };

                // Return the response with a 200 OK status
                return new JsonResult(response)
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                // Return a JSON result for internal server errors
                return new JsonResult(new { error = "Internal server error" })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        public async Task<IActionResult> GetItemsAsync()
        {
            try
            {
                var items = await _context.WarehouseItemLists
                    .Where(m => m.ItemListCode > 0)
                    .ToListAsync();

                // Return items directly without Ok or NotFound
                var response = new
                {
                    data = items,
                    count = items.Count
                };

                return new JsonResult(response); // Returns a JSON response with items and count
            }
            catch (Exception ex)
            {
                // Return 500 status with error message
                return new JsonResult(new { error = ex.Message }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }


        public async Task<IActionResult> GetWarehousesAsync()
        {
            try
            {
                var result = await _context.Warehouse
                    .Where(n => n.Id > 0)
                    .ToListAsync();

                var response = new
                {
                    data = result,
                    count = result.Count
                };

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                // Return 500 status with error message
                return new JsonResult(new { error = ex.Message }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }


        // Update an existing ItemList by its CODE
        public async Task<ActionResult<ItemList>> UpdateItemListByCodeAsync(int itemListCode, CreateItemListDTO updateItemListDTO)
        {
            var itemList = await _context.WarehouseItemLists.FindAsync(itemListCode);
            if (itemList == null)
            {
                return new NotFoundResult();
            }

            // Update fields
            itemList.Name = updateItemListDTO.Name;
            itemList.Group = updateItemListDTO.Group;
            itemList.Unit = updateItemListDTO.Unit;

            _context.WarehouseItemLists.Update(itemList);
            await _context.SaveChangesAsync();

            return new OkObjectResult(itemList);
        }

        // Update as existing item of te table Warehouse :
        public async Task<ActionResult<EnjoylifeItems>> UpdateWarehouseByCodeAsync(int warehouseId, CreateWarehouseDTO updateWarehouseDTO)
        {
            var result = await _context.Warehouse.FindAsync(warehouseId);
            if (result == null)
            {
                return new NotFoundResult();
            }

            // Update fields
            result.Id = updateWarehouseDTO.Id;
            result.ItemCode = updateWarehouseDTO.ItemCode;
            result.ItemName = updateWarehouseDTO.ItemName;
            result.ItemDescription = updateWarehouseDTO.ItemDescription;
            result.ItemCounts = updateWarehouseDTO.ItemCounts;
            result.ItemDateRegistration = updateWarehouseDTO.ItemDateRegistration;
            result.ItemStatus = updateWarehouseDTO.ItemStatus;
            result.ItemGroup = updateWarehouseDTO.ItemGroup;
            result.ItemUnit = updateWarehouseDTO.ItemUnit;
            result.ItemMonth = updateWarehouseDTO.ItemMonth;

            _context.Warehouse.Update(result);
            await _context.SaveChangesAsync();

            return new OkObjectResult(result);
        }

        // Delete an ItemList by its CODE
        public async Task<IActionResult> DeleteItemListByCodeAsync(int itemListCode)
        {
            var itemList = await _context.WarehouseItemLists.FindAsync(itemListCode);
            if (itemList == null)
            {
                return new NotFoundResult();
            }

            _context.WarehouseItemLists.Remove(itemList);
            await _context.SaveChangesAsync();

            return new OkResult();
        }

        // delete an item from warehouse table :
        public async Task<IActionResult> DeleteWarehouseByIdAsync(int warehouseId)
        {
            var result = await _context.Warehouse.FirstOrDefaultAsync(m => m.Id == warehouseId);
            if (result == null) 
            {
                return new NotFoundResult();
            }
            _context.Warehouse.Remove(result);
            await _context.SaveChangesAsync();

            return new OkResult();
        }

        // upload :

        public async Task UploadAsync(List<CreateItemListDTO> requestList)
        {
            if (requestList == null || !requestList.Any())
            {
                throw new ArgumentNullException(nameof(requestList), "ItemList data cannot be null or empty");
            }

            var itemListEntities = new List<ItemList>();

            foreach (var createItemListDTO in requestList)
            {
                var itemList = new ItemList
                {
                    ItemListCode = createItemListDTO.ItemListCode,
                    Name = createItemListDTO.Name,
                    Group = createItemListDTO.Group,
                    Unit = createItemListDTO.Unit,
                    // Map other fields from CreateItemListDTO as needed
                };

                itemListEntities.Add(itemList);
            }

            await _context.WarehouseItemLists.AddRangeAsync(itemListEntities);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Units
        public async Task<IActionResult> CreateUnitsAsync([FromBody] CreateUnitsDTO createUnitsDTO)
        {
            // Check if the DTO is null and return a NotFound result if it is
            if (createUnitsDTO == null)
            {
                return new NotFoundResult();
            }

            // Use the database context to add the new unit
            using (var context = _context)
            {
                var unit = new Units
                {
                    Id = createUnitsDTO.Id,
                    Name = createUnitsDTO.Name
                    // Add other properties as needed
                };

                // Add the unit to the context
                await context.UnitsWarehouse.AddRangeAsync(unit);

                // Save changes to the database
                await context.SaveChangesAsync();
            }

            // Return an Ok or CreatedAtAction response indicating success
            return new OkObjectResult("Unit/s created successfully.");
        }
        public async Task<IActionResult> CreateGroupsAsync([FromBody] CreateGroupsDTO createGroupsDTO)
        {
            // Check if the DTO is null and return a NotFound result if it is
            if (createGroupsDTO == null)
            {
                return new NotFoundResult();
            }

            // Use the database context to add the new unit
            using (var context = _context)
            {
                var group = new Groups
                {
                    Id = createGroupsDTO.Id,
                    Name = createGroupsDTO.Name
                    // Add other properties as needed
                };

                // Add the unit to the context
                await context.GroupsWarehouse.AddRangeAsync(group);

                // Save changes to the database
                await context.SaveChangesAsync();
            }

            // Return an Ok or CreatedAtAction response indicating success
            return new OkObjectResult("Group/s created successfully.");
        }
        public async Task<IActionResult> GetUnitsAsync()
        {
            try
            {
                var result = await _context.UnitsWarehouse
                    .Where(n => n.Id > 0)
                    .ToListAsync();

                var response = new
                {
                    data = result,
                    count = result.Count
                };

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                // Return 500 status with error message
                return new JsonResult(new { error = ex.Message }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
        public async Task<IActionResult> GetGroupsAsync()
        {
            try
            {
                var result = await _context.GroupsWarehouse
                    .Where(n => n.Id > 0)
                    .ToListAsync();

                var response = new
                {
                    data = result,
                    count = result.Count
                };

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                // Return 500 status with error message
                return new JsonResult(new { error = ex.Message }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
        public async Task<IActionResult> GetUnitById(int unitId)
        {
            try
            {
                var result = await _context.UnitsWarehouse.FirstOrDefaultAsync(n => n.Id == unitId);
                if (result == null)
                    return new JsonResult(new { status = StatusCodes.Status500InternalServerError });
                return new JsonResult(result);
            } catch (Exception ex)
            {
                return new JsonResult(new {error = ex.Message}) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
        public async Task<IActionResult> GetGroupById(int groupId)
        {
            try
            {
                var result = await _context.GroupsWarehouse.FirstOrDefaultAsync(m => m.Id == groupId);
                if (result == null)
                    return new JsonResult(new {status = StatusCodes.Status500InternalServerError });
                return new JsonResult(result);
            } catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
        public async Task<IActionResult> UpdateUnit(int unitId, [FromBody] ManageUnitsDTO units)
        {
            try
            {
                var updated = await _context.UnitsWarehouse.FirstOrDefaultAsync(u => u.Id == unitId);
                if (updated == null)
                    return new JsonResult(new { status = StatusCodes.Status500InternalServerError });
                //updated.Id = units.Id;
                updated.Name = units.Name; // update the name of the unit ...
                await _context.SaveChangesAsync();
                return new NoContentResult();
            } catch (Exception ex)
            {
                return new JsonResult($"{ex.Message}");
            }
        }
        public async Task<IActionResult> UpdateGroup(int groupId, [FromBody] ManageGroupsDTO groups)
        {
            try
            {
                var updated = await _context.GroupsWarehouse.FirstOrDefaultAsync(u => u.Id == groupId);
                if (updated == null)
                    return new JsonResult(new { status = StatusCodes.Status500InternalServerError });
                //updated.Id = groups.Id;
                updated.Name = groups.Name;
                await _context.SaveChangesAsync();
                return new NoContentResult();
            } catch (Exception ex)
            {
                return new JsonResult($"{ex.Message}");
            }
        }
        public async Task<IActionResult> DeleteUG(int ugId, string formCheck)
        {
            try
            {
                if (string.IsNullOrEmpty(formCheck))
                    return new BadRequestResult(); // Bad Request for null or empty formCheck

                if (formCheck == "unitS")
                {
                    var deleted = await _context.UnitsWarehouse.FirstOrDefaultAsync(del => del.Id == ugId);
                    if (deleted == null)
                        return new NotFoundResult(); // Not Found if the unit is not in the database

                    _context.UnitsWarehouse.Remove(deleted);
                }
                else if (formCheck == "groupS")
                {
                    var deleted = await _context.GroupsWarehouse.FirstOrDefaultAsync(del => del.Id == ugId);
                    if (deleted == null)
                        return new NotFoundResult(); // Not Found if the group is not in the database

                    _context.GroupsWarehouse.Remove(deleted);
                }
                else
                {
                    return new BadRequestResult(); // Bad Request for unrecognized formCheck values
                }

                await _context.SaveChangesAsync(); // Save the changes after deletion
                return new OkResult(); // Successful deletion
            }
            catch (Exception ex)
            {
                // Optionally log the exception for debugging purposes
                return new ObjectResult(ex.Message);
            }
        }
        #endregion
    }
}
