using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestManager.QuestFiles.Quest
{
    struct BINFormat
    {
        public string Name;
        public string ParentText;
        public string[] ParentChild;
        public string Text;

        public BINFormat(string[] args)
        {
            this.Name = args[0];
            this.Text = args[1];
            this.ParentChild = args[2].Split(',');
            this.ParentText = args[3];
        }

        public BINFormat(string arg)
        {
            string[] args = (arg.Split('|'));
            this.Name = args[0];
            this.Text = args[1];
            this.ParentChild = args[2].Split(',');
            this.ParentText = args[3];
        }
        public static string Pack(Quest[] q)
        {
            List<string> args = new List<string>();
            foreach (var item in q)
            {
                args.Add(item.Name+"|"+ item.Text + "|" + String.Join(",", item.ParentChild) + "|" + item.ParentText );
            }
            return String.Join("\r", args.ToArray());
        }
    }
}
