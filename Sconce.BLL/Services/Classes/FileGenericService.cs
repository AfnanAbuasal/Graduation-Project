using Mapster;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models.Enums;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sconce.DAL.DTO.Requests;

namespace Sconce.BLL.Services.Classes;

public class FileGenericService<TRequest, TResponse, TEntity>
    : IFileGenericService<TRequest, TResponse, TEntity>
    where TEntity : BaseModel, IFileEntity
    where TRequest : IFileRequest
{
    private readonly IGenericRepository<TEntity> _repository;
    private readonly IFileService _fileService;
    private readonly IUrlHelper _urlHelper;

    private readonly string _folder;

    public FileGenericService(
        IGenericRepository<TEntity> repository,
        IFileService fileService,
        IUrlHelper urlHelper,
        string folder)
    {
        _repository = repository;
        _fileService = fileService;
        _urlHelper = urlHelper;
        _folder = folder;
    }

    public virtual async Task<(int NumberOfEntries, Response Response)> CreateAsync(TRequest request)
    {
        var entity = request.Adapt<TEntity>();

        if (request.File != null)
            entity.FilePath = await _fileService.SaveFileAsync(request.File, _folder);

        var rows = await _repository.AddAsync(entity);

        return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) created successfully." });
    }

    public virtual async Task<(bool Success, Response Response)> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return (false, new ErrorResponse { Errors = ["Not Found."] });

        var dto = entity.Adapt<TResponse>();

        // inject FileUrl
        var fileUrlProp = typeof(TResponse).GetProperty("FileUrl");
        if (fileUrlProp != null)
        {
            var url = _urlHelper.BuildUrl(entity.FilePath);
            fileUrlProp.SetValue(dto, url);
        }

        return (true, new SuccessResponse<TResponse> { Data = dto });
    }

    public virtual async Task<Response> GetAllAsync(bool onlyActive = false)
    {
        var list = await _repository.GetAllAsync();

        if (onlyActive)
            list = list.Where(x => x.Status == Status.Active);

        var responseList = new List<TResponse>();

        foreach (var entity in list)
        {
            var dto = entity.Adapt<TResponse>();

            var fileUrlProp = typeof(TResponse).GetProperty("FileUrl");
            if (fileUrlProp != null)
                fileUrlProp.SetValue(dto, _urlHelper.BuildUrl(entity.FilePath));

            responseList.Add(dto);
        }

        return new SuccessResponse<IEnumerable<TResponse>> { Data = responseList };
    }

    public virtual async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int id, TRequest request)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return (0, new ErrorResponse { Errors = ["Not Found."] });

        request.Adapt(entity);

        // If a new file was uploaded → replace old file
        if (request.File != null)
        {
            if (!string.IsNullOrEmpty(entity.FilePath))
                _fileService.DeleteFileAsync(entity.FilePath);

            entity.FilePath = await _fileService.SaveFileAsync(request.File, _folder);
        }

        var rows = await _repository.UpdateAsync(entity);

        return (rows, new SuccessResponse<string>
        {
            Data = $"{rows} record(s) updated successfully."
        });
    }

    public virtual async Task<(int NumberOfEntries, Response Response)> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return (0, new ErrorResponse { Errors = ["Not Found."] });

        // delete file
        if (!string.IsNullOrEmpty(entity.FilePath))
            _fileService.DeleteFileAsync(entity.FilePath);

        var rows = await _repository.DeleteAsync(entity);

        return (rows, new SuccessResponse<string>
        {
            Data = $"{rows} record(s) deleted successfully."
        });
    }

    public async Task<(bool Success, Response Response)> ToggleStatusAsync(int ID)
    {
        var entity = await _repository.GetByIdAsync(ID);

        if (entity == null)
            return (false, new ErrorResponse { Errors = ["Not Found."] });

        entity.Status = entity.Status == Status.Active ? Status.Inactive : Status.Active;

        await _repository.UpdateAsync(entity);

        return (true, new SuccessResponse<string> { Data = "Status toggled successfully." });
    }
}
