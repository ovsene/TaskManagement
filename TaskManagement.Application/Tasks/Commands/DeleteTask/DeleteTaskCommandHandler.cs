using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;
using TaskManagement.Domain.Common.Interfaces;

namespace TaskManagement.Application.Tasks.Commands.DeleteTask
{
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, BaseResponse<Unit>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DeleteTaskCommandHandler> _logger;

        public DeleteTaskCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            ILogger<DeleteTaskCommandHandler> logger)
        {
            _context = context;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<BaseResponse<Unit>> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to delete task with ID: {TaskId}", request.Id);

                var currentUserId = _currentUserService.UserId;
                if (string.IsNullOrEmpty(currentUserId.ToString()))
                {
                    _logger.LogWarning("No user ID found in current session");
                    return BaseResponse<Unit>.CreateError("User not authenticated");
                }

                var task = await _context.Tasks
                    .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

                if (task == null)
                {
                    _logger.LogWarning("Task with ID {TaskId} not found", request.Id);
                    return BaseResponse<Unit>.CreateError("Task not found");
                }

                if (task.CreatedById != currentUserId)
                {
                    _logger.LogWarning("User {UserId} attempted to delete task {TaskId} created by {CreatedById}", 
                        currentUserId, request.Id, task.CreatedById);
                    return BaseResponse<Unit>.CreateError("You can only delete tasks that you created");
                }

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Task {TaskId} deleted successfully", request.Id);

                return BaseResponse<Unit>.CreateSuccess(Unit.Value, "Task deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {TaskId}", request.Id);
                return BaseResponse<Unit>.CreateError($"Error deleting task: {ex.Message}");
            }
        }
    }
} 