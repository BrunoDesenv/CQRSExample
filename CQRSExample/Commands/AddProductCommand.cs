using CQRSExample.Class;
using MediatR;

namespace CQRSExample.Commands
{
    public record AddProductCommand(Product Product) : IRequest;
}
