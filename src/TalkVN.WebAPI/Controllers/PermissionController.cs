using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TalkVN.Application.Helpers;
using TalkVN.Application.Models;
using TalkVN.Application.Models.Dtos.Group;
using TalkVN.Application.Services.Interface;
using TalkVN.DataAccess.Data;
using TalkVN.Domain.Entities.SystemEntities.Permissions;

namespace TalkVN.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPermissionService _permissionService;
        private readonly IClaimService _claimService;

        public PermissionController(
            ApplicationDbContext context,
            IPermissionService permissionService,
            IClaimService claimService)
        {
            _context = context;
            _permissionService = permissionService;
            _claimService = claimService;
        }

        // GET: api/Permission
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<List<Permission>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPermissions()
        {
            var permissions = await _context.Permissions
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync();

            return Ok(ApiResult<List<Permission>>.Success(permissions));
        }


    }
}
