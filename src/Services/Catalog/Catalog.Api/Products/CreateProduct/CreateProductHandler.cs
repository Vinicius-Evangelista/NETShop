using FluentValidation;

namespace Catalog.Api.Products.CreateProduct;

public record CreateProductCommand(
    string Name,
    List<string> Category,
    string Description,
    string ImageFile,
    decimal Price
) : ICommand<CreateProductResult>
{
    public static CreateProductCommand FromRequest(
        CreateProductRequest req
    ) =>
        new(
            req.Name,
            req.Category,
            req.Description,
            req.ImageFile,
            req.Price
        );
}

public record CreateProductResult(Guid Id)
{
    public static CreateProductResponse ToResponse(
        CreateProductResult req
    ) => new(req.Id);
}

public class CreateProductCommandValidator
    : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Name is required");

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Category is required");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Description is required");

        RuleFor(x => x.ImageFile)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("ImageFile is required");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .NotEmpty()
            .WithMessage("Price is required");
    }
}

internal class CreateProductHandler(IDocumentSession dbSession)
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken
    )
    {
        var product = new Product
        {
            Name = request.Name,
            Category = request.Category,
            ImageFile = request.ImageFile,
            Description = request.Description,
            Price = request.Price,
        };

        dbSession.Store(product);
        await dbSession.SaveChangesAsync(cancellationToken);

        return new CreateProductResult(product.Id);
    }
}
