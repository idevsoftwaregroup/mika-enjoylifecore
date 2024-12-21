using core.application.Contract.API.DTO.Warehouse;
using core.domain.DomainModelDTOs.WarehouseDTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace core.application.Contract.infrastructure
{
    public interface IWarehouseRepository
    {
        // Method to create an ItemList in the database :
        Task<ItemList> CreateItemListAsync(ItemList itemList);
        // Method to create a set of insert into the Warehouse table :
        Task<CreateWarehouseDTO> CreateWarehouseAsync(CreateWarehouseDTO createWarehouseDTO);
        // get a record of the list item table in this query method :
        Task<IActionResult> GetItemListByCodeAsync(int itemListCode);
        // get an item of the table Warehouse :
        Task<IActionResult> GetWarehouseItemByItemCodeAsync(int warehouseItemCode);
        // get all of the list items of the tabler ListItems :
        Task<IActionResult> GetItemsAsync();
        // get all Warehouse items :
        Task<IActionResult> GetWarehousesAsync();
        // to update a record of the table of List Items we are about using this code :
        Task<ActionResult<ItemList>> UpdateItemListByCodeAsync(int itemListCode, CreateItemListDTO updateItemListDTO);
        // to update the record of the table Warehouse :
        Task<ActionResult<EnjoylifeItems>> UpdateWarehouseByCodeAsync(int itemListCode, CreateWarehouseDTO updateWarehouseDTO);
        // delete a record by this method :
        Task<IActionResult> DeleteItemListByCodeAsync(int itemListCode);
        // delete an item from warehouse :
        Task<IActionResult> DeleteWarehouseByIdAsync(int warehouseId);
        // you will be able to upload excel file by this method :
        Task UploadAsync(List<CreateItemListDTO> requestList);

        // Units and Groups :

        // Create new Unit/s :
        Task<IActionResult> CreateUnitsAsync([FromBody] CreateUnitsDTO createUnitsDTO);
        Task<IActionResult> CreateGroupsAsync([FromBody] CreateGroupsDTO createGroupsDTO);
        Task<IActionResult> GetUnitsAsync();
        Task<IActionResult> GetGroupsAsync();
        Task<IActionResult> GetUnitById(int unitId);
        Task<IActionResult> GetGroupById(int groupId);
        Task<IActionResult> UpdateUnit(int unitId, [FromBody] ManageUnitsDTO units);
        Task<IActionResult> UpdateGroup(int groupId, [FromBody] ManageGroupsDTO groups);
        Task<IActionResult> DeleteUG(int ugId, string formCheck);
    }
}
