using System;
using System.IO;

namespace WotlkClient.Shared
{
    public static class Log
    {
        public static void WriteLine(LogType type, string format, string prefix, params object[] parameters)
        {
            try
            {
                format = string.Format("[{0}][{1}]{2}", Time.GetTime(), type, (string)format);
                string msg = string.Format(format, parameters);

                if (Config.LogToFile)
                {
                    if (type == LogType.Packet)
                    {
                        StreamWriter packetFile = File.AppendText("logs/" + prefix + "_packet_log.txt");
                        if (parameters.Length > 0)
                            packetFile.WriteLine(parameters[0].ToString());
                        packetFile.Flush();
                        packetFile.Close();
                    }
                    else if(type == LogType.Network)
                    {
                        StreamWriter packetFile = File.AppendText("logs/" + prefix + "_network_log.txt");
                        if(parameters.Length > 0)
                            packetFile.WriteLine(parameters[0].ToString());
                        packetFile.Flush();
                        packetFile.Close();
                    }
                    else if (type == LogType.Error)
                    {
                        StreamWriter packetFile = File.AppendText("logs/" + prefix + "_error_log.txt");
                        if (parameters.Length > 0)
                            packetFile.WriteLine(parameters[0].ToString());
                        packetFile.Flush();
                        packetFile.Close();
                    }
                    else if (type == LogType.Debug)
                    {
                        StreamWriter packetFile = File.AppendText("logs/" + prefix + "_debug_log.txt");
                        if (parameters.Length > 0)
                            packetFile.WriteLine(parameters[0].ToString());
                        packetFile.Flush();
                        packetFile.Close();
                    }
                    else if (type == LogType.Chat)
                    {
                        StreamWriter packetFile = File.AppendText("logs/" + prefix + "_chat_log.txt");
                        if (parameters.Length > 0)
                            packetFile.WriteLine(parameters[0].ToString());
                        packetFile.Flush();
                        packetFile.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }
    }


    public enum LogType : long
    {
        Command = 0x1000000000000000,
        Normal = 0x0100000000000000,
        Success = 0x0010000000000000,
        Error = 0x0001000000000000,
        Debug = 0x0000100000000000,
        Test = 0x0000010000000000,
        Chat = 0x0000001000000000,
        Terrain = 0x0000000100000000,
        Network = 0x0000000010000000,
        Packet = 0x0000000001000000,
    }

}
