using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalksController : Controller
    {
        private readonly IWalkRepository _walkRepository;
        private readonly IMapper _mapper;

        public WalksController(IWalkRepository walkRepository, IMapper mapper)
        {
            _walkRepository = walkRepository;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllWalksAsync()
        {
            var walks = await _walkRepository.GetAllAsync();

            var walksDTO = _mapper.Map<List<Models.DTO.Walk>>(walks);

            return Ok(walksDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetWalkAsync")]
        public async Task<IActionResult> GetWalkAsync(Guid id)
        {
            var walk = await _walkRepository.GetAsync(id);

            if (walk == null)
            {
                return NotFound();
            }

            var walkDTO = _mapper.Map<Models.DTO.Walk>(walk);
                       
            return Ok(walkDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddWalkAsync(AddWalkRequest addWalkRequest)
        {
            // convert request to domain model.
            var walk = _mapper.Map<Models.Domain.Walk>(addWalkRequest);
            // pass details to repository
            walk = await _walkRepository.AddAsync(walk);
            // Convert back to DTO
            var walkDTO = _mapper.Map<Models.DTO.Walk>(walk);
           
            return CreatedAtAction(nameof(GetWalkAsync), new {id = walkDTO.Id}, walkDTO);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalkAsync(Guid id)
        {
            // Get walk from database
            var walk = await _walkRepository.DeleteAsync(id);
            // if null NotFound
            if(walk==null)
            {
                return NotFound();
            }
            // Convert Response back to DTO
            var walkDTO = _mapper.Map<Models.DTO.Walk>(walk);
                       
            // return Ok response
            return Ok(walkDTO);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalkAsync([FromRoute] Guid id, [FromBody] UpdateWalkRequest updateWalkRequest )
        {
            // Convert DTO to domain model
            var walk = _mapper.Map<Models.Domain.Walk>(updateWalkRequest);
          
            // Update Region with repository
            walk = await _walkRepository.UpdateAsync(id, walk);
            
            // If null then NotFound
            if (walk == null)
            {
                return NotFound();
            }

            var walkDTO = _mapper.Map<Models.DTO.Walk>(walk);            // Convert Response back to DTO
           
            // return Ok response
            return Ok(walkDTO);
        }
    }
}
