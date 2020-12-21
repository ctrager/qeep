using System;
using System.Collections.Generic;
using System.Data;
using MimeKit;
using System.Threading;

namespace keep
{
    public static class bd_email
    {

        static List<string[]> filter_lines = new List<string[]>();

        public static void queue_email(string type, string to, string subject, string body, int post_id = 0)
        {
            string sql = @"insert into outgoing_email_queue
                (oq_email_type, oq_email_to, oq_email_subject, oq_email_body, oq_post_id)
                values(@oq_email_type, @oq_email_to, @oq_email_subject, @oq_email_body, @oq_post_id)";

            var dict = new Dictionary<string, dynamic>();
            dict["@oq_email_type"] = type;
            dict["@oq_email_to"] = to;
            dict["@oq_email_subject"] = subject;
            dict["@oq_email_body"] = body;
            dict["@oq_post_id"] = post_id;

            bd_db.exec(sql, dict);

            spawn_email_sending_thread();

        }

        public static void spawn_email_sending_thread()
        {
            // Spawn sending thread
            // The thread doesn't loop. It sends and then dies.
            // But we spawn it every time we put an email into the outgoing queue.
            Thread thread = new Thread(threadproc_send_emails);
            thread.Start();
        }

        public static void spawn_registration_request_expiration_thread()
        {

            // Spawn sending thread
            // loops, sleeps
            Thread thread = new Thread(threadproc_expire_registration_requests);
            thread.Start();
        }


        // read the table outgoing_email_queue, try to send an email for each row
        // and if good, delete the row
        static void threadproc_send_emails()
        {
            var sql = @"select * from outgoing_email_queue 
                where oq_sending_attempt_count < @max_retries
                order by oq_id desc";

            var dict = new Dictionary<string, dynamic>();
            dict["@max_retries"] = bd_config.get(bd_config.MaxNumberOfSendingRetries);

            DataTable dt = bd_db.get_datatable(sql, dict);

            foreach (DataRow dr in dt.Rows)
            {
                int oq_id = (int)dr[0];
                bd_util.log("trying to send " + oq_id.ToString());

                try
                {
                    send_email(
                        (string)dr["oq_email_to"],
                        (string)dr["oq_email_subject"],
                        (string)dr["oq_email_body"],
                        (int)dr["oq_post_id"]);

                    // Done, good, so delete from queue
                    sql = "delete from outgoing_email_queue where oq_id = " + oq_id.ToString();
                    bd_db.exec(sql);
                }
                catch (Exception exception)
                {
                    bd_util.log(exception.Message, Serilog.Events.LogEventLevel.Error);
                    bd_util.log(exception.StackTrace, Serilog.Events.LogEventLevel.Error);
                    increment_retry_count(oq_id, exception.Message);
                }
            }
        }

        static void increment_retry_count(int oq_id, string exception_message)
        {
            var sql = @"update outgoing_email_queue 
                        set oq_sending_attempt_count = oq_sending_attempt_count + 1, 
                        oq_last_sending_attempt_date = CURRENT_TIMESTAMP,
                        oq_last_exception = @exception
                        where oq_id = @oq_id";
            var dict = new Dictionary<string, dynamic>();
            dict["@exception"] = exception_message;
            dict["@oq_id"] = oq_id;

            bd_db.exec(sql, dict);
        }


        static void send_email(string to, string subject, string body_text, int post_id = 0)
        {

            var message = new MimeMessage();

            string[] addresses = to.Split(",");
            for (int i = 0; i < addresses.Length; i++)
            {
                var parsed_address = new System.Net.Mail.MailAddress(addresses[i]);
                message.To.Add(new MailboxAddress(
                    parsed_address.DisplayName,
                    parsed_address.Address));
            }
            message.Subject = subject;
            bd_util.log("send_email to: " + to);
            bd_util.log("email subject: " + subject);

            var multipart = new Multipart("mixed");

            // plain text body
            var body = new TextPart("plain")
            {
                Text = body_text
            };
            multipart.Add(body);

            message.Body = multipart;
            smtp_send(message);
        }



        static void smtp_send(MimeMessage message)
        {

            // all our outgoing emails get sent from what's
            // configured in config file
            var from_address = new MailboxAddress(
                bd_config.get(bd_config.OutgoingEmailDisplayName),
                bd_config.get(bd_config.SmtpUser));

            message.From.Add(from_address);

            if (bd_config.get(bd_config.DebugSkipSendingEmails) == 0)
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(
                        bd_config.get(bd_config.SmtpHost),
                        bd_config.get(bd_config.SmtpPort),
                        MailKit.Security.SecureSocketOptions.Auto);

                    string smtp_user = bd_config.get(bd_config.SmtpUser);
                    string smtp_password = bd_config.get(bd_config.SmtpPassword);
                    client.Authenticate(smtp_user, smtp_password);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }

        }

        static void threadproc_fetch_incoming_messages()
        {
            while (true)
            {

                // config in seconds, func expects milliseconds
                int milliseconds = 1000 * bd_config.get(bd_config.SecondsToSleepAfterCheckingIncomingEmail);
                System.Threading.Thread.Sleep(milliseconds);
            }
        }


        public static bool validate_email_address(string address)
        {
            //if (!EmailValidation.EmailValidator.Validate(address))
            try
            {
                System.Net.Mail.MailAddress ma = new System.Net.Mail.MailAddress(address);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        static void threadproc_expire_registration_requests()
        {
            while (true)
            {

                string sql;
                int hours;
                DateTime time_in_past;
                var dict = new Dictionary<string, dynamic>();

                // Regisgtrations
                hours = bd_config.get(bd_config.RegistrationRequestExpirationInHours);
                time_in_past = DateTime.Now.AddHours(-1 * hours);
                dict["@date"] = time_in_past;

                sql = @"delete from registration_requests 
                where rr_is_invitation = false and rr_created_date < @date";

                bd_db.exec(sql, dict);

                // Invitations
                hours = bd_config.get(bd_config.InviteUserExpirationInHours);
                time_in_past = DateTime.Now.AddHours(-1 * hours);
                dict["@date"] = time_in_past;

                sql = @"delete from registration_requests 
                where rr_is_invitation = true and rr_created_date < @date";

                bd_db.exec(sql, dict);

                // sleep for an hour
                int milliseconds = 1000 * 60 * 60;
                System.Threading.Thread.Sleep(milliseconds);
            }
        }
    }
}
