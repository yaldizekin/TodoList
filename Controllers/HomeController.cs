
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Controllers
       
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
           
            _userManager = userManager;
            _context = context;
        }
     

        
		public IActionResult Index(Todo todo)
        {

          
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                
                return RedirectToAction("PageNotFound");
            }
            IEnumerable<Todo> todocart;
             todocart = _context.Todos.Where(t => t.AppUserId == userId).OrderBy(t => t.DueDate);

            return View(todocart);


        }
        [Route(template:"today")]
        public IActionResult Today()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("PageNotFound");
            }

            // Bugünün tarihini alın
            var today = DateTime.Today;

            // Kullanıcının bugün için yapması gereken görevleri alın
            IEnumerable<Todo> todocart;
            todocart = _context.Todos.Where(t => t.AppUserId == userId && t.DueDate.Date == today).OrderBy(t => t.DueDate);

            return View(todocart);

        }
        [Route(template: "week")]
        public IActionResult Week()
        {
          
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("PageNotFound");
                }

                // Bu haftanın başlangıcını bul
                var today = DateTime.Today;
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                var endOfWeek = startOfWeek.AddDays(6);

                IEnumerable<Todo> todocart;
                todocart = _context.Todos
                    .Where(t => t.AppUserId == userId && t.DueDate.Date >= startOfWeek && t.DueDate.Date <= endOfWeek);

                return View(todocart);
            }



     
        [Route(template: "month")]
        public IActionResult Month()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("PageNotFound");
            }

            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            IEnumerable<Todo> todocart;
            todocart = _context.Todos.Where(t => t.AppUserId == userId && t.DueDate.Date >= startOfMonth && t.DueDate.Date <= endOfMonth).OrderBy(t => t.DueDate);

            return View(todocart);
        }

        [Route(template: "addtodo")]
        public IActionResult AddTodo()
        {
            // Doğrudan HttpContext.User.Claims üzerinden kullanıcı kimliğini alın
            var currentUserIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            // Eğer kullanıcı kimliği varsa, değerini ViewData'ya atayın
            if (currentUserIdClaim != null)
            {
                ViewData["AppUserId"] = currentUserIdClaim.Value;
            }
            else
            {
                // Eğer kullanıcı kimliği yoksa, boş bir değer veya hata durumuyla başa çıkabilirsiniz
                ViewData["AppUserId"] = string.Empty;
            }

            return View();
        }



        [HttpPost]
        [Route(template: "addtodo")]
        public async Task<IActionResult> AddTodo(Todo todo)
        {
            if (ModelState.IsValid)
            {
                // Giriş yapan kullanıcının ID'sini alın
                var user = await _userManager.GetUserAsync(User);

                // Kullanıcı oturum açmışsa ve kullanıcı bilgileri doğru bir şekilde alınmışsa devam edin
                if (user != null)
                {
                    todo.AppUserId = user.Id; // Kullanıcının ID'sini todo'nun AppUserId özelliğine atayın
                }

                todo.CreatedOn = DateTime.Now;
                _context.Add(todo);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(todo);
        }
        [Route(template: "edittodo")]
        public IActionResult EditTodo(int id)
        {

            var blog = _context.Todos.SingleOrDefault(i => i.Id == id);


            return View(blog);
        }

        [HttpPost]
        [Route(template: "edittodo")]
        public IActionResult EditTodo(Todo e)
        {

            var existingTodo = _context.Todos.SingleOrDefault(i => i.Id == e.Id);


            existingTodo.Title = e.Title;
            existingTodo.Summary = e.Summary;
            existingTodo.Description = e.Description;
            existingTodo.IsActive = e.IsActive;
            existingTodo.DueDate = e.DueDate;
       
            existingTodo.CreatedOn = DateTime.Now;

            _context.Update(existingTodo);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));



        }

        public IActionResult Detail(int id)
        {
            var post = _context
                .Todos
                .Where(Todo => Todo.Id == id)
                .FirstOrDefault();


            if (post != null)
            {
                return View(post);
            }

            Response.StatusCode = 404;
            return View("PageNotFound");
        }

        [Route("deletetodo/{id}")]
        public IActionResult DeleteTodo(int id)
        {
            _context.Todos.Remove(
                    _context.Todos.Find(id)
                );
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }
        public IActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}