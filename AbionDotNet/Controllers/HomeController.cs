﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Net.Mail;
using System.Web;
using System.Net;
using System.Data.Entity;
using System.Web.Mvc;
using AbionDotNet.DAL;
using AbionDotNet.Models;

namespace AbionDotNet.Controllers
{
    public class HomeController : Controller
    {

        private AbionDotNetContext db = new AbionDotNetContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WebApp()
        {
            return View();
        }

        public ActionResult Mobile()
        {
            return View();
        }

        public ActionResult Azure()
        {
            return View();
        }

        public ActionResult SocialMedia()
        {
            return View();
        }

        public ActionResult WebAdvertising()
        {
            return View();
        }

        public ActionResult CustomApp()
        {
            return View();
        }

        // Contact GET
        public ActionResult Contact()
        {
            ViewBag.Message = "Abion Technology Contact";

            // build select list of contact categories
            ViewBag.Categories = new SelectList(db.ContactCategories.OrderBy(x => x.sortorder), "ID", "category");

            return View();
        }

        // Contact POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact([Bind(Include = "ContactName, ContactEmail, ContactPhone, ContactCategories_ID, WebMessage")] EmailContact emailContact)
        {
            ViewBag.Message = "Abion Technology Contact";

            try
            {
                // update database with contact information
                if (ModelState.IsValid)
                {

                    db.EmailContacts.Add(emailContact);
                    db.SaveChanges();

                    // create Gmailer object and initialize data
                    OutlookMailer mailer = new OutlookMailer();
                    mailer.ToEmail = "michael.g.workman@gmail.com";
                    mailer.FromEmail = emailContact.ContactEmail;
                    mailer.FromName = emailContact.ContactName;

                    // get the email category and set the email Body
                    string emailCategory = db.ContactCategories.Where(x => x.ID == emailContact.ContactCategories_ID).SingleOrDefault().category;
                    mailer.Subject = "Abion Website Inquiry - Category: " + emailCategory;
                    mailer.Body = "From Name: " + emailContact.ContactName + " From Email: " + emailContact.ContactEmail +  " Phone: " + emailContact.ContactPhone + "<br>" + emailContact.WebMessage;

                    // send email
                    mailer.Send();

                    return View("ContactConfirmation");
                }
                else
                {
                    ViewBag.ErrorMessage = "Contact View, Model State Not Valid, Email Not Sent.";

                    return View("~/Views/Shared/Error.cshtml");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error Encountered: " + ex.Message + " Inner Exception: " + ex.InnerException;
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        // mailer class courtesy of stackoverflow.com
        public class OutlookMailer
        {
            public static string OutlookUsername { get; set; }
            public static string OutlookPassword { get; set; }
            public static string OutlookHost { get; set; }
            public static int OutlookPort { get; set; }
            public static bool OutlookSSL { get; set; }
            public string ToEmail { get; set; }
            public string FromEmail { get; set; }
            public string FromName { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }

            static OutlookMailer()
            {
                OutlookHost = "smtp-mail.outlook.com";
                OutlookPort = 587;
                OutlookSSL = true;
            }

            public void Send()
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Host = OutlookHost;
                smtp.Port = OutlookPort;
                smtp.EnableSsl = OutlookSSL;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("michael.g.workman@outlook.com", "REDACTED");

                MailMessage message = new MailMessage();
                message.From = new MailAddress("michael.g.workman@outlook.com");
                message.To.Add(new MailAddress(ToEmail));
                message.Subject = Subject;
                message.Body = Body;
                message.IsBodyHtml = true;
                smtp.Send(message);
            }
        }
    }
}