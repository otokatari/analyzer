using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;

namespace UserAnalyzer
{
    public static class Utils
    {
        public static int ExecuteCommand(string program, out string stdout, out string stderr, params string[] arguments)
        {
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = program;
            pProcess.StartInfo.Arguments = ' '.Connect(arguments);
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.RedirectStandardError = true;
            pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
            stdout = pProcess.StandardOutput.ReadToEnd();
            stderr = pProcess.StandardError.ReadToEnd();
            pProcess.WaitForExit();
            return pProcess.ExitCode;
        }

        public static string Connect(this char delimiter, params string[] slices)
                        => new StringBuilder().AppendJoin(delimiter, slices).ToString();


        public static bool ContainAllKeys<TJsonObject>(this TJsonObject root,params string[] keys) where TJsonObject: JObject
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (!root.ContainsKey(keys[i])) return false;
                if (i <= keys.Length - 2)
                {
                    if(root[keys[i]].Type != JTokenType.Object) return false;
                    root = root[keys[i]].Value<TJsonObject>();
                } 
            }
            return true;
        }
    }
}