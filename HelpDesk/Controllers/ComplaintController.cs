using HelpDesk.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Controllers
{
    [Authorize]
    public class ComplaintController : Controller
    {
        private readonly HelpDeskdbContext _context;
        private readonly UserManager<User> _userManager;

        public ComplaintController(HelpDeskdbContext context, UserManager<User> UserManager)
        {
            _context = context;
            _userManager = UserManager;
        }

        //display complaint List
        public ActionResult Index()
        {
            if (User.IsInRole("Technician"))
            {
                var complaints = _context.Complaints.ToList();
                return View(complaints);
            }
            else
            {
                var currentUser = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                var complaints = _context.Complaints.Where(c => c.UserId == currentUser.Id).ToList();
                return View(complaints);
            }
        }

       // user create a new complaint 
       // this action return the form view of creating new complain
        public IActionResult Create() {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  async  Task<IActionResult> Create([Bind("Title,Description")] Complaint complaint)
        {

            var user = await _userManager.GetUserAsync(User);
                complaint.UserId = user.Id;
                complaint.Id = Guid.NewGuid().ToString();
                complaint.IsApproved = false;
                complaint.IsLocked = false;
                complaint.Action = "";
                complaint.Status = "not yet processed";
                complaint.DateCreated = DateTime.Now;
                
               await _context.AddAsync(complaint);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            
            
        }


        //user could update his complaint

        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var complaint = await _context.Complaints.FirstOrDefaultAsync(c => c.Id == id);
            if (complaint == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (complaint.IsLocked || complaint.UserId != user.Id)
            {
                return Forbid();
            }

            if (complaint.Status != "not yet processed")
            {
                TempData["ErrorMessage"] = "You can only Edit complaints that have not yet been processed.";
                return RedirectToAction(nameof(Index));
            }

            return View(complaint);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Description")] Complaint complaint)
        {
            if (id == null || complaint == null)
            {
                return BadRequest();
            }

            var user = await _userManager.GetUserAsync(User);
            var originalComplaint = await _context.Complaints.FirstOrDefaultAsync(c => c.Id == id);
            if (originalComplaint == null || originalComplaint.IsLocked || originalComplaint.UserId != user.Id)
            {
                return Forbid();
            }

            if (originalComplaint.Status != "not yet processed")
            {
                TempData["ErrorMessage"] = "You can only Edit complaints that have not yet been processed.";
                return View(complaint);
            }

            originalComplaint.Title = complaint.Title;
            originalComplaint.Description = complaint.Description;
            _context.Update(originalComplaint);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        public async Task<IActionResult> Delete(string id)
        {
            var complaint = await _context.Complaints.FirstOrDefaultAsync(c => c.Id == id);
            if (complaint == null)
            {
                return NotFound();
            }
            //var user = await _userManager.GetUserAsync(User);
            if (complaint.IsLocked || complaint.IsApproved)
            {
                return Forbid();
            }
            if (complaint.Status != "not yet processed")
            {
                TempData["ErrorMessage"] = "You can only Delete complaints that have not yet been processed.";
                return RedirectToAction(nameof(Index));
            }

            return View(complaint);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async   Task<IActionResult> DeleteConfirmed(string id)
        {
            var complaint = await  _context.Complaints.FirstOrDefaultAsync(c => c.Id == id);
            if (complaint == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (complaint.IsLocked || complaint.IsApproved || complaint.UserId != user.Id )
            {
                return Forbid();
            }
            if (complaint.Status != "not yet processed")
            {
                TempData["ErrorMessage"] = "You can only Delete complaints that have not yet been processed.";
                return RedirectToAction(nameof(Index));
            }
       
            
            _context.Complaints.Remove(complaint); 
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Complaint successfully deleted.";
            return RedirectToAction(nameof(Index));
        }

        //user approve the complaint 
        [HttpPost]
        
        public async Task<ActionResult> Approve(string id)
        {
            var complaint = _context.Complaints.FirstOrDefault(c => c.Id == id);
            if (complaint == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (complaint.UserId != user.Id)
            {
                return Forbid();
            }
            if (complaint.IsLocked || complaint.IsApproved)
            {


                TempData["ErrorMessage"] = "You can only Delete complaints that have not yet been processed.";
                return RedirectToAction(nameof(Index));
            }
            if (complaint.Status == "not yet processed")
            {
                TempData["ErrorMessage"] = "You can not approve  complaints that have not yet been processed.";
                return RedirectToAction(nameof(Index));
            }

            complaint.IsApproved = true;
            complaint.Status = "Approved";
            _context.Update(complaint);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        // Technician actions
        [Authorize(Policy = "TechnicianOnly")]
        public async  Task<ActionResult> Process(string id)
        {
            var complaint = await _context.Complaints.FirstOrDefaultAsync(c => c.Id == id);
            if (complaint == null)
            {
                return NotFound();
            }
            if (complaint.IsLocked || complaint.IsApproved)
            {
                return Forbid();
            }
                return View(complaint);
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "TechnicianOnly")]
        public async Task<IActionResult> Process(string id, [Bind("Id,Action,Status")] Complaint complaint)
        {

            if (id != complaint.Id)
            {
                return NotFound();
            }
            var originalComplaint = await _context.Complaints.FirstOrDefaultAsync(c => c.Id == id);
            if (originalComplaint == null)
            {
                return NotFound();
            }
            if (originalComplaint.IsLocked || originalComplaint.IsApproved)
            {
                return Forbid();
            }
            originalComplaint.Status = complaint.Status;
            originalComplaint.Action = complaint.Action;
            originalComplaint.DateResolved =  DateTime.Now;
            _context.Update(originalComplaint);
            await  _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "TechnicianOnly")]
        public IActionResult Close(String id)
        {
            var complaint = _context.Complaints.FirstOrDefault(c => c.Id == id);
            if (complaint == null)
            {
                return NotFound();
            }
            if (complaint.IsLocked || !complaint.IsApproved)
            {


                TempData["ErrorMessage"] = "You can only Close complaints that have been approved.";
                return RedirectToAction(nameof(Index));
            }
            complaint.IsLocked = true;
            complaint.Status = "closed";
            _context.Update(complaint);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


    }
}

