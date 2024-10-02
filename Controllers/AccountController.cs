using AutoMapper;
using BankApp.DTOs;
using BankApp.Exceptions;
using BankApp.Models;
using BankApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BankApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        public AccountController(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Account> accounts = _accountService.GetAllAccounts();
            List<AccountSendDTO> accountSendDTOs = _mapper.Map<List<AccountSendDTO>>(accounts);
            return Ok(accountSendDTOs);
        }
        [HttpGet("{id:Guid}")]
        public IActionResult GetByAccountID(Guid id)
        {
            try
            {
                Account getAccount = _accountService.GetAccountById(id);
                AccountSendDTO accountSendDTO = _mapper.Map<AccountSendDTO>(getAccount);
                return Ok(accountSendDTO);
            }
            catch (AccountNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message, statusCode = (int)HttpStatusCode.NotFound });
            }
        }
        
        [HttpGet("{acc_no}")]
        public IActionResult GetByAccNo(string acc_no)
        {
            try
            {
                var getAccount = _accountService.GetAccountByNumber(acc_no);
                AccountSendDTO accountSendDTO = _mapper.Map<AccountSendDTO>(getAccount);
                return Ok(accountSendDTO);
            }
            catch (AccountNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message, statusCode = (int)HttpStatusCode.NotFound });
            }
        }

        [HttpGet("user/{userid:Guid}")]
        public IActionResult GetByAccountUserID(Guid userid)
        {
            try
            {
                var getAccount = _accountService.GetAccountByUserId(userid);
                var accountSendDTO = _mapper.Map<List<AccountSendDTO>>(getAccount);
                return Ok(accountSendDTO);
            }
            catch (AccountNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message, statusCode = (int)HttpStatusCode.NotFound });
            }
        }


        [HttpDelete("{accNo}")]
        public IActionResult Deactivate(string accNo)
        {
            try
            {
                var getAccount = _accountService.GetAccountByNumber(accNo);
                _accountService.DeleteAccount(getAccount.AccountId);
                return Ok("Account deactivated successfully");
            }
            catch (AccountNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message, statusCode = (int)HttpStatusCode.NotFound });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddAccount([FromBody]AccountAddDTO accountAddDTO)
        {
            try
            {
                var accountDetails = _mapper.Map<Account>(accountAddDTO);
                _accountService.AddAccount(accountDetails);
                return CreatedAtAction(nameof(GetAll), new { id = accountDetails.AccountId },
                        new { message = $"Account created successfully with account id : {accountDetails.AccountId} and account number : {accountDetails.AccountNumber}" });
            }
            catch (InvalidAccountException ex)
            {
                return BadRequest(new { message = ex.Message, statusCode = (int)HttpStatusCode.BadRequest });
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAccount([FromBody] AccountUpdateDTO accountUpdateDTO)
        {
            try
            {
                if(accountUpdateDTO.AccountBalance < 0)
                {
                    throw new InvalidAccountException("Account balance is invalid");
                }
                var account = _mapper.Map<Account>(accountUpdateDTO);
                _accountService.UpdateAccount(account);
                return Ok(new { message = "Account updated successfully." });
            }
            catch(AccountNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message, statusCode = (int)HttpStatusCode.NotFound });
            }
            catch(InvalidAccountException ex)
            {
                return BadRequest(new { message = ex.Message, statusCode = (int)HttpStatusCode.BadRequest });
            }
            
        }

    }
}
