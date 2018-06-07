using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Text;
using System.IO;
using System.Threading;
using EasyHook;

namespace FileMon{
    public class FileMonInterface : MarshalByRefObject
    {
        public void IsInstalled(Int32 InClientPID)
        {
            Console.WriteLine("FileMon has been installed in target {0}.\r\n", InClientPID);
        }

        public void OnCreateFile(Int32 InClientPID, String[] InFileNames)
        {
            for (int i = 0; i < InFileNames.Length; i++)
            {
                Console.WriteLine(InFileNames[i]);
            }
        }

        public void ReportException(Exception InInfo)
        {
            Console.WriteLine("The target process has reported an error:\r\n" + InInfo.ToString());
        }

        public void Ping()
        {
        }

        public static Dictionary<string, Dictionary<string, string>> dict = new Dictionary<string, Dictionary<string, string>>()
        {
                {
                    @"C:\Windows\system32\calc.exe", new Dictionary<string, string>(){
                        {
                            "计算器","Calculator"
                        },


                        {
                            "查看(V)", "View"
                        },

                        {
                            "标准型(T)","Standard"
                        },
                        {
                            "科学型(S)","Scientific"
                        },
                        {
                            "程序员(P)","Programmer"
                        },
                        {
                            "统计信息(A)","Statistical"
                        },


                        {
                            "编辑(E)", "Edit"
                        },
                        {
                            "复制(C)","Copy"
                        },
                        {
                            "粘贴(P)","Paste"
                        },
                        

                        {
                            "帮助(H)", "Help"
                        },
                        {
                            "查看帮助(V)","View"
                        },
                        {
                            "关于计算器(A)","About"
                        },

                        {
                            "十六进制","Hexadecimal"
                        },
                        {
                            "十进制","Decimal"
                        },
                        {
                            "八进制","Octal"
                        },
                        {
                            "二进制","Binary"
                        },
                        
                        
                        {
                            "四字","Four bytes"
                        },
                        {
                            "双字","Double byte"
                        },
                        {
                            "字","Word"
                        },
                        {
                            "字节","Byte"
                        },
                    }
                }
            };
    }
    class Program{
        static String ChannelName = null;
        static void Main(string[] args){
//#if DEBUG
//#else
            var configFile = "Localization.json";
            if (!File.Exists(configFile))
                File.WriteAllText(configFile, Newtonsoft.Json.JsonConvert.SerializeObject(FileMonInterface.dict, Newtonsoft.Json.Formatting.Indented));
            FileMonInterface.dict =Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(configFile));
//#endif
            var targetExe = "notepad.exe";
            foreach (var key in FileMonInterface.dict.Keys){
                targetExe = key;
                break;
            }
            Int32 TargetPID = 0;
            Process p=null;
            foreach (var process in Process.GetProcesses()){
                if ("NOTEPAD" == process.ProcessName.ToUpper()) {
                    p = process;
                    TargetPID = process.Id;
                    break;
                }
            }
            if(0==TargetPID){
                p=Process.Start(targetExe);
                TargetPID = p.Id;
            }
            try{
                Config.Register(
                    "A FileMon like demo application.",
                    "FileMon.exe",
                    "FileMonInject.dll");
                RemoteHooking.IpcCreateServer<FileMonInterface>(ref ChannelName, WellKnownObjectMode.SingleCall);
                RemoteHooking.Inject(
                    TargetPID,
                    "FileMonInject.dll",
                    "FileMonInject.dll",
                    ChannelName);
                while (true) {
                    Thread.Sleep(1);
                    if(null!=p)
                        if (p.HasExited)
                            break;
                }
                Console.Write("done");
                Thread.Sleep(100);
                //Console.ReadLine();
            }
            catch (Exception ExtInfo)
            {
                Console.WriteLine("There was an error while connecting to target:\r\n{0}", ExtInfo.ToString());
                Console.ReadLine();
            }
        }
    }
}
