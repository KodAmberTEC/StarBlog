﻿using FreeSql;
using Microsoft.AspNetCore.Mvc;
using StarBlog.Data.Models;
using StarBlog.Web.Extensions;
using StarBlog.Web.ViewModels;
using StarBlog.Web.ViewModels.Response;
using X.PagedList;

namespace StarBlog.Web.Apis;

[ApiController]
[Route("Api/[controller]")]
public class CategoryController : ControllerBase {
    private IBaseRepository<Category> _categoryRepo;

    public CategoryController(IBaseRepository<Category> categoryRepo) {
        _categoryRepo = categoryRepo;
    }

    [HttpGet]
    public ApiResponsePaged<Category> GetList(int page = 1, int pageSize = 10) {
        var paged = _categoryRepo.Select.ToList().ToPagedList(page, pageSize);
        return new ApiResponsePaged<Category> {
            Pagination = paged.ToPaginationMetadata(),
            Data = paged.ToList()
        };
    }

    [HttpGet("{id:int}")]
    public ApiResponse<Category> Get(int id) {
        var item = _categoryRepo.Where(a => a.Id == id).First();
        if (item == null) return ApiResponse<Category>.NotFound(Response);
        return new ApiResponse<Category> { Data = item };
    }

    /// <summary>
    /// 分类词云
    /// </summary>
    /// <returns></returns>
    [HttpGet("[action]")]
    public ApiResponse<List<object>> WordCloud() {
        var list = _categoryRepo.Select.IncludeMany(a => a.Posts).ToList();
        var data = new List<object>();
        foreach (var item in list) {
            data.Add(new { name = item.Name, value = item.Posts.Count });
        }

        return new ApiResponse<List<object>> { Data = data };
    }
}