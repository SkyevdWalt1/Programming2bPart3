using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Part_2.Models;
using Part_2.Services; // Assuming you have a service class for database interactions

namespace Part_2.Controllers {
    public class UserController : Controller {
        private Db _db;
        public UserController() {
            _db = new();
        }

        public IActionResult Index() {
            int? id = HttpContext.Session.GetInt32("user_id");

            if (id.HasValue) {
                Console.WriteLine("Admin ID Session: " + id.Value);

                UserLoggedInModel model = new();
                User user = _db.GetUserById(id.Value);
                List<Claim> claims = _db.GetClaims(user.ID);
                model.User = user;
                model.Claims = claims;

                return View(model);
            } else {
                // Handle the case where user_id is not available
                return Redirect("Login");
            }
        }

        public IActionResult ViewContracts() {
            int? id = HttpContext.Session.GetInt32("user_id"); // Get user ID from session

            if (id.HasValue) {
                Console.WriteLine("Admin ID Session: " + id.Value);

                UserLoggedInModel model = new();
                User user = _db.GetUserById(id.Value);
                List<Claim> claims = _db.GetClaims(user.ID);
                model.User = user;
                model.Claims = claims;
                return View(model);
            } else {
                // Handle the case where user_id is not available
                return Redirect("Login");
            }
        }

        public IActionResult SubmitClaim() {

            return View();
        }

        [HttpPost]
        public IActionResult CreateClaim(SubmitClaimModel model) {
            if (ModelState.IsValid) {
                int? id = HttpContext.Session.GetInt32("user_id"); // Get user ID from session
                // Call the SubmitClaim method without the status
                _db.SubmitClaim(id.Value, model.Title, model.Rate, model.Hours, model.StartDate, model.EndDate);

                // Redirect to a confirmation page or back to the claims list
                return RedirectToAction("Index"); // Adjust as necessary
            }
            return View(model); // Return the view with the model if the state is invalid
        }

        public IActionResult PaymentHistory() {
            int id = HttpContext.Session.GetInt32("user_id") ?? 0;
            List<Claim> claims = _db.GetPaidClaims(id);
            return View(claims);
        }

        public IActionResult Logout() {
            HttpContext.Session.Clear();
            Navbar.isLoggedIn = false;
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public IActionResult NotesAndDocuments(int claimID) {
            HttpContext.Session.SetInt32("submitted", claimID);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> files, string notes) {
            // Get the submitted claim ID from the session
            int? claimId = HttpContext.Session.GetInt32("submitted");

            if (claimId == null) {
                return BadRequest("Claim ID not found in session.");
            }

            var submittedDocs = new SubmittedDocs {
                Blobs = new List<Blob>(),
                fileNames = new List<string>(),
                fileTypes = new List<string>(),
                Notes = notes
            };

            // Process each uploaded file
            if (files != null && files.Count > 0) {
                foreach (var file in files) {
                    if (file.Length > 0) {
                        using (var memoryStream = new MemoryStream()) {
                            await file.CopyToAsync(memoryStream);
                            byte[] fileBytes = memoryStream.ToArray();

                            // Add file information to SubmittedDocs
                            submittedDocs.Blobs.Add(new Blob { Data = fileBytes });
                            submittedDocs.fileNames.Add(file.FileName);
                            submittedDocs.fileTypes.Add(file.ContentType);
                        }
                    }
                }
            }

            // Save documents to the database if any files are uploaded
            if (submittedDocs.Blobs.Count > 0) {
                _db.SetDocuments(claimId.Value, submittedDocs);
            }

            // Save notes to the database if notes are provided
            if (!string.IsNullOrEmpty(submittedDocs.Notes)) {
                _db.SetNotes(claimId.Value, submittedDocs.Notes);
            }


            Claim claim = _db.GetClaim(claimId.Value);
            int automation_result = Automation.AutomatePayOrReject(claim.Hours, claim.HourlyRate);

            if (automation_result == 1) {
                _db.MarkClaimAsPaid(claimId.Value);
            } else if (automation_result == 0) {
                _db.MarkClaimAsUnpaid(claimId.Value);
            } else {
                _db.MarkClaimAsRejected(claimId.Value);
            }


            // Redirect after successful upload
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DownloadInvoice(int claimId) {
            // Retrieve the claim from the database
            Claim claim = _db.GetClaim(claimId);
            if (claim == null) {
                return NotFound("Claim not found.");
            }

            // Generate the PDF using your existing PdfGenerator class
            FileContentResult pdfFile = PdfGenerator.GenerateInvoice(claim);

            // Return the PDF file
            return pdfFile;
        }

    }
}
