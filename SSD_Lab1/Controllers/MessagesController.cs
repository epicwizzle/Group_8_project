using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSD_Lab1.Data;
using SSD_Lab1.Models;

namespace SSD_Lab1.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _manager;

        public MessagesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _manager = userManager;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            var messages = await _context.Message.Include((m) => m.User).ToListAsync();

            return View(messages);
        }

        // GET: Messages/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("MessageString")] Message message)
        {
            var userId = _manager.GetUserId(User);
            message.UserId = userId;

            if (ModelState.IsValid)
            {

                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

    }
}
