using FixHub.Application.Common.Interfaces;
using FixHub.Application.Common.Exceptions;
using FixHub.Application.Features.Rag.DTOs;
using FixHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FixHub.Application.Common.Interfaces.Rag;

namespace FixHub.Application.Features.Rag.Commands.AskRag;

public sealed class AskRagHandler : IRequestHandler<AskRagCommand, RagAnswerResponse>
{
    private readonly IRagSearchService _searchService;
    private readonly IRagAnswerService _answerService;
    private readonly IUnitOfWork _unitOfWork;

    public AskRagHandler(IRagSearchService searchService, IRagAnswerService answerService, IUnitOfWork unitOfWork)
    {
        _searchService = searchService;
        _answerService = answerService;
        _unitOfWork = unitOfWork;
    }

    public async Task<RagAnswerResponse> Handle(AskRagCommand request, CancellationToken cancellationToken)
    {
        var searchQuestion = request.Question;
        if (request.ProductId.HasValue)
        {
            var product = await _unitOfWork.ProductRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == request.ProductId.Value && x.IsActive, cancellationToken);
            if (product == null)
                throw new NotFoundException(nameof(Product), request.ProductId.Value);

            searchQuestion = $"Sản phẩm {product.Name} {product.Brand} {product.Model}. Thông số: {product.SpecificationsJson}. Câu hỏi: {request.Question}";
        }

        var sources = await _searchService.SearchAsync(searchQuestion, request.TopK, cancellationToken);
        if (sources.Count == 0)
        {
            return new RagAnswerResponse
            {
                Answer = "Mình chưa tìm thấy tài liệu phù hợp để trả lời chắc chắn. Bạn nên ngắt điện nếu thiết bị có dấu hiệu bất thường và liên hệ kỹ thuật viên FixHub.",
                Sources = sources,
                Confidence = 0
            };
        }

        var answer = await _answerService.GenerateAnswerAsync(request.Question, sources, cancellationToken);
        return new RagAnswerResponse
        {
            Answer = answer,
            Sources = sources,
            Confidence = sources.Max(x => x.Score)
        };
    }
}
