using IOCH;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace ConversationMigration
{
    class Program
    {
        static Regex regFormat = new Regex(@"^([\S\s]+)(?=\[+)\[(([0-1]?[0-9]|[2][0-3]):([0-5][0-9]))\]:\s+$", RegexOptions.Singleline);

        //0 timespan 1 sender 2 message
        static string OCMessageFormat = @"<DIV>
<DIV style=""POSITION: relative; PADDING-BOTTOM: 0px; PADDING-LEFT: 3px; WIDTH: 100%; PADDING-RIGHT: 3px; FONT-FAMILY: MS Shell Dlg 2; CLEAR: both; FONT-SIZE: 10pt; PADDING-TOP: 0px"" id=Normalheader class=immessageheader xmlns:convItem=""http://schemas.microsoft.com/2008/10/sip/convItems"" xmlns=""http://schemas.microsoft.com/2008/10/sip/convItems"" xmlns:rtc=""urn:microsoft-rtc-xslt-functions"" xmlns:msxsl=""urn:schemas-microsoft-com:xslt"" xmlns:xs=""http://www.w3.org/2001/XMLSchema""><SPAN style=""WHITE-SPACE: nowrap; FLOAT: right; COLOR: #666666; FONT-SIZE: 8pt; PADDING-TOP: 2px"" id=imsendtimestamp>{0}</SPAN><SPAN style=""FLOAT: left; COLOR: #666666"" id=imsendname>{1}</SPAN><SPAN style=""CLEAR: both""></SPAN></DIV>
<DIV style=""POSITION: relative; PADDING-BOTTOM: 0px; PADDING-LEFT: 3px; WIDTH: 100%; PADDING-RIGHT: 3px; CLEAR: both; PADDING-TOP: 0px"" id=Normalcontent xmlns:convItem=""http://schemas.microsoft.com/2008/10/sip/convItems"" xmlns=""http://schemas.microsoft.com/2008/10/sip/convItems"" xmlns:rtc=""urn:microsoft-rtc-xslt-functions"" xmlns:msxsl=""urn:schemas-microsoft-com:xslt"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
<DIV style=""FLOAT: left; HEIGHT: 100%; MARGIN-LEFT: 5px"" id=imwidget>
<OBJECT tabIndex=-1 classid=clsid:b3913e54-389f-45ea-9a3c-56b74cd62307><PARAM NAME=""_cx"" VALUE=""159""><PARAM NAME=""_cy"" VALUE=""318""><PARAM NAME=""EmoticonID"" VALUE=""114""></OBJECT></DIV>
<DIV style=""MARGIN-LEFT: 12px"" id=imcontent><SPAN>
<DIV style=""FONT-FAMILY: MS Shell Dlg 2; DIRECTION: ltr; COLOR: #000000; FONT-SIZE: 9pt"">OCHMEASAGEHERE</DIV></SPAN></DIV></DIV></DIV>
<DIV>";

        static string OCMessageFormatSameSender = @"<DIV>
<DIV style=""POSITION: relative; PADDING-BOTTOM: 0px; PADDING-LEFT: 3px; WIDTH: 100%; PADDING-RIGHT: 3px; CLEAR: both; PADDING-TOP: 0px"" id=Normalcontent xmlns:convItem=""http://schemas.microsoft.com/2008/10/sip/convItems"" xmlns=""http://schemas.microsoft.com/2008/10/sip/convItems"" xmlns:rtc=""urn:microsoft-rtc-xslt-functions"" xmlns:msxsl=""urn:schemas-microsoft-com:xslt"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
<DIV style=""FLOAT: left; HEIGHT: 100%; MARGIN-LEFT: 5px"" id=imwidget>
<OBJECT tabIndex=-1 classid=clsid:b3913e54-389f-45ea-9a3c-56b74cd62307><PARAM NAME=""_cx"" VALUE=""159""><PARAM NAME=""_cy"" VALUE=""318""><PARAM NAME=""EmoticonID"" VALUE=""114""></OBJECT></DIV>
<DIV style=""MARGIN-LEFT: 12px"" id=imcontent><SPAN>
<DIV style=""FONT-FAMILY: MS Shell Dlg 2; DIRECTION: ltr; COLOR: #000000; FONT-SIZE: 9pt"">OCHMEASAGEHERE</DIV></SPAN></DIV></DIV></DIV>";


        static void Main(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                Console.WriteLine("Usage: ConversationImport XmlFilePath");
            }
            else
            {
                string xmlFile = args[0];

                if(!File.Exists(xmlFile))
                {
                    Console.WriteLine("{0} does not exist", xmlFile);
                    return;
                }

                try
                {
                    using (FileStream fs = new FileStream(xmlFile, FileMode.Open))
                    {

                        XmlSerializer serializer = new XmlSerializer(typeof(Conversations));
                        object o = serializer.Deserialize(fs);

                        Conversations conversations = o as Conversations;

                        if(conversations == null)
                        {
                            Console.WriteLine("Cannot retrive information from {0}", xmlFile);
                            return;
                        }

                        DateTime dtStart = DateTime.Now;

                        ImportToDatabase(conversations.Items);

                        Console.WriteLine("Done. Time elapses: {0}", DateTime.Now - dtStart);
                        Console.WriteLine("Enjoy OCH! Hahahah");
                    }
                }
                catch (Exception exp)
                {
                    Console.WriteLine("Illegal {0}", xmlFile);
                    Console.Error.WriteLine(exp);
                    return;
                }
            }
        }

        static string FormatMessage(string message, DateTime date)
        {
            StringBuilder sb = new StringBuilder();

            string[] lines = message.Split(new string[]{"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);

            bool needReplace = false;

            foreach (string line in lines)
            {
                MatchCollection mcs = regFormat.Matches(line);

                if (mcs != null && mcs.Count > 0)
                {
                    foreach (Match mc in mcs)
                    {
                        if (mc.Groups.Count > 4)
                        {
                            Console.WriteLine("Processing {0} - {1}", mc.Groups[1].Value, mc.Groups[2].Value);

                            DateTime newDate = new DateTime(
                                date.Year, 
                                date.Month, 
                                date.Day, 
                                int.Parse(mc.Groups[3].Value), 
                                int.Parse(mc.Groups[4].Value), 
                                0);
                            string msg = string.Format(OCMessageFormat, newDate.ToString("yyyy-MM-dd HH:mm"), mc.Groups[1].Value);

                            sb.AppendLine(msg);
                        }
                    }

                    needReplace = true;
                }
                else
                {
                    if (needReplace)
                    {
                        if (sb.ToString().IndexOf("OCHMEASAGEHERE") != -1)
                        {
                            sb.Replace("OCHMEASAGEHERE", line);
                        }

                        needReplace = false;
                    }
                    else
                    {
                        sb.AppendLine(OCMessageFormatSameSender.Replace("OCHMEASAGEHERE", line));
                    }                    
                }
            }

            return sb.ToString();
        }

        static void ImportToDatabase(Conversation[] list)
        {
            IMessageStore messageStore = new MessageStoreImpl.SQLite();
            if (list != null)
            {
                foreach (var l in list)
                {
                    string[] contracts = new string[l.Recipients.Length];
                    for (int i = 0; i < contracts.Length; i++)
                    {
                        contracts[i] = l.Recipients[i].Trim();
                    }

                    DateTime date = DateTime.Parse(l.Time.Trim());
                    string message = FormatMessage(l.Body.Trim(), date);

                    messageStore.SaveMessage(date, message, contracts);
                }
            }
        }
    }
}
