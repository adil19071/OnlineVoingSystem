using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineVotingSystem.Data;
using OnlineVotingSystem.Models;

namespace OnlineVotingSystem.Controllers
{
    public class PollsController : Controller
    {
        private readonly AppDbContext _context;

        public PollsController(AppDbContext context)
        {
            _context = context;
        }

        // Public: list active polls
        public async Task<IActionResult> Index()
        {
            var polls = await _context.Polls
                .Where(p => p.IsActive)
                .Include(p => p.Options)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(polls);
        }

        // Public: vote page
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var poll = await _context.Polls
                .Include(p => p.Options)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (poll == null) return NotFound();

            return View(poll);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(int pollId, int optionId)
        {
            var option = await _context.PollOptions
                .Include(o => o.Poll)
                .FirstOrDefaultAsync(o => o.Id == optionId && o.PollId == pollId);

            if (option == null || option.Poll == null || !option.Poll.IsActive)
            {
                return NotFound();
            }

            option.VoteCount += 1;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Results), new { id = pollId });
        }

        // Public: results
        public async Task<IActionResult> Results(int? id)
        {
            if (id == null) return NotFound();

            var poll = await _context.Polls
                .Include(p => p.Options)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (poll == null) return NotFound();

            return View(poll);
        }

        // "Admin" area (no authentication, for demo/college use only)

        // GET: Polls/Manage
        public async Task<IActionResult> Manage()
        {
            var polls = await _context.Polls
                .Include(p => p.Options)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(polls);
        }

        // GET: Polls/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Polls/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Question,IsActive")] Poll poll)
        {
            if (ModelState.IsValid)
            {
                _context.Add(poll);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Manage));
            }
            return View(poll);
        }

        // GET: Polls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var poll = await _context.Polls.FindAsync(id);
            if (poll == null) return NotFound();

            return View(poll);
        }

        // POST: Polls/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Question,IsActive,CreatedAt")] Poll poll)
        {
            if (id != poll.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(poll);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PollExists(poll.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Manage));
            }
            return View(poll);
        }

        // GET: Polls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var poll = await _context.Polls
                .FirstOrDefaultAsync(m => m.Id == id);
            if (poll == null) return NotFound();

            return View(poll);
        }

        // POST: Polls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var poll = await _context.Polls.FindAsync(id);
            if (poll != null)
            {
                _context.Polls.Remove(poll);
                var options = _context.PollOptions.Where(o => o.PollId == id);
                _context.PollOptions.RemoveRange(options);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Manage));
        }

        // Manage options for a poll
        public async Task<IActionResult> ManageOptions(int? pollId)
        {
            if (pollId == null) return NotFound();

            var poll = await _context.Polls
                .Include(p => p.Options)
                .FirstOrDefaultAsync(p => p.Id == pollId);

            if (poll == null) return NotFound();

            return View(poll);
        }

        // GET: Polls/CreateOption
        public IActionResult CreateOption(int pollId)
        {
            ViewBag.PollId = pollId;
            return View();
        }

        // POST: Polls/CreateOption
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOption([Bind("PollId,Text")] PollOption option)
        {
            if (ModelState.IsValid)
            {
                _context.PollOptions.Add(option);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageOptions), new { pollId = option.PollId });
            }
            ViewBag.PollId = option.PollId;
            return View(option);
        }

        // POST: Polls/DeleteOption/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOption(int id, int pollId)
        {
            var option = await _context.PollOptions.FindAsync(id);
            if (option != null)
            {
                _context.PollOptions.Remove(option);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ManageOptions), new { pollId });
        }

        private bool PollExists(int id)
        {
            return _context.Polls.Any(e => e.Id == id);
        }
    }
}
