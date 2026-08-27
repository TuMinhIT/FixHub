using FixHub.Application.Common.Interfaces;
using MediatR;

namespace FixHub.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOrderStatusHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.OrderRepository.FindById(request.Id);
            if (order == null) throw new Exception("Order not found");
            
            order.Status = request.Status;
            
            await _unitOfWork.OrderRepository.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
