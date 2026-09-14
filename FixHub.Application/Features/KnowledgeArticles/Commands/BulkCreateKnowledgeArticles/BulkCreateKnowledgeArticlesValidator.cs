using FluentValidation;

namespace FixHub.Application.Features.KnowledgeArticles.Commands.BulkCreateKnowledgeArticles;

public class BulkCreateKnowledgeArticlesValidator : AbstractValidator<BulkCreateKnowledgeArticlesCommand>
{
    public const int MaxArticlesPerRequest = 500;

    public BulkCreateKnowledgeArticlesValidator()
    {
        RuleFor(command => command.Articles)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithMessage("At least one article is required.")
            .Must(articles => articles.Count <= MaxArticlesPerRequest)
            .WithMessage($"A request can contain at most {MaxArticlesPerRequest} articles.");

        RuleForEach(command => command.Articles)
            .NotNull()
            .WithMessage("Article cannot be null.");

        RuleForEach(command => command.Articles).ChildRules(article =>
        {
            article.RuleFor(item => item.Title).NotEmpty();
            article.RuleFor(item => item.Content).NotEmpty();
        });
    }
}
