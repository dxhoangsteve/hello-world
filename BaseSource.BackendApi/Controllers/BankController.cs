using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BaseSource.Data.EF;
using BaseSource.Data.Entities;
using BaseSource.ViewModels.Common;
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using BaseSource.BackendApi.Services;
using BaseSource.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using BaseSource.ViewModels.Shared;

namespace BaseSource.BackendApi.Controllers
{
    public class BankController : BaseApiController
    {
        private readonly BaseSourceDbContext _db;
        private readonly IVietQrService _vietQrService;

        public BankController(BaseSourceDbContext db,
            IVietQrService vietQrService)
        {
            _db = db;
            _vietQrService = vietQrService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            if (!await _db.Banks.AnyAsync())
            {
                var bankList = await _vietQrService.GetBanks();
                if (bankList.IsSuccessed)
                {
                    var banks = bankList.ResultObj.Select(x => new Bank()
                    {
                        id = x.id.ToString(),
                        name = x.name,
                        code = x.code,
                        bin = x.bin,
                        shortName = x.shortName,
                        logo = x.logo,
                        swift_code = x.swift_code,
                        IsEnabled = true
                    }).ToList();
                    _db.Banks.AddRange(banks);
                    await _db.SaveChangesAsync();
                }
            }

            var model = await _db.Banks
                .Where(x => x.IsEnabled)
                .Select(x => new BankInfoVm()
                {
                    id = x.id,
                    name = "[" + x.code + "] " + x.shortName + " - " + x.name,
                    shortName = x.shortName,
                    code = x.code,
                    bin = x.bin,
                    logo = x.logo,
                    swift_code = x.swift_code
                }).ToListAsync();

            return Ok(new ApiSuccessResult<List<BankInfoVm>>(model));
        }
    }
}
