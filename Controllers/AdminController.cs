using Microsoft.AspNetCore.Mvc;
using Part_2.Models;
using Part_2.Services;

namespace Part_2.Controllers {
    public class AdminController : Controller {
        private readonly Db _db;
        public AdminController() {
            _db = new Db();

        }

        public IActionResult Index() {
            int id = HttpContext.Session.GetInt32("user_id").Value;

            AdminLoggedInModel model = new();
            User user = _db.GetUserById(id);
            List<AdminClaims> claims = _db.GetAdminClaims();
            model.User = user;
            model.Claims = claims;
            return View(model);
        }

        public IActionResult ViewAllClaims() {
            Dictionary<string, List<Claim>> model = _db.GetAllUsersWithClaims();
            return View(model);
        }

        public IActionResult ViewAllContracts() {
            var model = new ViewContractsModel();

            // Assuming _db.GetSubmittedClaimsByUser() returns a dictionary or a method similar for each user
            var submittedClaims = _db.GetSubmittedClaimsByUser();

            foreach (var user in submittedClaims.Keys) {
                var claimsList = new List<ClaimDetails>();

                foreach (var claim in submittedClaims[user]) {
                    var claimDetails = new ClaimDetails {
                        ID = claim.ID,
                        Title = claim.Title,
                        HourlyRate = claim.HourlyRate,
                        Hours = claim.Hours,
                        StartDate = claim.StartDate,
                        EndDate = claim.EndDate,
                        Status = claim.Status,
                        Notes = _db.GetNotesByClaimId(claim.ID), // Get notes for the claim
                        Documents = _db.GetDocumentsByClaimId(claim.ID) // Get documents for the claim
                    };

                    claimsList.Add(claimDetails);
                }

                model.Claims.Add(user, claimsList);
            }

            return View(model);
        }

        public IActionResult DownloadDocument(int claimId, string fileName) {
            // Fetch the document from the database
            var document = _db.GetDocumentByFileName(claimId, fileName);

            if (document == null) {
                return NotFound("Document not found.");
            }

            // Return the file as a downloadable file
            return File(document.Data, document.FileType, document.FileName);
        }




        public IActionResult ManageUsers() {
            ViewAllUsersModel model = _db.GetAllUsers();
            return View(model);
        }

        public IActionResult PaymentHistory() {
            Dictionary<string, List<Claim>> model = _db.GetPaidClaimsByUser();
            return View(model);
        }

        public IActionResult LogOut() {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult UpdatePendingUser(int pendingId, string action) {
            if (action == "confirm") {
                _db.ConfirmPendingUser(pendingId);
            } else if (action == "reject") {
                _db.RejectPendingUser(pendingId);
            }

            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        public IActionResult PayClaim(int claimId, string method) {
            if (method == "pay") {
                _db.MarkClaimAsPaid(claimId);
            } else if (method == "rej") {
                _db.MarkClaimAsRejected(claimId);
            } else {}
            return RedirectToAction("ViewAllContracts");
        }

    }
}