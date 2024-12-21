using core.application.Contract.API.DTO.Warehouse;
using core.domain.DomainModelDTOs.WarehouseDTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace core.application.Contract.API.Interfaces;

public interface IWarehouseService
{
    // Begin::Warehouse

    // Creates a new item list
    Task<IActionResult> CreateItemListAsync(CreateItemListDTO createItemListDTO);

    // Create main Warehouse for the ListItems of the Enjoylife Warehouse in every buildings :
    Task<IActionResult> CreateWarehouse(CreateWarehouseDTO createWarehouseDTO);

    // Gets an item list by its code
    Task<IActionResult> GetItemListByCodeAsync(int itemListCode);

    //Get as item of the warehosue table :
    Task<IActionResult> GetWarehouseItemByItemCode(int warehouseItemCode);

    // Gets all items
    Task<IActionResult> GetItems();

    // Get All items of Warehouses :
    Task<IActionResult> GetWarehouses();

    // Updates an existing item list by its code
    Task<ActionResult<ItemList>> UpdateItemListByCodeAsync(int itemListCode, CreateItemListDTO updateItemListDTO);

    // Update resord of the table Warehouse :
    Task<ActionResult<EnjoylifeItems>> UpdateWarehouseAsync(int warehouseId, [FromBody] CreateWarehouseDTO createWarehouseDTO);

    // Deletes an item list by its code
    Task<IActionResult> DeleteItemListByCodeAsync(int itemListCode);

    // Delete an item from Warehouse table :
    Task<IActionResult> DeleteWarehouseByIdAsync(int warehouseId);

    // End::Warehouse


    // Begin::Units and Groups

    Task<IActionResult> CreateUnits([FromBody] CreateUnitsDTO createUnitsDTO);
    Task<IActionResult> CreateGroups([FromBody] CreateGroupsDTO createGroupsDTO);
    Task<IActionResult> GetUnits();
    Task<IActionResult> GetGroups();
    Task<IActionResult> GetUnitById(int unitId);
    Task<IActionResult> GetGroupById(int groupId);
    Task<IActionResult> UpdateUnit(int unitId, [FromBody] ManageUnitsDTO units);
    Task<IActionResult> UpdateGroup(int groupId, [FromBody] ManageGroupsDTO groups);
    Task<IActionResult> DeleteUG(int ugId, string formCheck);
}