using Application.Common.Interfeces.Repositorieses;
using Domain.Exercises;
using MediatR;
namespace ;
public record CreateExerciseCommand : IRequest<Exercise>
{
public required string Title { get; init; }
public required decimal Price { get; init; }
}
public class CreateProductCommandHandler(IProductRepository productRepository)
: IRequestHandler<CreateProductCommand, Product>
{
public async Task<Product> Handle(CreateProductCommand request,
CancellationToken cancellationToken)
{
var existingProduct = await productRepository.GetByTitle(request.Title, cancellationToken);
if (existingProduct is not null)
{
throw new ArgumentException($"Product with title '{request.Title}'already exists");
}
var product = Product.New(Guid.NewGuid(), request.Title, request.Price);
return await productRepository.Add(product, cancellationToken);
}
}