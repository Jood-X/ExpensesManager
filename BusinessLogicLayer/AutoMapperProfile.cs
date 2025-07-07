using AutoMapper;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.BusinessLayer.UserService.UserDTO;
using ExpenseManager.BusinessLayer.CategoriesService.CategoriesDTO;
using ExpenseManager.BusinessLayer.TransactionsService.TransactionsDTO;
using ExpenseManager.BusinessLayer.RecurringsService.RecurringsDTO;
using ExpenseManager.BusinessLayer.WalletService.WalletDTO;

namespace ExpenseManager.BusinessLayer
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            // Create maps between User and UserDTO, CreateUserDTO, UpdateUserDTO
            CreateMap<User, UsersDTO>();
            CreateMap<UsersDTO, User>();
            CreateMap<CreateUserDTO, User>();
            CreateMap<UpdateUserDTO, User>().ForAllMembers(opts => opts.Condition((src,dest,srcMember) => srcMember != null));

            // Create maps between Category and CategoryDTO, CreateCategoryDTO, UpdateCategoryDTO
            CreateMap<Category, CategoryDTO>()
            .ForMember(dest => dest.CreatedBy,
                opt => opt.MapFrom(src => src.CreateByNavigation != null ? src.CreateByNavigation.Name : "Unknown"))
            .ForMember(dest => dest.UpdatedBy,
                opt => opt.MapFrom(src => src.UpdateByNavigation != null ? src.UpdateByNavigation.Name : null));
            
            CreateMap<CategoryDTO, Category>();
            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<UpdateCategoryDTO, Category>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Create maps between Transaction and TransactionDTO, CreateTransactionDTO, UpdateTransactionDTO
            CreateMap<Transaction, TransactionDTO>()
                .ForMember(dest => dest.CreatedBy,
                    opt => opt.MapFrom(src => src.CreateByNavigation != null ? src.CreateByNavigation.Name : "Unknown"))
                .ForMember(dest => dest.UpdatedBy,
                    opt => opt.MapFrom(src => src.UpdateByNavigation != null ? src.UpdateByNavigation.Name : null))
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : ""))
                .ForMember(dest => dest.WalletName,
                    opt => opt.MapFrom(src => src.Wallet != null ? src.Wallet.Name : ""));
            CreateMap<TransactionDTO, Transaction>();
            CreateMap<CreateTransactionDTO, Transaction>();
            CreateMap<UpdateTransactionDTO, Transaction>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Create maps between RecurringExpense and RecurringExpenseDTO, CreateRecurringExpenseDTO, UpdateRecurringExpenseDTO
            CreateMap<Recurring, RecurringExpenseDTO>()
                .ForMember(dest => dest.CreatedBy,
                    opt => opt.MapFrom(src => src.CreateByNavigation != null ? src.CreateByNavigation.Name : "Unknown"))
                .ForMember(dest => dest.UpdatedBy,
                    opt => opt.MapFrom(src => src.UpdateByNavigation != null ? src.UpdateByNavigation.Name : null))
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : ""))
                .ForMember(dest => dest.WalletName,
                    opt => opt.MapFrom(src => src.Wallet != null ? src.Wallet.Name : "")); 
            CreateMap<RecurringExpenseDTO, Recurring>();
            CreateMap<CreateRecurringDTO, Recurring>();
            CreateMap<UpdateRecurringDTO, Recurring>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Create maps between Wallet and WalletDTO, CreateWalletDTO, UpdateWalletDTO
            CreateMap<Wallet, WalletsDTO>()
            .ForMember(dest => dest.CreatedBy,
                opt => opt.MapFrom(src => src.CreateByNavigation != null ? src.CreateByNavigation.Name : "Unknown"))
            .ForMember(dest => dest.UpdatedBy,
                opt => opt.MapFrom(src => src.UpdateByNavigation != null ? src.UpdateByNavigation.Name : null));

            CreateMap<WalletsDTO, Wallet>();
            CreateMap<CreateWalletDTO, Wallet>();
            CreateMap<UpdateWalletDTO, Wallet>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        }

    }
}
