using CQRSExample.Class;
using MediatR;

namespace CQRSExample.Queries
{
    public record GetProductsQuery() : IRequest<IEnumerable<Product>>;
}
