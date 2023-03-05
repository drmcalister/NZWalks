using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        private readonly NZWalksDbContext _context;  

        public WalkRepository(NZWalksDbContext context)
        {
            _context = context;
        }
        public async Task<Walk> AddAsync(Walk walk)
        {
            walk.Id = Guid.NewGuid();

            await _context.AddAsync(walk);
            await _context.SaveChangesAsync();

            return walk;
        }

        public async Task<Walk> DeleteAsync(Guid id)
        {
            var walk = await _context.Walks.FirstOrDefaultAsync(x => x.Id == id);
            if (walk == null)
            {
                return null;
            }

            _context.Walks.Remove(walk);
            await _context.SaveChangesAsync();

            return walk;

        }

        public async Task<Walk> UpdateAsync(Guid id, Walk walk)
        {
            var existingWalk =  await _context.Walks.FirstOrDefaultAsync(x => x.Id == id);
            if(existingWalk==null)
            {
                return null;
            }

            existingWalk.WalkDifficultyId = walk.WalkDifficultyId;
            existingWalk.Length = walk.Length;
            existingWalk.RegionId = walk.RegionId;
            existingWalk.Name = walk.Name;
         
            await _context.SaveChangesAsync();

            return existingWalk;

        }

        public async Task<IEnumerable<Walk>> GetAllAsync()
        {
            return await 
                _context.Walks
                .Include(x => x.Region)
                .Include(x => x.WalkDifficulty)
                .ToListAsync();
        }

        public async Task<Walk> GetAsync(Guid id)
        {
            return await
                _context.Walks
                .Include(x => x.Region)
                .Include(x => x.WalkDifficulty)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
       

    }
}
