using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace WebSearch.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        /// <summary>
        /// Search properties and managements
        /// </summary>
        /// <param name="query"></param>
        /// <param name="market"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Search([FromHeader] string query, [FromHeader] string market = null, [FromHeader] int size = 25)
        {
            await Task.Delay(1000);
            var response = _searchService.GetData(query, market, size);
            if (response.Any())
            {
                var output = response.Select(x => Mapper.MapFrom(x));
                return Ok(output);
            }

            return NotFound();
        }
    }
}