﻿using Filer.Models;
using Microsoft.AspNetCore.Mvc;

namespace Filer.Api
{
    public class BaseController : Controller
    {
        protected readonly IConfiguration _configuration;
        protected readonly List<WorkDir> _workDirs;
        public BaseController(IConfiguration configuration)
        {
            _configuration = configuration;
            var workDirs = _configuration
                .GetSection("WorkDirs").Get<WorkDir[]>();
            var index = 0;
            foreach (var item in workDirs)
            {
                item.Path = item.Path.TrimEnd(Path.DirectorySeparatorChar);
                item.Path = $@"{item.Path}{Path.DirectorySeparatorChar}";
                item.IsPathError = !Directory.Exists(item.Path);
                item.Index = index++;
            }
            _workDirs = workDirs.ToList();
        }
    }
}