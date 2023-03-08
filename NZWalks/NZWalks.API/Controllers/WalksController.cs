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
        private readonly IRegionRepository _regionRepository;
        private readonly IWalkDifficultyRepository _walkDifficultyRepository;
        private readonly IMapper _mapper;

        public WalksController(IWalkRepository walkRepository, IRegionRepository regionRepository,
            IWalkDifficultyRepository walkDifficultyRepository, IMapper mapper)
        {
            _walkRepository = walkRepository;
            _regionRepository = regionRepository;
            _walkDifficultyRepository = walkDifficultyRepository;
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
           // validate addWalkRequest
           if(!(await ValidateAddWalkAsync(addWalkRequest)))
           {
                return BadRequest(ModelState);
           }
            
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
            // Validate Request
            if (!(await ValidateUpdateWalkAsync(updateWalkRequest)))
            {
                return BadRequest(ModelState);
            }


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

        #region Private methods

        private async Task<bool> ValidateAddWalkAsync(AddWalkRequest addWalkRequest)
        {
            if(addWalkRequest == null)
            {
                ModelState.AddModelError(nameof(addWalkRequest), $"Add region data is required");
                return false;
            }

            if (string.IsNullOrWhiteSpace(addWalkRequest.Name))
            {
                ModelState.AddModelError(nameof(addWalkRequest.Name), $"{nameof(addWalkRequest.Name)} cannot be null or empty or white space.");
             }

            if(addWalkRequest.Length<=0)
            {
                ModelState.AddModelError(nameof(addWalkRequest.Length), $"{nameof(addWalkRequest.Length)} cannot be zero or less than zero.");
            }

            var region = _regionRepository.GetAsync(addWalkRequest.RegionId);
            if(region==null)
            {
                ModelState.AddModelError(nameof(addWalkRequest.RegionId), $"{nameof(addWalkRequest.RegionId)} is invalid.");
            }

            var walkdifficulty = await _walkDifficultyRepository.GetAsync(addWalkRequest.WalkDifficultyId);
            if (walkdifficulty == null)
            {
                ModelState.AddModelError(nameof(addWalkRequest.WalkDifficultyId), $"{nameof(addWalkRequest.WalkDifficultyId)} is invalid.");
            }


            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> ValidateUpdateWalkAsync(UpdateWalkRequest updateWalkRequest)
        {
            if (updateWalkRequest == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest), $"Add region data is required");
                return false;
            }

            if (string.IsNullOrWhiteSpace(updateWalkRequest.Name))
            {
                ModelState.AddModelError(nameof(updateWalkRequest.Name), $"{nameof(updateWalkRequest.Name)} cannot be null or empty or white space.");
            }

            if (updateWalkRequest.Length <= 0)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.Length), $"{nameof(updateWalkRequest.Length)} cannot be zero or less than zero.");
            }

            var region = _regionRepository.GetAsync(updateWalkRequest.RegionId);
            if (region == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.RegionId), $"{nameof(updateWalkRequest.RegionId)} is invalid.");
            }

            var walkdifficulty = await _walkDifficultyRepository.GetAsync(updateWalkRequest.WalkDifficultyId);
            if (walkdifficulty == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest.WalkDifficultyId), $"{nameof(updateWalkRequest.WalkDifficultyId)} is invalid.");
            }


            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
