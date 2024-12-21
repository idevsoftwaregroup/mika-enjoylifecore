using Microsoft.AspNetCore.Mvc;
using OrganizationChart.API.Contracts.DTOs;
using OrganizationChart.API.Contracts.Interfaces;
using OrganizationChart.API.Framework;

namespace OrganizationChart.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemDetailsController : ControllerBase
    {
        private readonly ISystemDetails _isystemDetails;
        private readonly ISystemUpsRepository _systemUpsRepository;
        public SystemDetailsController(ISystemDetails isystemDetails, ISystemUpsRepository systemUpsRepository)
        {
            _isystemDetails = isystemDetails;
            _systemUpsRepository = systemUpsRepository;
        }
        [HttpGet("GetUpsProjects")]
        public async Task<ActionResult<OperationResult<GetSystemProjectDTO>>> GetUpsProjects(CancellationToken cancellationToken = default)
        {
            var operation = await _systemUpsRepository.GetUpsProjects(cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetUPSes")]
        public async Task<ActionResult<OperationResult<GetUpsDTO>>> GetUPSes(CancellationToken cancellationToken = default)
        {
            var operation = await _systemUpsRepository.GetUPSes(cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetUPS/{Id}")]
        public async Task<ActionResult<OperationResult<GetUpsDTO>>> GetUPS(int Id, CancellationToken cancellationToken = default)
        {
            var operation = await _systemUpsRepository.GetUPS(Id, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPost("CreateUps")]
        public async Task<ActionResult<OperationResult<object>>> CreateUps(CreateUpsDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _systemUpsRepository.CreateUps(model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPut("UpdateUps")]
        public async Task<ActionResult<OperationResult<object>>> UpdateUps(UpdateUpsDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _systemUpsRepository.UpdateUps(model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpDelete("DeleteUps")]
        public async Task<ActionResult<OperationResult<object>>> DeleteUps(DeleteUpsDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _systemUpsRepository.DeleteUps(model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPost("CreateUpsService")]
        public async Task<ActionResult<OperationResult<object>>> CreateUpsService(CreateUpsServiceDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _systemUpsRepository.CreateUpsService(model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPut("UpdateUpsService")]
        public async Task<ActionResult<OperationResult<object>>> UpdateUpsService(UpdateUpsServiceDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _systemUpsRepository.UpdateUpsService(model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpDelete("DeleteUpsService")]
        public async Task<ActionResult<OperationResult<object>>> DeleteUpsService(DeleteUpsServiceDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _systemUpsRepository.DeleteUpsService(model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetSystemTypes")]
        public async Task<ActionResult<OperationResult<GetSystemTypeDTO>>> GetSystemTypes(CancellationToken cancellationToken = default)
        {
            var operation = await _isystemDetails.GetSystemTypes(cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetSystem/{ComputerName}")]
        public async Task<ActionResult<OperationResult<GetSystemDTO>>> GetSystem(string ComputerName, CancellationToken cancellationToken = default)
        {
            var operation = await _isystemDetails.GetSystem(ComputerName, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }
        [HttpPost("CreateSystem")]
        public async Task<ActionResult<OperationResult<object>>> CreateSystem(CreateSystemDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _isystemDetails.CreateSystem(model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetAllSystems")]
        public async Task<ActionResult<GetSystemDTO>> GetAllSystems(CancellationToken cancellationToken = default)
        {
            return Ok(await _isystemDetails.GetAllSystems(cancellationToken));
        }
        [HttpPut("UpdateSystem")]
        public async Task<ActionResult<OperationResult<GetSystemDTO>>> UpdateSystem(UpdateSystemDetailsDTO requestDTO, CancellationToken cancellationToken = default)
        {
            var operation = await _isystemDetails.UpdateSystem(requestDTO, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }
        [HttpDelete("DeleteSystem")]
        public async Task<ActionResult<OperationResult<object>>> DeleteSystem(SystemIdDTO requestDTO, CancellationToken cancellationToken = default)
        {
            var operation = await _isystemDetails.DeleteSystem(requestDTO, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }


    }
}
