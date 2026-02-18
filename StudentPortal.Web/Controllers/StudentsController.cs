using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Web.Data;
using StudentPortal.Web.Models;
using StudentPortal.Web.Models.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPortal.Web.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(ApplicationDbContext db, ILogger<StudentsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddStudentViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddStudentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var student = new Student
            {
                Id = Guid.NewGuid(),
                Name = viewModel.Name,
                Email = viewModel.Email,
                Phone = viewModel.Phone,
                Subscribed = viewModel.Subscribed
            };

            try
            {
                _db.Students.Add(student);
                await _db.SaveChangesAsync();

                // Diagnostics: datasource + connection string + DB + row count
                var conn = _db.Database.GetDbConnection();
                var dataSource = conn.DataSource;
                var connString = conn.ConnectionString;
                var databaseName = conn.Database;
                var rowCount = await _db.Students.CountAsync();

                _logger.LogInformation("DataSource: {DataSource}", dataSource);
                _logger.LogInformation("ConnectionString: {Conn}", connString);
                _logger.LogInformation("Database: {DbName}, Students count: {Count}", databaseName, rowCount);

                TempData["SuccessMessage"] = $"Saved. DataSource: {dataSource}; DB: {databaseName}; Count: {rowCount}";
                return RedirectToAction(nameof(Add));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving student");
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewData["Exception"] = ex.ToString();
                return View(viewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var students = await _db.Students.ToListAsync();

            return View(students);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var student = await _db.Students.FindAsync(id);

            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Edit (Student viewModel)
        {
            var student = await _db.Students.FindAsync(viewModel.Id);

            if(student is not null)
            {
                student.Name = viewModel.Name;
                student.Email = viewModel.Email;
                student.Phone = viewModel.Phone;
                student.Subscribed = viewModel.Subscribed;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("List", "Students");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Student viewModel)
        {
            var student = await _db.Students.FindAsync(viewModel.Id);

            if(student is not null)
            {
                 _db.Students.Remove(student);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("List", "Students");
        }
    }
}