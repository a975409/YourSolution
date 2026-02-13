using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Domain.Data;
using YourSolution.Domain.DTOs.Response;
using YourSolution.Domain.Models;

namespace YourSolution.Domain.Repositories
{
    public class IpWhitelistRepository
    {
        private readonly AppDBContext _context;

        public IpWhitelistRepository(AppDBContext fRDBContext)
        {
            _context = fRDBContext;
        }

        public async Task<IEnumerable<IpWhitelistDto>> GetAllWhitelistsAsync()
        {
            return await _context.IpWhitelists
                .Select(w => new IpWhitelistDto
                {
                    Id = w.Id,
                    IpAddress = w.IpAddress,
                    Mask = w.Mask,
                    IsEnable = w.IsEnable
                })
                .ToListAsync();
        }

        public IpWhitelistDto? GetWhitelistById(int id)
        {
            var whitelist = _context.IpWhitelists
                .FirstOrDefault(w => w.Id == id);

            if (whitelist is null)
            {
                return null;
            }

            return new IpWhitelistDto
            {
                Id = whitelist.Id,
                IpAddress = whitelist.IpAddress,
                Mask = whitelist.Mask,
                IsEnable = whitelist.IsEnable
            };
        }

        public async Task AddWhitelistAsync(string creater, IpWhitelistDto whitelistDto)
        {
            var newWhitelist = new IpWhitelist
            {
                IpAddress = whitelistDto.IpAddress,
                Mask = whitelistDto.Mask,
                IsEnable = whitelistDto.IsEnable,
                CreatedDate = DateTime.Now,
                CreatedBy = creater
            };

            _context.IpWhitelists.Add(newWhitelist);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateWhitelistAsync(string editer, IpWhitelistDto whitelistDto)
        {
            var existingWhitelist = _context.IpWhitelists.Find(whitelistDto.Id);

            if (existingWhitelist is not null)
            {
                existingWhitelist.IpAddress = whitelistDto.IpAddress;
                existingWhitelist.Mask = whitelistDto.Mask;
                existingWhitelist.IsEnable = whitelistDto.IsEnable;
                existingWhitelist.ModifiedDate = DateTime.Now;
                existingWhitelist.ModifiedBy = editer;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteWhitelistAsync(int id)
        {
            var whitelistToDelete = _context.IpWhitelists.Find(id);

            if (whitelistToDelete is not null)
            {
                _context.IpWhitelists.Remove(whitelistToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public Task<bool> ExistsByIpAddress(string ipAddress)
        {
            return _context.IpWhitelists
                           .AsNoTracking()
                           .AnyAsync(w => w.IpAddress == ipAddress);
        }
    }
}
