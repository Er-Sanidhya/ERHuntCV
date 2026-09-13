using HuntCV_Portal.Models;
using HuntCV_Portal.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace HuntCV_Portal.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountRepository _accountRepository;
        private readonly IConfiguration _configuration;

        public AccountController(
            AccountRepository accountRepository,
            IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
        }


        [HttpGet]
        public IActionResult OrganizationLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrganizationLogin(OrganizationLoginM model)
        {
            // =================================================
            // VALIDATE MODEL
            // =================================================

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            // =================================================
            // LOGIN ORGANIZATION
            // =================================================

            OrganizationRegisterM? organization =
                _accountRepository.OrganizationLogin(model);


            // =================================================
            // INVALID LOGIN
            // =================================================

            if (organization == null)
            {
                ModelState.AddModelError(
                    "",
                    "Invalid email or password.");

                return View(model);
            }


            // =================================================
            // ORGANIZATION ID
            // =================================================

            int organizationId = organization.nID;


            // =================================================
            // GET IP ADDRESS
            // =================================================

            string ipAddress =
                HttpContext.Connection
                    .RemoteIpAddress?.ToString()
                ?? "Unknown";

            if (ipAddress == "::1")
            {
                ipAddress = "127.0.0.1";
            }


            // =================================================
            // GET BROWSER / ACCESS TYPE
            // =================================================

            string userAgent =
                HttpContext.Request.Headers["User-Agent"]
                    .ToString();

            string accessType = "Unknown";


            if (userAgent.Contains(
                "Edg",
                StringComparison.OrdinalIgnoreCase))
            {
                accessType = "Microsoft Edge";
            }
            else if (userAgent.Contains(
                "Chrome",
                StringComparison.OrdinalIgnoreCase))
            {
                accessType = "Google Chrome";
            }
            else if (userAgent.Contains(
                "Firefox",
                StringComparison.OrdinalIgnoreCase))
            {
                accessType = "Mozilla Firefox";
            }
            else if (userAgent.Contains(
                "Safari",
                StringComparison.OrdinalIgnoreCase))
            {
                accessType = "Safari";
            }
            else if (userAgent.Contains(
                "Opera",
                StringComparison.OrdinalIgnoreCase))
            {
                accessType = "Opera";
            }


            // =================================================
            // LOCATION
            // =================================================

            string location = "Localhost";


            // =================================================
            // CONNECTION STRING
            // =================================================

            string connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection");


            // =================================================
            // SAVE ORGANIZATION ACTIVITY LOG
            // =================================================

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            INSERT INTO dbo.tblOrganizationActivityLog
            (
                OrgID,
                AccessType,
                IPAddress,
                Location,
                DateTime
            )
            VALUES
            (
                @OrgID,
                @AccessType,
                @IPAddress,
                @Location,
                GETDATE()
            )";


                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@OrgID",
                        SqlDbType.Int).Value =
                            organizationId;


                    cmd.Parameters.Add(
                        "@AccessType",
                        SqlDbType.NVarChar,
                        100).Value =
                            accessType;


                    cmd.Parameters.Add(
                        "@IPAddress",
                        SqlDbType.NVarChar,
                        100).Value =
                            ipAddress;


                    cmd.Parameters.Add(
                        "@Location",
                        SqlDbType.NVarChar,
                        250).Value =
                            location;


                    cmd.ExecuteNonQuery();
                }
            }


            // =================================================
            // SET ORGANIZATION SESSION
            // =================================================

            HttpContext.Session.SetInt32(
                "OrgID",
                organization.nID);

            HttpContext.Session.SetString(
                "OrgName",
                organization.sOrgName ?? "");

            HttpContext.Session.SetString(
                "OrgEmail",
                organization.sEmail ?? "");


            // =================================================
            // REDIRECT TO ORGANIZATION DASHBOARD
            // =================================================

            return RedirectToAction(
                "Dashboard",
                "Organization");
        }

        [HttpGet]
        public IActionResult OrganizationRegister()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrganizationRegister(OrganizationRegisterM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                int result = _accountRepository.RegisterOrganization(model);

                if (result > 0)
                {
                    TempData["SuccessMessage"] =
                        "Organization registered successfully.";

                    return RedirectToAction("OrganizationLogin", "Account");
                }

                ModelState.AddModelError(
                    "",
                    "Organization registration failed."
                );

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    "An error occurred: " + ex.Message
                );

                return View(model);
            }
        }



        // =========================
        // Candidate Login
        // =========================
        [HttpGet]
        public IActionResult CandidateLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CandidateLogin(CandidateLoginM model)
        {
            // =================================================
            // VALIDATE MODEL
            // =================================================

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            // =================================================
            // LOGIN CANDIDATE
            // =================================================

            CandidateRegisterM? candidate =
                _accountRepository.LoginCandidate(model);


            // =================================================
            // INVALID LOGIN
            // =================================================

            if (candidate == null)
            {
                ViewBag.Error =
                    "Invalid email or password.";

                return View(model);
            }


            // =================================================
            // CANDIDATE ID
            // =================================================

            int candidateId = candidate.nID;


            // =================================================
            // GET IP ADDRESS
            // =================================================

            string ipAddress =
                HttpContext.Connection
                    .RemoteIpAddress?.ToString()
                ?? "Unknown";

            if (ipAddress == "::1")
            {
                ipAddress = "127.0.0.1";
            }


            // =================================================
            // GET BROWSER / ACCESS TYPE
            // =================================================

            string userAgent =
                HttpContext.Request.Headers["User-Agent"]
                    .ToString();

            string accessType = "Unknown";


            if (userAgent.Contains(
                "Edg",
                StringComparison.OrdinalIgnoreCase))
            {
                accessType = "Microsoft Edge";
            }
            else if (userAgent.Contains(
                "Chrome",
                StringComparison.OrdinalIgnoreCase))
            {
                accessType = "Google Chrome";
            }
            else if (userAgent.Contains(
                "Firefox",
                StringComparison.OrdinalIgnoreCase))
            {
                accessType = "Mozilla Firefox";
            }
            else if (userAgent.Contains(
                "Safari",
                StringComparison.OrdinalIgnoreCase))
            {
                accessType = "Safari";
            }
            else if (userAgent.Contains(
                "Opera",
                StringComparison.OrdinalIgnoreCase))
            {
                accessType = "Opera";
            }


            // =================================================
            // LOCATION
            // =================================================

            string location = "Localhost";


            // =================================================
            // CONNECTION STRING
            // =================================================

            string connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection");


            // =================================================
            // SAVE CANDIDATE ACTIVITY LOG
            // =================================================

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            INSERT INTO dbo.tblCandidateActivityLog
            (
                CandidateID,
                AccessType,
                IPAddress,
                Location,
                DateTime
            )
            VALUES
            (
                @CandidateID,
                @AccessType,
                @IPAddress,
                @Location,
                GETDATE()
            )";


                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@CandidateID",
                        SqlDbType.Int).Value =
                            candidateId;


                    cmd.Parameters.Add(
                        "@AccessType",
                        SqlDbType.NVarChar,
                        100).Value =
                            accessType;


                    cmd.Parameters.Add(
                        "@IPAddress",
                        SqlDbType.NVarChar,
                        100).Value =
                            ipAddress;


                    cmd.Parameters.Add(
                        "@Location",
                        SqlDbType.NVarChar,
                        250).Value =
                            location;


                    cmd.ExecuteNonQuery();
                }
            }


            // =================================================
            // SET CANDIDATE SESSION
            // =================================================

            HttpContext.Session.SetInt32(
                "CandidateID",
                candidate.nID);

            HttpContext.Session.SetString(
                "CandidateName",
                candidate.sFName ?? "");

            HttpContext.Session.SetString(
                "CandidateEmail",
                candidate.sEmail ?? "");


            // =================================================
            // REDIRECT TO CANDIDATE DASHBOARD
            // =================================================

            return RedirectToAction(
                "Dashboard",
                "Candidate");
        }

        // =========================
        // Candidate Registration
        // =========================
        [HttpGet]
        public IActionResult CandidateRegistration()
        {
            return View();
        }


        // =========================
        // Candidate Registration POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CandidateRegistration(CandidateRegisterM model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {

                // ==========================================
                // PROFILE IMAGE UPLOAD
                // ==========================================

                if (model.ProfileImageFile != null &&
                    model.ProfileImageFile.Length > 0)
                {
                    // ==========================================
                    // ALLOWED IMAGE TYPES
                    // ==========================================

                    string[] allowedExtensions =
                    {
    ".png",
    ".jpg",
    ".jpeg"
};

                    string extension =
                        Path.GetExtension(
                            model.ProfileImageFile.FileName
                        ).ToLowerInvariant();

                    // ==========================================
                    // CHECK IMAGE EXTENSION
                    // ==========================================

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError(
                            "ProfileImageFile",
                            "Only PNG, JPG and JPEG images are allowed."
                        );

                        return View(model);
                    }

                    // ==========================================
                    // MAXIMUM 5 MB
                    // ==========================================

                    if (model.ProfileImageFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError(
                            "ProfileImageFile",
                            "Image size must be less than 5 MB."
                        );

                        return View(model);
                    }

                    // ==========================================
                    // CREATE UPLOAD FOLDER
                    // ==========================================

                    string uploadFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads",
                        "candidates"
                    );

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    // ==========================================
                    // GENERATE IMAGE NAME: 1, 2, 3, 4...
                    // ==========================================

                    int nextNumber = 1;

                    while (Directory.GetFiles(
                        uploadFolder,
                        nextNumber + ".*"
                    ).Length > 0)
                    {
                        nextNumber++;
                    }

                    // Example: 1.jpg, 2.png, 3.jpeg
                    string imageName =
                        nextNumber + extension;

                    // ==========================================
                    // SAVE IMAGE
                    // ==========================================

                    string filePath = Path.Combine(
                        uploadFolder,
                        imageName
                    );

                    using (FileStream stream =
                           new FileStream(
                               filePath,
                               FileMode.Create))
                    {
                        model.ProfileImageFile.CopyTo(stream);
                    }

                    // ==========================================
                    // ONLY IMAGE NAME GOES TO DATABASE
                    // ==========================================

                    model.sProfileImage = imageName;
                }
                else
                {
                    model.sProfileImage = "";
                }

                int candidateID =
                    _accountRepository.Register(model);

                if (candidateID > 0)
                {
                    TempData["SuccessMessage"] =
                        "Candidate registration successful.";

                    return RedirectToAction(
                        "CandidateLogin",
                        "Account");
                }

                ModelState.AddModelError(
                    "",
                    "Candidate registration failed.");

                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    "",
                    "An error occurred while registering candidate.");

                return View(model);
            }
        }



        public IActionResult ActivityLog()
        {
            // =================================================
            // CHECK CANDIDATE
            // =================================================

            int? candidateId =
                HttpContext.Session.GetInt32("CandidateID");

            if (candidateId != null && candidateId > 0)
            {
                return RedirectToAction(
                    "ActivityLog",
                    "Candidate");
            }


            // =================================================
            // CHECK ORGANIZATION
            // =================================================

            int? orgId =
                HttpContext.Session.GetInt32("OrgID");

            if (orgId != null && orgId > 0)
            {
                return RedirectToAction(
                    "ActivityLog",
                    "Organization");
            }


            // =================================================
            // CHECK SUPER ADMIN
            // IMPORTANT: Super Admin uses SAID
            // =================================================

            string? saId =
                HttpContext.Session.GetString("SAID");

            if (int.TryParse(
                saId,
                out int superAdminId) &&
                superAdminId > 0)
            {
                return RedirectToAction(
                    "ActivityLog",
                    "SuperAdmin");
            }


            // =================================================
            // NO LOGIN SESSION
            // =================================================

            return RedirectToAction(
                "Index",
                "SALogin");
        }
    }



}