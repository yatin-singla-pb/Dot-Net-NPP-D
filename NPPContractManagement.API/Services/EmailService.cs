using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;

namespace NPPContractManagement.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(ISendGridClient sendGridClient, IConfiguration configuration, ILogger<EmailService> logger)
        {
            _sendGridClient = sendGridClient;
            _configuration = configuration;
            _logger = logger;

            // Guard against blocked SMTP port 25 if SMTP is configured
            var smtpPortStr = _configuration["Email:Smtp:Port"] ?? _configuration["SendGrid:Smtp:Port"]; // optional config
            if (int.TryParse(smtpPortStr, out var smtpPort) && smtpPort == 25)
            {
                _logger.LogWarning("SMTP port 25 is not allowed. Please use port 587 (STARTTLS) or 465 (SSL).");
            }
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            try
            {
                // Choose provider: default SendGrid, optional SMTP if enabled
                var useSmtp = _configuration.GetValue<bool>("Email:UseSmtp", false);

                var fromEmail = _configuration["SendGrid:FromEmail"] ?? _configuration["Email:FromEmail"] ?? "noreply@nppcontractmanagement.com";
                var fromName = _configuration["SendGrid:FromName"] ?? _configuration["Email:FromName"] ?? "NPP Contract Management";

                if (useSmtp)
                {
                    var host = _configuration["Email:Smtp:Host"] ?? "smtp.sendgrid.net";
                    var port = _configuration.GetValue<int?>("Email:Smtp:Port") ?? 587;
                    if (port == 25) { _logger.LogWarning("Overriding disallowed SMTP port 25 to 587."); port = 587; }
                    var enableSsl = _configuration.GetValue<bool?>("Email:Smtp:Ssl") ?? (port == 465);
                    var username = _configuration["Email:Smtp:User"];
                    var password = _configuration["Email:Smtp:Pass"];

                    using var client = new SmtpClient(host, port)
                    {
                        EnableSsl = enableSsl,
                        Credentials = string.IsNullOrWhiteSpace(username) ? CredentialCache.DefaultNetworkCredentials : new NetworkCredential(username, password)
                    };

                    using var mail = new MailMessage
                    {
                        From = new MailAddress(fromEmail, fromName),
                        Subject = subject,
                        Body = htmlContent,
                        IsBodyHtml = true
                    };
                    mail.To.Add(new MailAddress(toEmail));

                    await client.SendMailAsync(mail);
                    _logger.LogInformation("SMTP email sent successfully to {Email}", toEmail);
                    return true;
                }
                else
                {
                    var from = new EmailAddress(fromEmail, fromName);
                    var to = new EmailAddress(toEmail);
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);

                    var response = await _sendGridClient.SendEmailAsync(msg);

                    if (response.StatusCode == System.Net.HttpStatusCode.Accepted || response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogInformation("SendGrid email sent successfully to {Email}", toEmail);
                        return true;
                    }
                    else
                    {
                        var responseBody = await response.Body.ReadAsStringAsync();
                        _logger.LogError("Failed to send SendGrid email to {Email}. Status: {Status}, Body: {Body}", toEmail, response.StatusCode, responseBody);
                        return false;
                    }
                }
            }
            catch (SmtpException smtpex)
            {
                _logger.LogError(smtpex, "SMTP error sending email to {Email}", toEmail);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendUserInvitationEmailAsync(string toEmail, string userName, string invitationLink)
        {
            var subject = "Welcome to NPP Contract Management - Set Your Password";
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Welcome to NPP Contract Management</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ padding: 30px; background-color: #f8f9fa; border-radius: 0 0 8px 8px; }}
                        .button {{ display: inline-block; padding: 12px 24px; background-color: #007bff; color: white; text-decoration: none; border-radius: 4px; margin: 20px 0; font-weight: bold; }}
                        .footer {{ padding: 20px; text-align: center; color: #666; font-size: 12px; }}
                        .warning {{ background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 4px; margin: 15px 0; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 style='margin: 0;'>Welcome to NPP Contract Management</h1>
                        </div>
                        <div class='content'>
                            <h2>Hello {userName},</h2>
                            <p>You have been invited to join the NPP Contract Management system. This platform will help you manage contracts, manufacturers, distributors, and related business processes efficiently.</p>
                            <p>To get started, please click the button below to set your password and activate your account:</p>
                            <p style='text-align: center;'>
                                <a href='{invitationLink}' class='button'>Set Your Password</a>
                            </p>
                            <div class='warning'>
                                <strong>Important:</strong> This invitation link will expire in 24 hours for security reasons.
                            </div>
                            <p>If you have any questions or need assistance, please contact your system administrator.</p>
                            <p>Best regards,<br><strong>NPP Contract Management Team</strong></p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 NPP Contract Management System. All rights reserved.</p>
                            <p>This is an automated message. Please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(toEmail, subject, htmlContent);
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string userName, string resetLink)
        {
            var subject = "Password Reset Request - NPP Contract Management";
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Password Reset Request</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #dc3545; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ padding: 30px; background-color: #f8f9fa; border-radius: 0 0 8px 8px; }}
                        .button {{ display: inline-block; padding: 12px 24px; background-color: #dc3545; color: white; text-decoration: none; border-radius: 4px; margin: 20px 0; font-weight: bold; }}
                        .footer {{ padding: 20px; text-align: center; color: #666; font-size: 12px; }}
                        .warning {{ background-color: #f8d7da; border: 1px solid #f5c6cb; padding: 15px; border-radius: 4px; margin: 15px 0; color: #721c24; }}
                        .security-note {{ background-color: #d1ecf1; border: 1px solid #bee5eb; padding: 15px; border-radius: 4px; margin: 15px 0; color: #0c5460; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 style='margin: 0;'>Password Reset Request</h1>
                        </div>
                        <div class='content'>
                            <h2>Hello {userName},</h2>
                            <p>You have requested to reset your password for the NPP Contract Management system.</p>
                            <p>To reset your password, please click the button below:</p>
                            <p style='text-align: center;'>
                                <a href='{resetLink}' class='button'>Reset Your Password</a>
                            </p>
                            <div class='warning'>
                                <strong>Important:</strong> This password reset link will expire in 1 hour for security reasons.
                            </div>
                            <div class='security-note'>
                                <strong>Security Notice:</strong> If you did not request this password reset, please ignore this email and contact your system administrator immediately.
                            </div>
                            <p>Best regards,<br><strong>NPP Contract Management Team</strong></p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 NPP Contract Management System. All rights reserved.</p>
                            <p>This is an automated message. Please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(toEmail, subject, htmlContent);
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var subject = "Welcome to NPP Contract Management - Account Activated";
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Welcome to NPP Contract Management</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #28a745; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ padding: 30px; background-color: #f8f9fa; border-radius: 0 0 8px 8px; }}
                        .button {{ display: inline-block; padding: 12px 24px; background-color: #28a745; color: white; text-decoration: none; border-radius: 4px; margin: 20px 0; font-weight: bold; }}
                        .footer {{ padding: 20px; text-align: center; color: #666; font-size: 12px; }}
                        .success {{ background-color: #d4edda; border: 1px solid #c3e6cb; padding: 15px; border-radius: 4px; margin: 15px 0; color: #155724; }}
                        .features {{ background-color: #e2e3e5; padding: 20px; border-radius: 4px; margin: 15px 0; }}
                        .features ul {{ margin: 0; padding-left: 20px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 style='margin: 0;'>🎉 Welcome to NPP Contract Management</h1>
                        </div>
                        <div class='content'>
                            <h2>Hello {userName},</h2>
                            <div class='success'>
                                <strong>Congratulations!</strong> Your account has been successfully activated.
                            </div>
                            <p>You can now log in to the NPP Contract Management system and start managing your business processes efficiently.</p>
                            <div class='features'>
                                <h3>What you can do with NPP Contract Management:</h3>
                                <ul>
                                    <li>Manage contracts and track their status</li>
                                    <li>Maintain manufacturer and distributor information</li>
                                    <li>Monitor product catalogs and pricing</li>
                                    <li>Generate reports and analytics</li>
                                    <li>Collaborate with team members</li>
                                </ul>
                            </div>
                            <p style='text-align: center;'>
                                <a href='http://localhost:4200/login' class='button'>Login to Your Account</a>
                            </p>
                            <p>If you have any questions or need assistance getting started, please contact your system administrator.</p>
                            <p>Best regards,<br><strong>NPP Contract Management Team</strong></p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 NPP Contract Management System. All rights reserved.</p>
                            <p>This is an automated message. Please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(toEmail, subject, htmlContent);
        }

        public async Task<bool> SendTemporaryPasswordEmailAsync(string toEmail, string userName, string temporaryPassword)
        {
            var subject = "Your Temporary Password";
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Your Temporary Password</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #0069d9; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ padding: 30px; background-color: #f8f9fa; border-radius: 0 0 8px 8px; }}
                        .footer {{ padding: 20px; text-align: center; color: #666; font-size: 12px; }}
                        .password-box {{ background: #fff; border: 1px solid #e1e1e1; padding: 16px; border-radius: 6px; font-size: 18px; font-weight: bold; text-align: center; letter-spacing: 0.5px; }}
                        .note {{ background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 12px; border-radius: 4px; margin-top: 16px; }}
                        .btn {{ display:inline-block; margin-top: 16px; padding: 10px 18px; background:#0069d9; color:#fff; text-decoration:none; border-radius:4px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 style='margin: 0;'>Your Temporary Password</h1>
                        </div>
                        <div class='content'>
                            <p>Hello {userName},</p>
                            <p>Your account has been created. Use the temporary password below to sign in:</p>
                            <div class='password-box'>{temporaryPassword}</div>
                            <div class='note'>
                                <strong>Important:</strong> For security, you should reset your password immediately after your first login. Do not share this password with anyone.
                            </div>
                            <p>
                                <a href='http://localhost:4200/login' class='btn'>Go to Login</a>
                            </p>
                            <p>If you did not expect this email, please contact your administrator.</p>
                            <p>Regards,<br><strong>NPP Contract Management Team</strong></p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 NPP Contract Management System. All rights reserved.</p>
                            <p>This is an automated message. Please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(toEmail, subject, htmlContent);
        }

        public async Task<bool> SendProposalRequestedEmailAsync(string toEmail, string userName, string proposalTitle, int proposalId, DateTime? startDate, DateTime? endDate)
        {
            var subject = "New Proposal Request from NPP";
            var dateRange = (startDate.HasValue || endDate.HasValue)
                ? $"<p><strong>Proposed Term:</strong> {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}</p>"
                : string.Empty;

            var htmlContent = $@"\n                <!DOCTYPE html>\n                <html>\n                <head>\n                    <meta charset='utf-8'>\n                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>\n                    <title>New Proposal Request</title>\n                    <style>\n                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }}\n                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}\n                        .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}\n                        .content {{ padding: 30px; background-color: #f8f9fa; border-radius: 0 0 8px 8px; }}\n                        .footer {{ padding: 20px; text-align: center; color: #666; font-size: 12px; }}\n                    </style>\n                </head>\n                <body>\n                    <div class='container'>\n                        <div class='header'>\n                            <h1 style='margin: 0;'>New Proposal Request</h1>\n                        </div>\n                        <div class='content'>\n                            <p>Hello {userName},</p>\n                            <p>An NPP user has created a new proposal request that is assigned to your manufacturer.</p>\n                            <p><strong>Proposal:</strong> {WebUtility.HtmlEncode(proposalTitle ?? "(no title)")} (ID: {proposalId})</p>\n                            {dateRange}\n                            <p>Please sign in to the NPP Contract Management system to review and take any required action.</p>\n                            <p>Best regards,<br><strong>NPP Contract Management Team</strong></p>\n                        </div>\n                        <div class='footer'>\n                            <p>© 2024 NPP Contract Management System. All rights reserved.</p>\n                            <p>This is an automated message. Please do not reply to this email.</p>\n                        </div>\n                    </div>\n                </body>\n                </html>";

            return await SendEmailAsync(toEmail, subject, htmlContent);
        }

        public async Task<bool> SendVelocityJobCompletionEmailAsync(string toEmail, string userName, string jobId, int totalRows, int successRows, int failedRows, string status, string detailsUrl)
        {
            var statusColor = status == "completed" ? "#28a745" : status == "completed_with_errors" ? "#ffc107" : "#dc3545";
            var statusText = status == "completed" ? "Completed Successfully" : status == "completed_with_errors" ? "Completed with Errors" : "Failed";

            var subject = $"Velocity Job {jobId} - {statusText}";
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Velocity Job Completed</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: {statusColor}; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ padding: 30px; background-color: #f8f9fa; border-radius: 0 0 8px 8px; }}
                        .stats {{ background-color: white; padding: 15px; border-radius: 5px; margin: 20px 0; }}
                        .stat-row {{ display: flex; justify-content: space-between; padding: 8px 0; border-bottom: 1px solid #e9ecef; }}
                        .stat-label {{ font-weight: bold; }}
                        .button {{ display: inline-block; padding: 12px 24px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px; margin-top: 20px; }}
                        .footer {{ padding: 20px; text-align: center; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 style='margin: 0;'>Velocity Job {statusText}</h1>
                        </div>
                        <div class='content'>
                            <p>Hello {userName},</p>
                            <p>Your velocity data import job <strong>{jobId}</strong> has finished processing.</p>

                            <div class='stats'>
                                <div class='stat-row'>
                                    <span class='stat-label'>Status:</span>
                                    <span>{statusText}</span>
                                </div>
                                <div class='stat-row'>
                                    <span class='stat-label'>Total Rows:</span>
                                    <span>{totalRows:N0}</span>
                                </div>
                                <div class='stat-row'>
                                    <span class='stat-label'>Successfully Processed:</span>
                                    <span style='color: #28a745;'>{successRows:N0}</span>
                                </div>
                                <div class='stat-row'>
                                    <span class='stat-label'>Failed:</span>
                                    <span style='color: #dc3545;'>{failedRows:N0}</span>
                                </div>
                            </div>

                            <p>Click the button below to view the detailed results:</p>
                            <a href='{detailsUrl}' class='button'>View Job Details</a>

                            <p style='margin-top: 30px;'>Best regards,<br><strong>NPP Contract Management Team</strong></p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 NPP Contract Management System. All rights reserved.</p>
                            <p>This is an automated message. Please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(toEmail, subject, htmlContent);
        }

        public async Task<bool> SendRegistrationInvitationEmailAsync(string toEmail, string userName, string registrationLink)
        {
            var subject = "Complete Your Registration - NPP Contract Management";
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Complete Your Registration</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ padding: 30px; background-color: #f8f9fa; border-radius: 0 0 8px 8px; }}
                        .button {{ display: inline-block; padding: 12px 24px; background-color: #007bff; color: white; text-decoration: none; border-radius: 4px; margin: 20px 0; font-weight: bold; }}
                        .footer {{ padding: 20px; text-align: center; color: #666; font-size: 12px; }}
                        .info {{ background-color: #d1ecf1; border: 1px solid #bee5eb; padding: 15px; border-radius: 4px; margin: 15px 0; color: #0c5460; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 style='margin: 0;'>Complete Your Registration</h1>
                        </div>
                        <div class='content'>
                            <h2>Hello {userName},</h2>
                            <p>An account has been created for you in the NPP Contract Management system. To get started, you need to complete your registration by creating your login credentials.</p>
                            <p>Click the button below to begin the registration process:</p>
                            <p style='text-align: center;'>
                                <a href='{registrationLink}' class='button'>Register Your Account</a>
                            </p>
                            <div class='info'>
                                <strong>What you'll need:</strong>
                                <ul>
                                    <li>Your email address (this email)</li>
                                    <li>A 6-digit verification code (sent when you start registration)</li>
                                    <li>A User ID and password of your choice</li>
                                </ul>
                            </div>
                            <p>If you have any questions, please contact your system administrator.</p>
                            <p>Best regards,<br><strong>NPP Contract Management Team</strong></p>
                        </div>
                        <div class='footer'>
                            <p>&copy; 2024 NPP Contract Management System. All rights reserved.</p>
                            <p>This is an automated message. Please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(toEmail, subject, htmlContent);
        }

        public async Task<bool> SendVerificationCodeEmailAsync(string toEmail, string userName, string code)
        {
            var subject = "Your Verification Code - NPP Contract Management";
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Verification Code</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ padding: 30px; background-color: #f8f9fa; border-radius: 0 0 8px 8px; }}
                        .code-box {{ background: #fff; border: 2px solid #007bff; padding: 20px; border-radius: 8px; font-size: 32px; font-weight: bold; text-align: center; letter-spacing: 8px; font-family: 'Courier New', monospace; color: #007bff; margin: 20px 0; }}
                        .footer {{ padding: 20px; text-align: center; color: #666; font-size: 12px; }}
                        .warning {{ background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 4px; margin: 15px 0; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 style='margin: 0;'>Verification Code</h1>
                        </div>
                        <div class='content'>
                            <h2>Hello {userName},</h2>
                            <p>Use the following verification code to complete your registration:</p>
                            <div class='code-box'>{code}</div>
                            <div class='warning'>
                                <strong>Important:</strong> This code will expire in 15 minutes. Do not share this code with anyone.
                            </div>
                            <p>If you did not request this code, please ignore this email.</p>
                            <p>Best regards,<br><strong>NPP Contract Management Team</strong></p>
                        </div>
                        <div class='footer'>
                            <p>&copy; 2024 NPP Contract Management System. All rights reserved.</p>
                            <p>This is an automated message. Please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(toEmail, subject, htmlContent);
        }

        public async Task<bool> SendProposalSubmittedEmailAsync(string toEmail, string userName, string proposalTitle, int proposalId)
        {
            var subject = $"Proposal Submitted - {proposalTitle}";
            var htmlContent = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <div style='background-color: #007bff; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;'>
                        <h1 style='margin: 0;'>Proposal Submitted</h1>
                    </div>
                    <div style='padding: 30px; background-color: #f8f9fa; border-radius: 0 0 8px 8px;'>
                        <p>Hello {System.Net.WebUtility.HtmlEncode(userName)},</p>
                        <p>A proposal has been submitted for your review.</p>
                        <p><strong>Proposal:</strong> {System.Net.WebUtility.HtmlEncode(proposalTitle)} (ID: {proposalId})</p>
                        <p>Please sign in to the NPP Contract Management system to review and take action.</p>
                        <p>Best regards,<br><strong>NPP Contract Management Team</strong></p>
                    </div>
                </div>";
            return await SendEmailAsync(toEmail, subject, htmlContent);
        }

        public async Task<bool> SendProposalAcceptedEmailAsync(string toEmail, string userName, string proposalTitle, int proposalId)
        {
            var subject = $"Proposal Accepted - {proposalTitle}";
            var htmlContent = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <div style='background-color: #28a745; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;'>
                        <h1 style='margin: 0;'>Proposal Accepted</h1>
                    </div>
                    <div style='padding: 30px; background-color: #f8f9fa; border-radius: 0 0 8px 8px;'>
                        <p>Hello {System.Net.WebUtility.HtmlEncode(userName)},</p>
                        <p>Your proposal has been accepted and a contract has been created.</p>
                        <p><strong>Proposal:</strong> {System.Net.WebUtility.HtmlEncode(proposalTitle)} (ID: {proposalId})</p>
                        <p>Best regards,<br><strong>NPP Contract Management Team</strong></p>
                    </div>
                </div>";
            return await SendEmailAsync(toEmail, subject, htmlContent);
        }

        public async Task<bool> SendProposalRejectedEmailAsync(string toEmail, string userName, string proposalTitle, int proposalId, string? rejectReason)
        {
            var subject = $"Proposal Rejected - {proposalTitle}";
            var reasonHtml = string.IsNullOrWhiteSpace(rejectReason)
                ? string.Empty
                : $"<p><strong>Reason:</strong> {System.Net.WebUtility.HtmlEncode(rejectReason)}</p>";
            var htmlContent = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <div style='background-color: #dc3545; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;'>
                        <h1 style='margin: 0;'>Proposal Rejected</h1>
                    </div>
                    <div style='padding: 30px; background-color: #f8f9fa; border-radius: 0 0 8px 8px;'>
                        <p>Hello {System.Net.WebUtility.HtmlEncode(userName)},</p>
                        <p>Your proposal has been rejected.</p>
                        <p><strong>Proposal:</strong> {System.Net.WebUtility.HtmlEncode(proposalTitle)} (ID: {proposalId})</p>
                        {reasonHtml}
                        <p>Please sign in to the NPP Contract Management system for more details.</p>
                        <p>Best regards,<br><strong>NPP Contract Management Team</strong></p>
                    </div>
                </div>";
            return await SendEmailAsync(toEmail, subject, htmlContent);
        }

        public async Task<bool> SendContractExpiryNotificationEmailAsync(string toEmail, string userName, string contractName, int contractId, DateTime endDate, int daysUntilExpiry)
        {
            var subject = $"Contract Expiring Soon - {contractName}";
            var urgencyColor = daysUntilExpiry <= 7 ? "#dc3545" : daysUntilExpiry <= 30 ? "#ffc107" : "#007bff";
            var htmlContent = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <div style='background-color: {urgencyColor}; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;'>
                        <h1 style='margin: 0;'>Contract Expiring Soon</h1>
                    </div>
                    <div style='padding: 30px; background-color: #f8f9fa; border-radius: 0 0 8px 8px;'>
                        <p>Hello {System.Net.WebUtility.HtmlEncode(userName)},</p>
                        <p>The following contract is expiring soon and has not yet been pushed to the proposal workflow:</p>
                        <p><strong>Contract:</strong> {System.Net.WebUtility.HtmlEncode(contractName)} (ID: {contractId})</p>
                        <p><strong>End Date:</strong> {endDate:yyyy-MM-dd}</p>
                        <p><strong>Days Until Expiry:</strong> {daysUntilExpiry}</p>
                        <p>Please take action to renew or create a proposal for this contract.</p>
                        <p>Best regards,<br><strong>NPP Contract Management Team</strong></p>
                    </div>
                </div>";
            return await SendEmailAsync(toEmail, subject, htmlContent);
        }
    }
}
