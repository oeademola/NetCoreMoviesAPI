using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecurityController : ControllerBase
    {
        private readonly IDataProtector _protector;
        private readonly HashService hashService;
        public SecurityController(IDataProtectionProvider protectionProvider, HashService hashService)
        {
            this.hashService = hashService;
            _protector = protectionProvider.CreateProtector("value_secret_and_unique");
        }

        [HttpGet]
        public IActionResult GetAction()
        {
            string plaintext = "Oluwaseyi Ademola";
            string encryptedText = _protector.Protect(plaintext);
            string decryptedText = _protector.Unprotect(encryptedText);

            return Ok(new { plaintext, encryptedText, decryptedText });

        }

        [HttpGet("TimeBound")]
        public async Task<IActionResult> GetTimeBound()
        {
            var protectorTimeBound = _protector.ToTimeLimitedDataProtector();
            string plaintext = "Oluwaseyi Ademola";
            string encryptedText = protectorTimeBound.Protect(plaintext, lifetime: TimeSpan.FromSeconds(5));
            await Task.Delay(6000);
            string decryptedText = protectorTimeBound.Unprotect(encryptedText);

            return Ok(new { plaintext, encryptedText, decryptedText });
        }

        [HttpGet("Hash")]
        public IActionResult GetHash()
        {
            var plaintext = "Oluwaseyi Ademola";
            var hashResult1 = hashService.Hash(plaintext);
            var hashResult2 = hashService.Hash(plaintext);
            return Ok(new {plaintext, hashResult1, hashResult2});
        }
    }
}