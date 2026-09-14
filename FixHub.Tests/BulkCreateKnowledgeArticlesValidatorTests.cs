using FixHub.Application.Features.KnowledgeArticles.Commands.BulkCreateKnowledgeArticles;
using FixHub.Application.Features.KnowledgeArticles.Commands.CreateKnowledgeArticle;
using Xunit;

namespace FixHub.Tests;

public sealed class BulkCreateKnowledgeArticlesValidatorTests
{
    private readonly BulkCreateKnowledgeArticlesValidator _validator = new();

    [Fact]
    public void Validate_RejectsEmptyListAndArticlesWithoutRequiredFields()
    {
        var emptyResult = _validator.Validate(new BulkCreateKnowledgeArticlesCommand());
        var missingFieldsResult = _validator.Validate(new BulkCreateKnowledgeArticlesCommand
        {
            Articles = [new CreateKnowledgeArticleCommand { Title = "  ", Content = "" }]
        });

        Assert.Contains(emptyResult.Errors, error => error.PropertyName == "Articles");
        Assert.Contains(missingFieldsResult.Errors, error => error.PropertyName.Contains("Title"));
        Assert.Contains(missingFieldsResult.Errors, error => error.PropertyName.Contains("Content"));
    }

    [Fact]
    public void Validate_AllowsAtMostFiveHundredArticles()
    {
        var validRequest = new BulkCreateKnowledgeArticlesCommand
        {
            Articles = Enumerable.Range(1, BulkCreateKnowledgeArticlesValidator.MaxArticlesPerRequest)
                .Select(index => new CreateKnowledgeArticleCommand
                {
                    Title = $"Article {index}",
                    Content = $"Content {index}"
                })
                .ToList()
        };
        var tooManyRequest = new BulkCreateKnowledgeArticlesCommand
        {
            Articles = [.. validRequest.Articles, new CreateKnowledgeArticleCommand { Title = "Overflow", Content = "Content" }]
        };

        Assert.True(_validator.Validate(validRequest).IsValid);
        Assert.Contains(_validator.Validate(tooManyRequest).Errors, error => error.PropertyName == "Articles");
    }
}
