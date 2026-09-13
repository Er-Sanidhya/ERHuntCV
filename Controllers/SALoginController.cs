using ErJobPortal.Models;
using HuntCV_Portal.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace HuntCV_Portal.Controllers
{
    public class SALoginController : Controller
    {
        private readonly AccountRepository _repository;
        private readonly IConfiguration _configuration;

        public SALoginController(
            AccountRepository repository,
            IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(SALoginM model)
        {
            // =================================================
            // VALIDATE MODEL
            // =================================================

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            // =================================================
            // LOGIN SUPER ADMIN
            // =================================================

            var user =
                _repository.Login(
                    model.sEmail,
                    model.sPassword);


            // =================================================
            // INVALID LOGIN
            // =================================================

            if (user == null)
            {
                ViewBag.Error =
                    "Invalid Email or Password";

                return View(model);
            }


            // =================================================
            // SUPER ADMIN ID
            // =================================================

            int superAdminId = user.nID;


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
            // SAVE SUPER ADMIN ACTIVITY LOG
            // =================================================

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            INSERT INTO dbo.tblSuperAdminActivityLog
            (
                SuperAdminID,
                AccessType,
                IPAddress,
                Location,
                DateTime
            )
            VALUES
            (
                @SuperAdminID,
                @AccessType,
                @IPAddress,
                @Location,
                GETDATE()
            )";


                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@SuperAdminID",
                        SqlDbType.Int).Value =
                            superAdminId;


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
            // STORE SUPER ADMIN DETAILS IN SESSION
            // =================================================

            HttpContext.Session.SetString(
                "SuperAdminID",
                user.nID.ToString());


            HttpContext.Session.SetString(
                "SAName",
                user.sFName);


            HttpContext.Session.SetString(
                "SARole",
                user.sRole);


            // =================================================
            // REDIRECT TO SUPER ADMIN DASHBOARD
            // =================================================

            return RedirectToAction(
                "Dashboard",
                "SuperAdmin",
                new { id = user.nID });
        }
    }
}