using ClosedXML.Excel;
using core.application.Contract.API.DTO.Expense;
using core.application.Contract.API.DTO.Warehouse;
using core.application.Contract.API.Interfaces;
using core.domain.DomainModelDTOs.WarehouseDTOs;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Packaging;
using IdentityProvider.Application.Framework;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace core.api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        // POST: api/Warehouse/CreateItemList
        [HttpPost("CreateItemList")]
        public async Task<IActionResult> CreateItemList([FromBody] CreateItemListDTO itemList)
        {
            try
            {
                var result = await _warehouseService.CreateItemListAsync(itemList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // POST: api/Warehouse/CreateWarehouse
        [HttpPost("CreateWarehouse")]
        public async Task<IActionResult> CreateWarehouse([FromBody] CreateWarehouseDTO createWarehouseDTO)
        {
            try
            {
                var create = _warehouseService.CreateWarehouse(createWarehouseDTO);
                return Ok(create);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // GET: api/Warehouse/GetItemListByCode/{itemListCode}
        [HttpGet("GetItemListByCode/{itemListCode}")]
        public async Task<IActionResult> GetItemListByCode(int itemListCode)
        {
            try
            {
                var itemList = await _warehouseService.GetItemListByCodeAsync(itemListCode);
                return Ok(itemList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Warehouse/GetWarehouseItemByItemCode
        [HttpGet("GetWarehouseItemByItemCode")]
        public async Task<IActionResult> GetWarehouseItemByItemCode(int warehouseItemCode)
        {
            try
            {
                var result = await _warehouseService.GetWarehouseItemByItemCode(warehouseItemCode);
                return Ok(result);
            } catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Warehouse/GetItems
        [HttpGet("GetItems")]
        public async Task<IActionResult> GetItems()
        {
            try
            {
                var result = await _warehouseService.GetItems();
                return new JsonResult(result); // Returns JSON without Ok
            }
            catch (Exception ex)
            {
                // Catch the exception and return a BadRequest result with the message
                return new JsonResult(new { error = ex.Message }) 
                { 
                    StatusCode = StatusCodes.Status400BadRequest 
                };
            }
        }

        // GET: api/Warehouse/GetWarehouses
        [HttpGet("GetWarehouses")]
        public async Task<IActionResult> GetWarehouses()
        {
            try
            {
                var result = await _warehouseService.GetWarehouses(); // Ensure this is awaited if it's async

                // If the result is null or empty, return a NotFound result
                if (result == null)
                {
                    return NotFound("No warehouses found.");
                }

                // Return the result directly with an Ok response
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return BadRequest($"Error: {ex.Message}");
            }
        }


        // PUT: api/Warehouse/UpdateItemListByCode/{itemListCode}
        [HttpPut("UpdateItemListByCode/{itemListCode}")]
        public async Task<IActionResult> UpdateItemListByCode(int itemListCode, [FromBody] CreateItemListDTO updateItemListDTO)
        {
            try
            {
                var result = await _warehouseService.UpdateItemListByCodeAsync(itemListCode, updateItemListDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Warehouse/UpdateWarehouse/{warhouseItemCode}
        [HttpPut("UpdateWarehouse/{warehouseId}")]
        public async Task<IActionResult> UpdateWarehouse(int warehouseId, [FromBody] CreateWarehouseDTO createWarehouseDTO)
        {
            try
            {
                var result = await _warehouseService.UpdateWarehouseAsync(warehouseId, createWarehouseDTO);
                return Ok(result);
            } catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // DELETE: api/Warehouse/DeleteItemListByCode/{itemListCode}
        [HttpDelete("DeleteItemListByCode/{itemListCode}")]
        public async Task<IActionResult> DeleteItemListByCode(int itemListCode)
        {
            await _warehouseService.DeleteItemListByCodeAsync(itemListCode);
            return NoContent();
        }

        // DELETE: api/Warehouse/DeleteWarehouseByCode/{warehouseItemCode}
        [HttpDelete("DeleteWarehouseById/{warehouseId}")]
        public async Task<IActionResult> DeleteWarehouseById(int warehouseId)
        {
            await _warehouseService.DeleteWarehouseByIdAsync(warehouseId);
            return NoContent();
        }

        // Services for Units and Groups

        // CREATE: api/Warehouse/CreateUnits
        [HttpPost("CreateUnits")]
        public async Task<IActionResult> CreateUnits([FromBody] CreateUnitsDTO createUnitsDTO)
        {
            try
            {
                var createdUnit = await _warehouseService.CreateUnits(createUnitsDTO);
                if (createdUnit == null)
                {
                    return BadRequest("Unit creation failed.");
                }
                return Ok("Unit created successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception (for debugging purposes)
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // CREATE: api/Warehouse/CreateGroups
        [HttpPost("CreateGroups")]
        public async Task<IActionResult> CreateGroups([FromBody] CreateGroupsDTO createGroupsDTO)
        {
            try
            {
                var createdGroups = await _warehouseService.CreateGroups(createGroupsDTO);
                if (createdGroups == null)
                {
                    return BadRequest("Group creation failed.");
                }
                return Ok("Group created successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception (for debugging purposes)
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // GET: api/Warehouse/GetUnits
        [HttpGet("GetUnits")]
        public async Task<IActionResult> GetUnits()
        {
            try
            {
                var result = await _warehouseService.GetUnits(); // Ensure this is awaited if it's async

                // If the result is null or empty, return a NotFound result
                if (result == null)
                {
                    return NotFound("No Unit/s found.");
                }

                // Return the result directly with an Ok response
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return BadRequest($"Error: {ex.Message}");
            }
        }
        // GET: api/Warehouse/GetGroups
        [HttpGet("GetGroups")]
        public async Task<IActionResult> GetGroups()
        {
            try
            {
                var result = await _warehouseService.GetGroups(); // Ensure this is awaited if it's async

                // If the result is null or empty, return a NotFound result
                if (result == null)
                {
                    return NotFound("No Unit/s found.");
                }

                // Return the result directly with an Ok response
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return BadRequest($"Error: {ex.Message}");
            }
        }
        [HttpGet("GetUnitById/{unitId}")]
        public async Task<IActionResult> GetUnitById(int unitId)
        {
            var result = await _warehouseService.GetUnitById(unitId);
            if (result == null)
                return NotFound($"هیچ یونیتی با شماره آیدی {unitId} یافت نشد.");
            return Ok(result);
        }
        [HttpGet("GetGroupById/{groupId}")]
        public async Task<IActionResult> GetGroupById(int groupId)
        {
            var result = await _warehouseService.GetGroupById(groupId);
            // If service returns null, handle NotFound in the controller
            if (result == null)
                return NotFound($"هیچ گروهی با شماره آیدی {groupId} یافت نشد.");
            return Ok(result);
        }
        [HttpPut("UpdateUnit/{unitId}")]
        public async Task<IActionResult> UpdateUnit(int unitId, [FromBody] ManageUnitsDTO units)
        {
            var result = await _warehouseService.UpdateUnit(unitId, units);

            if (result == null)
                return NotFound($"آپدیت یونیت با شماره آیدی {unitId} انجام نشد. لطفا مجددا تلاش کنید.");
            if (result is NoContentResult) 
                return Ok($"یونیت با شماره آیدی {unitId} آپدیت شده است.");
            
            return result;
        }
        [HttpPut("UpdateGroup/{groupId}")]
        public async Task<IActionResult> UpdateGroup(int groupId, [FromBody] ManageGroupsDTO groups)
        {
            var result = await _warehouseService.UpdateGroup(groupId, groups);

            if (result == null)
                return NotFound($"آپدیت گروه با شماره آیدی {groupId}  انجام نشد.");
            if (result is NoContentResult)
                return Ok($"گروه با شماره آیدی {groupId} آپدیت شده است.");
            
            return result;
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteUG(int ugId, string formCheck)
        {
            if (string.IsNullOrEmpty(formCheck))
            {
                return BadRequest($"پارامتر {formCheck} مورد نیاز است.");
            }

            var result = await _warehouseService.DeleteUG(ugId, formCheck);

            if (result is OkResult)
            {
                return Ok($"حذف پارامتر مربوط به {(formCheck == "unitS" ? "یونیت" : "گروه")} با موفقیت انجام شد.");
            }

            return result;
        }
    }
}
