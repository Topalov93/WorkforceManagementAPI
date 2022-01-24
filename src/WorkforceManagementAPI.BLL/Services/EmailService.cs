using FluentEmail.Core;
using FluentEmail.Smtp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using WorkforceManagementAPI.Common.Constants;
using WorkforceManagementAPI.Common.ResultState;
using WorkforceManagementAPI.DAL.Entities;

namespace WorkforceManagementAPI.BLL.Services
{
    public class EmailService : IEmailService
    {
        public async Task<ResultState> SendEmail(string receiver, string subject, string template)
        {
            try
            {
                var smtpSender = new SmtpSender(() => new SmtpClient(EmailConstants.SmtpServer)
                {
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(EmailConstants.Username, EmailConstants.Password),
                    Port = EmailConstants.Port
                });

                Email.DefaultSender = smtpSender;

                var email = await Email
                    .From(EmailConstants.AppEmail)
                    .To(receiver)
                    .Subject(subject)
                    .Body(template)
                    .SendAsync();

                return new ResultState(true, "Email has been successfully sent");
            }
            catch (Exception ex)
            {
                return new ResultState(false, "Unable to send Email", ex);
            }
        }

        public async Task<ResultState> SendEmailRequestToTeamLeaders(List<User> teamLeaders, TimeOffRequest timeOffRequest)
        {
            try
            {
                string subject = $"{timeOffRequest.Type} Time-Off Request";

                StringBuilder template = new();
                template.AppendLine("Dear Team Leader,");
                template.AppendLine($"Employee {timeOffRequest.Creator} applies for {timeOffRequest.Type} Time-Off.");
                template.AppendLine($"The Period is from {timeOffRequest.StartDate.ToString("dd-MM-yyyy")} to {timeOffRequest.EndDate.ToString("dd-MM-yyyy")} ({timeOffRequest.Duration} Days). ");
                template.AppendLine($"{timeOffRequest.Description}");
                template.AppendLine("Best Regards!");

                foreach (User teamLeader in teamLeaders)
                {
                    await SendEmail(teamLeader.Email, subject, template.ToString().TrimEnd());
                }

                return new ResultState(true, "Email has been successfully sent to Teamleaders");
            }
            catch (Exception ex)
            {
                return new ResultState(false, "Unable to send Email To Teamleaders", ex);
            }
        }

        public async Task<ResultState> SendEmailNotificationToTeamLeaders(List<User> teamLeaders, TimeOffRequest timeOffRequest, bool isApproved)
        {
            try
            {
                string subject = $"{timeOffRequest.Creator}'s {timeOffRequest.Type} Time-Off Request";

                StringBuilder template = new();
                template.AppendLine("Dear Team Leader,");

                if (isApproved)
                {
                    template.AppendLine($"{timeOffRequest.Creator}'s {timeOffRequest.Type} Time-Off Request has been approved.");
                    template.AppendLine($"The Period is from {timeOffRequest.StartDate.ToString("dd-MM-yyyy")} to {timeOffRequest.EndDate.ToString("dd-MM-yyyy")}.");
                }
                else
                {
                    template.AppendLine($"{timeOffRequest.Creator}'s {timeOffRequest.Type} Time-Off Request (From {timeOffRequest.StartDate.ToString("dd-MM-yyyy")} To {timeOffRequest.EndDate.ToString("dd-MM-yyyy")}) has been rejected.");
                }

                template.AppendLine("Best Regards!");

                foreach (User teamLeader in teamLeaders)
                {
                    await SendEmail(teamLeader.Email, subject, template.ToString().TrimEnd());
                }

                return new ResultState(true, "Email has been successfully sent to Teamleaders");
            }
            catch (Exception ex)
            {
                return new ResultState(false, "Unable to send Email To Teamleaders", ex);
            }
        }

        public async Task<ResultState> SendEmailNotificationToTeamsMembers(List<Team> teams, string currentUserId, TimeOffRequest timeOffRequest, bool isApproved)
        {
            try
            {
                string subject = $"{timeOffRequest.Creator}'s {timeOffRequest.Type} Time-Off Request";

                StringBuilder template = new();
                template.AppendLine("Dear Team Member,");

                if (isApproved)
                {
                    template.AppendLine($"{timeOffRequest.Creator}'s {timeOffRequest.Type} Time-Off Request has been approved.");
                    template.AppendLine($"The Period is from {timeOffRequest.StartDate.ToString("dd-MM-yyyy")} to {timeOffRequest.EndDate.ToString("dd-MM-yyyy")}.");
                }
                else
                {
                    template.AppendLine($"{timeOffRequest.Creator}'s {timeOffRequest.Type} Time-Off Request (From {timeOffRequest.StartDate.ToString("dd-MM-yyyy")} To {timeOffRequest.EndDate.ToString("dd-MM-yyyy")}) has been rejected.");
                }

                template.AppendLine("Best Regards!");

                foreach (Team team in teams)
                {
                    foreach (User teamMember in team.TeamMembers)
                    {
                        if (teamMember.Id != currentUserId)
                        {
                            await SendEmail(teamMember.Email, subject, template.ToString().TrimEnd());
                        }
                    }
                }

                return new ResultState(true, "Email has been successfully sent to Team Members");
            }
            catch (Exception ex)
            {
                return new ResultState(false, "Unable to send Email To Team Members", ex);
            }
        }

        public async Task<ResultState> SendEmailNotificationToUser(User user, TimeOffRequest timeOffRequest, bool isApproved)
        {
            try
            {
                string subject = $"{timeOffRequest.Type} Time-Off Request";

                StringBuilder template = new();
                template.AppendLine("Dear Team Member,");

                if (isApproved)
                {
                    template.AppendLine($"Your {timeOffRequest.Type} Time-Off Request has been approved.");
                    template.AppendLine($"The Period is from {timeOffRequest.StartDate.ToString("dd-MM-yyyy")} to {timeOffRequest.EndDate.ToString("dd-MM-yyyy")}.");
                }
                else
                {
                    template.AppendLine($"Your {timeOffRequest.Type} Time-Off Request (From {timeOffRequest.StartDate.ToString("dd-MM-yyyy")} To {timeOffRequest.EndDate.ToString("dd-MM-yyyy")}) has been rejected.");
                }

                template.AppendLine("Best Regards!");

                await SendEmail(user.Email, subject, template.ToString().TrimEnd());

                return new ResultState(true, "Email has been successfully sent to User");
            }
            catch (Exception ex)
            {
                return new ResultState(false, "Unable to send Email To User", ex);
            }
        }
    }
}
