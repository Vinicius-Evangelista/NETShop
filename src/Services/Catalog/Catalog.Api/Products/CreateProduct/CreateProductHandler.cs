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
            Name: req.Name,
            Category: req.Category,
            Description: req.Description,
            ImageFile: req.ImageFile,
            Price: req.Price
        );
}

public record CreateProductResult(Guid Id)
{
    public static CreateProductResponse ToResponse(
        CreateProductResult req
    ) => new(Id: req.Id);
}

public class CreateProductCommandValidator
    : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(expression: x => x.Name)
            .NotEmpty()
            .MaximumLength(maximumLength: 100)
            .WithMessage(errorMessage: "Name is required");

        RuleFor(expression: x => x.Category)
            .NotEmpty()
            .WithMessage(errorMessage: "Category is required");

        RuleFor(expression: x => x.Description)
            .NotEmpty()
            .MaximumLength(maximumLength: 500)
            .WithMessage(errorMessage: "Description is required");

        RuleFor(expression: x => x.ImageFile)
            .NotEmpty()
            .MaximumLength(maximumLength: 100)
            .WithMessage(errorMessage: "ImageFile is required");

        RuleFor(expression: x => x.Price)
            .GreaterThan(valueToCompare: 0)
            .NotEmpty()
            .WithMessage(errorMessage: "Price is required");
    }
}

class CreateProductHandler(IDocumentSession dbSession)
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
        await dbSession.SaveChangesAsync(token: cancellationToken);

        return new CreateProductResult(Id: product.Id);
    }
}
