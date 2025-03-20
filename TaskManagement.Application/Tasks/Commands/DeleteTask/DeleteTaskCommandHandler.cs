using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Application.Tasks.Commands.DeleteTask
{
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, BaseResponse<Unit>>
    {
        private readonly ApplicationDbContext _context;

        public DeleteTaskCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BaseResponse<Unit>> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (task == null)
                return BaseResponse<Unit>.CreateError("Task not found");

            if (task.CreatedById != request.UserId)
                return BaseResponse<Unit>.CreateError("You can only delete tasks that you created");

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync(cancellationToken);

            return BaseResponse<Unit>.CreateSuccess(Unit.Value, "Task deleted successfully");
        }
    }
} 