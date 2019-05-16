using System.Collections.Generic;
using System.Threading.Tasks;
using lab4.Interfaces;
using lab4.Models;
using Microsoft.AspNetCore.Mvc;

namespace lab4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LuceneController : ControllerBase
    {
        private ISourceService _sourceService { get; set; }
        private ILuceneService _luceneService { get; set; }
        
        public LuceneController(ILuceneService luceneService, ISourceService sourceService)
        {
            _luceneService = luceneService;
            _sourceService = sourceService;
        }
        [HttpGet]
        [Route(Routes.Search)]
        public async ValueTask<IEnumerable<string>> Search(string text, bool viaLucene)
        {
            var result = await _luceneService.Search(text, viaLucene);
            return result;
        }
        
        [HttpGet]
        [Route(Routes.SetDatabase)]
        public async ValueTask<bool> SetDb()
        {
            var result = await _sourceService.SetDatabase();
            return result;
        }
    }
}