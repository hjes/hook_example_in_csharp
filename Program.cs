using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Text;
using System.IO;
using System.Threading;
using EasyHook;



namespace FileMon
{
    public class FileMonInterface : MarshalByRefObject
    {
        //public T DeserializeObject<T>(string json)
        //{
        //    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        //}






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
                    @"C:\Windows\system32\calc.exe", new Dictionary<string, string>()
                    {
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

    class Program
    {
        static String ChannelName = null;

        static void Main(string[] args)
        {
//#if DEBUG
//#else
            var configFile = "Localization.json";
            if (!File.Exists(configFile))
                File.WriteAllText(configFile, Newtonsoft.Json.JsonConvert.SerializeObject(FileMonInterface.dict, Newtonsoft.Json.Formatting.Indented));
            FileMonInterface.dict =Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(configFile));
//#endif




            var targetExe = @"C:\Program Files (x86)\Altova\XMLSpy2011\XMLSpy.exe";
            targetExe = "notepad.exe";
            foreach (var key in FileMonInterface.dict.Keys)
            {
                targetExe = key;
                break;
            }


            Int32 TargetPID = 0;

            Process p=null;
            foreach (var process in Process.GetProcesses())
            {
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

            //if ((args.Length != 1) || !Int32.TryParse(args[0], out TargetPID)){
            //    Console.WriteLine();
            //    Console.WriteLine("Usage: FileMon %PID%");
            //    Console.WriteLine();
            //    return;
            //}

            try
            {
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







/*
    EasyHook - The reinvention of Windows API hooking
 
    Copyright (C) 2008 Christoph Husse

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

    Please visit http://www.codeplex.com/easyhook for more information
    about the project and latest updates.

PLEASE NOTE:
    The LGPL allows you to sell propritary software based on this library
    (EasyHook) without releasing the source code for your application.
    This is a big difference to the original GPL. Refer to the attached
    "LICENSE" document for more information about the LGPL!
 
    To wrap it up (without warranty):
        
        1)  You are granted to sell any software that uses EasyHook over
            DLL or NET bindings. This is covered by the native API and the 
            managed interface.
        2)  You are NOT granted to sell any software that includes parts
            of the EasyHook source code or any modification! If you want
            to modify EasyHook, you are forced to release your work under
            the LGPL or GPL... Of course this only applies to the library
            itself. For example you could release a modification of EasyHook
            under LGPL, while still being able to release software, which
            takes advantage of this modification over DLL or NET bindings,
            under a proprietary license!
        3)  You shall include a visible hint in your software that EasyHook
            is used as module and also point out, that this module in
            particular is released under the terms of the LGPL and NOT
            under the terms of your software (assuming that your software
            has another license than LGPL or GPL).
 
    I decided to release EasyHook under LGPL to prevent commercial abuse
    of this free work. I didn't release it under GPL, because I also want to
    address commercial vendors which are more common under Windows.

BUG REPORTS:

    Reporting bugs is the only chance to get them fixed! Don't consider your
    report useless... I will fix any serious bug within a short time! Bugs with
    lower priority will always be fixed in the next release...

DONATIONS:

    I want to add support for Itanium (II - III) processors. If you have any hardware
    that you don't need anymore or could donate, which >supports< a recent Windows
    Itanium edition (Windows license is not required), please contact me. Of course we 
    could discuss a reasonable sponsorship reference for your company. Money for
    buying such hardware is also appreciated...
*/