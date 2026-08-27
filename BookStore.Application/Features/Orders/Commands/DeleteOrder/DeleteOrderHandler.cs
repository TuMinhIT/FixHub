using FixHub.Application.Common.Interfaces;
using MediatR;

namespace FixHub.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteOrderHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.OrderRepository.FindById(request.Id);
            if (order == null) throw new Exception("Order not found");

            await _unitOfWork.OrderRepository.DeleteAsync(order.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
