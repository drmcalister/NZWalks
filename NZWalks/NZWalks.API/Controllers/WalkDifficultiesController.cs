using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Xml.Linq;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalkDifficultiesController : Controller
    {
        private readonly IWalkDifficultyRepository _walkDifficultyRepository;
        private readonly IMapper _mapper;

        public WalkDifficultiesController(IWalkDifficultyRepository walkDifficultyRepository, IMapper mapper)
        {
            _walkDifficultyRepository = walkDifficultyRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalkDifficulties()
        {
            var walkDifficulties = await _walkDifficultyRepository.GetAllAsync();
            var walkDifficultiesDTO = _mapper.Map<List<WalkDifficulty>>(walkDifficulties);

            return Ok(walkDifficultiesDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetWalkDifficultyById")]
        public async Task<IActionResult> GetWalkDifficultyById(Guid id)
        {
            var walkDifficulty = await _walkDifficultyRepository.GetAsync(id);
            if (walkDifficulty==null)
            {
                return NotFound();
            }

            // Convert Domain to DTO
            var walkDifficultyDTO = _mapper.Map<Models.DTO.WalkDifficulty>(walkDifficulty);
            return Ok(walkDifficultyDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddWalkDifficultyAsync(AddWalkDifficultyRequest addWalkDifficultyRequest)
        {
            // Validate Request
            if (!ValidateAddWalkDifficulty(addWalkDifficultyRequest))
            {
                return BadRequest(ModelState);
            }

            // Convert DTO to Domain
            var walkDifficulty = new Models.Domain.WalkDifficulty
            {
                Code = addWalkDifficultyRequest.Code
            };

            // Call Repository
            walkDifficulty = await _walkDifficultyRepository.AddAsync(walkDifficulty);

            var walkDifficultyDTO = _mapper.Map<Models.DTO.WalkDifficulty>(walkDifficulty);

            return CreatedAtAction(nameof(GetWalkDifficultyById),
                new { id = walkDifficultyDTO.Id }, walkDifficultyDTO);

        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalkDifficultyAsync(Guid id,
            UpdateWalkDifficultyRequest updateWalkDifficultyRequest )
        {
            // Validate Request
            if (!ValidateUpdateWalkDifficulty(updateWalkDifficultyRequest))
            {
                return BadRequest(ModelState);
            }

            // Convert DTO to Domain model
            var walkDifficulty = new Models.Domain.WalkDifficulty
            {
                Code = updateWalkDifficultyRequest.Code
            };

            // Call Repository to update
            walkDifficulty = await _walkDifficultyRepository.UpdateAsync(id, walkDifficulty);
            if(walkDifficulty == null)
            { 
                return NotFound();
            }

            var walkDifficultyDTO = _mapper.Map<Models.DTO.WalkDifficulty>(walkDifficulty);
            return Ok(walkDifficultyDTO);   
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalkDifficulty(Guid id)
        {
            var walkDifficulty = await _walkDifficultyRepository.DeleteAsync(id);
            if (walkDifficulty == null)
            {
                return NotFound();
            }

            var walkDifficultyDTO = _mapper.Map<Models.DTO.WalkDifficulty>(walkDifficulty);
            return Ok(walkDifficultyDTO);
        }

        #region Private methods 
        private bool ValidateAddWalkDifficulty(AddWalkDifficultyRequest addWalkDifficulty)
        {

            if (addWalkDifficulty == null)
            {
                ModelState.AddModelError(nameof(addWalkDifficulty), $"Add region data is required");
                return false;
            }

            if(string.IsNullOrWhiteSpace(addWalkDifficulty.Code))
            {
                ModelState.AddModelError(nameof(addWalkDifficulty), $"{nameof(addWalkDifficulty)} is required and cannot be null or empty.");
            }

            if (ModelState.ErrorCount > 0)
            {
                return false;
            }

            return true;
        }
        private bool ValidateUpdateWalkDifficulty(UpdateWalkDifficultyRequest updateWalkDifficulty)
        {

            if (updateWalkDifficulty == null)
            {
                ModelState.AddModelError(nameof(updateWalkDifficulty), $"Add region data is required");
                return false;
            }

            if (string.IsNullOrWhiteSpace(updateWalkDifficulty.Code))
            {
                ModelState.AddModelError(nameof(updateWalkDifficulty), $"{nameof(updateWalkDifficulty)} is required and cannot be null or empty.");
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
