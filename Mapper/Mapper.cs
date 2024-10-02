using AutoMapper;
using BankApp.DTOs;
using BankApp.Models;

namespace BankApp.Mapper
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<User,UserSendDTO>();
            CreateMap<UserSignUpDTO, User>();
            CreateMap<UserLoginDTO, UserSendDTO>();
            CreateMap<Account, AccountSendDTO>();
            CreateMap<AccountAddDTO, Account>();
            CreateMap<AccountUpdateDTO, Account>();
            CreateMap<Transaction, TransactionSendDTO>();
            CreateMap<TransactionAddDTO, Transaction>();
        }
    }
}
