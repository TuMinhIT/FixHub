//using BookStore.Application.Common.Interfaces;
//using BookStore.Domain.Entities;
//using BookStore.Domain.IRepositories;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BookStore.Application.Books.Commands.Create
//{
//    public class CreateBookHandler
      
//        : IRequestHandler<CreateBookCommand, Guid>
//    {
//        private readonly IBookRepository _repository;

//        private readonly IUnitOfWork _unitOfWork;

//        public CreateBookHandler(
//            IBookRepository repository,
//            IUnitOfWork unitOfWork)
//        {
//            _repository = repository;
//            _unitOfWork = unitOfWork;
//        }

//        public async Task<Guid> Handle(
//            CreateBookCommand request,
//            CancellationToken cancellationToken)
//        {
//            var book = new Book
//            {
//                Title = request.Title,
//                Price = request.Price
//            };

//            await _repository.AddAsync(book);

//            await _unitOfWork.SaveChangesAsync(cancellationToken);

//            return book.Id;
//        }
//    }
//}
