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
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;
        public TransactionController(ITransactionService transactionService,IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Transaction> transactions = _transactionService.GetAllTransaction();
            List<TransactionSendDTO> transactionSendDTOs = _mapper.Map<List<TransactionSendDTO>>(transactions);
            return Ok(transactionSendDTOs);
        }
        [HttpGet("{id:Guid}")]
        public IActionResult Get(Guid id)
        {
            try
            {
                Transaction transaction = _transactionService.GetTransaction(id);
                TransactionSendDTO transactionSendDTO = _mapper.Map<TransactionSendDTO>(transaction);
                return Ok(transactionSendDTO);
            }
            catch (TransactionNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message, statusCode = (int)HttpStatusCode.NotFound });
            }
        }
        [HttpPost]
        public IActionResult AddTransaction([FromBody] TransactionAddDTO transactionAddDTO)
        {
            try
            {
                var transaction = _mapper.Map<Transaction>(transactionAddDTO);
                transaction.TransactionDateTime = DateTime.Now;
                bool result = _transactionService.AddTransaction(transaction);
                if (result)
                {
                    return CreatedAtAction(nameof(GetAll), new { id = transaction.TransactionId },
                        new { message = $"Transaction added successfully with id : {transaction.TransactionId}" });
                }
                else
                {
                    return BadRequest(new { message = "Failed to add transaction" });
                }
            }
            catch(InvalidTransactionException ex)
            {
                return BadRequest(new { message = ex.Message, statusCode = (int)HttpStatusCode.BadRequest });
            }
        }
        [HttpPost("transafer")]
        public IActionResult TransferFunds([FromBody] TransferFundsDTO transferFundsDto)
        {
            try
            {
                var success = _transactionService.TransferFunds(
                    transferFundsDto.AccountFrom,
                    transferFundsDto.AccountTo,
                    transferFundsDto.Amount

                    );
                if (success)
                {
                    return Ok("Transaction successful.");
                }
                return BadRequest("Transaction failed.");
            }
            catch (NotEnoughBalanceException ex)
            {
                return BadRequest(new { message = ex.Message, statusCode = (int)HttpStatusCode.BadRequest });
            }
            catch(AccountNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message, statusCode = (int)HttpStatusCode.NotFound });
            }
        }

        [HttpGet("account/{accountNumber}")]
        public IActionResult GetTransactionsByAccountNumber(string accountNumber)
        {
            try
            {
                var transactions = _transactionService.GetTransactionsByAccountNumber(accountNumber);
                if (transactions == null || transactions.Count == 0)
                {
                    return NotFound(new { message = "No transactions found for this account.", statusCode = (int)HttpStatusCode.NotFound });
                }

                var transactionSendDTOs = _mapper.Map<List<TransactionSendDTO>>(transactions);
                return Ok(transactionSendDTOs);
            }
            catch (AccountNotFoundException ex)
            {
                return NotFound(new { message = ex.Message, statusCode = (int)HttpStatusCode.NotFound });
            }
        }
        [HttpGet("{accountNumber}/{months}")]
        public IActionResult GetTransactionsByAccountNumberAndDateRange(string accountNumber, int months)
        {
            try
            {
                var transactions = _transactionService.GetTransactionsByAccountNumberAndDateRange(accountNumber, months);
                return Ok(transactions);
            }
            catch (AccountNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (TransactionNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching transactions.", error = ex.Message });
            }
        }
    }
}
