using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Attributes;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Tasks.Commands.CompleteTask;
using TaskManagement.Application.Tasks.Commands.CreateTask;
using TaskManagement.Application.Tasks.Commands.DeleteTask;
using TaskManagement.Application.Tasks.Commands.RejectTask;
using TaskManagement.Application.Tasks.Commands.UpdateTask;
using TaskManagement.Application.Tasks.DTOs;
using TaskManagement.Application.Tasks.Queries.GetAllTasks;
using TaskManagement.Application.Tasks.Queries.GetCreatedTasks;
using TaskManagement.Application.Tasks.Queries.GetDepartmentTasks;
using TaskManagement.Application.Tasks.Queries.GetTaskById;
using TaskManagement.Application.Tasks.Queries.GetUserTasks;
using TaskManagement.Application.Users.Commands.Login;
using TaskManagement.Application.Users.DTOs;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : BaseApiController
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<TaskDto>>>> GetAllTasks()
        {
            var result = await _mediator.Send(new GetAllTasksQuery());
            return Ok(result);
        }

        [HttpGet("my-tasks")]
        [ValidateUserId]
        public async Task<ActionResult<BaseResponse<List<TaskDto>>>> GetUserTasks()
        {
            var result = await _mediator.Send(new GetUserTasksQuery());
            return Ok(result);
        }

        [HttpGet("created")]
        [ValidateUserId]
        public async Task<ActionResult<BaseResponse<List<TaskDto>>>> GetCreatedTasks()
        {
            var result = await _mediator.Send(new GetCreatedTasksQuery());
            return Ok(result);
        }

        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<BaseResponse<List<TaskDto>>>> GetDepartmentTasks(Guid departmentId)
        {
            var result = await _mediator.Send(new GetDepartmentTasksQuery { DepartmentId = departmentId });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponse<TaskDto>>> GetTaskById(Guid id)
        {
            var result = await _mediator.Send(new GetTaskByIdQuery { Id = id });
            return Ok(result);
        }

        [HttpPost]
        [ValidateUserId]
        public async Task<ActionResult<BaseResponse<TaskDto>>> CreateTask([FromBody] CreateTaskCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [ValidateUserId]
        public async Task<ActionResult<BaseResponse<TaskDto>>> UpdateTask(Guid id, [FromBody] UpdateTaskCommand command)
        {
            if (id != command.Id)
                return BadRequest("Task ID mismatch");

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ValidateUserId]
        public async Task<ActionResult<BaseResponse<bool>>> DeleteTask(Guid id)
        {
            var result = await _mediator.Send(new DeleteTaskCommand { Id = id });
            return Ok(result);
        }

        [HttpPost("{id}/complete")]
        [ValidateUserId]
        public async Task<ActionResult<BaseResponse<TaskDto>>> CompleteTask(Guid id)
        {
            var result = await _mediator.Send(new CompleteTaskCommand { Id = id });
            return Ok(result);
        }

        [HttpPost("{id}/reject")]
        [ValidateUserId]
        public async Task<ActionResult<BaseResponse<TaskDto>>> RejectTask(Guid id)
        {
            var result = await _mediator.Send(new RejectTaskCommand { Id = id });
            return Ok(result);
        }
    }
} 