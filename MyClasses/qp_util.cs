using System;
using System.Security.Cryptography;
using System.Diagnostics;

public class bd { }; // for logging context

namespace qeep
{

    /* 
    
    A big bunch of heterogenous helper functions. Anytime I find myself
    writing code that isn't DRY, I make a function here.
    
    */


    public static class qp_util
    {

        static Serilog.ILogger _logger = null;

        public static void init_serilog_context()
        {
            _logger = Serilog.Log.ForContext<bd>();
        }

        public static void log(object msg, Serilog.Events.LogEventLevel level = Serilog.Events.LogEventLevel.Information)
        {
            string s = System.Threading.Thread.CurrentThread.ManagedThreadId + " " + msg.ToString();

            if (_logger is not null)
            {
                _logger.Write(level, s);
            }
            else
            {
                Console.WriteLine(System.DateTime.Now.ToString("HH-mm-ss-fff") + " " + s);
            }
        }


        // https://stackoverflow.com/questions/4181198/how-to-hash-a-password/10402129#10402129
        public static string compute_password_hash(string password)
        {

            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // 48 chars long
            return Convert.ToBase64String(hashBytes);

        }

        // https://stackoverflow.com/questions/4181198/how-to-hash-a-password/10402129#10402129
        public static bool check_password_against_hash(string entered_password, string saved_hash)
        {
            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(saved_hash);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(entered_password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;

            return true;
        }

        public static string run_command(string command, string args)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = qp_config.get(qp_config.DataFolder),
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            qp_util.log("command line output: " + command + " " + args);
            qp_util.log(output);
            qp_util.log(error);

            process.WaitForExit();

            if (string.IsNullOrEmpty(error))
            {
                return output;
            }
            else
            {
                return error;
            }
        }

    } // end class
}