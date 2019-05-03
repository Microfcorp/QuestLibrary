using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestManager.QuestFiles;

namespace QuestManager.QuestFiles.Quest
{
    public struct Quest
    {
        public string[] ParentChild
        {
            get;
            private set;
        }
        public string ParentText
        {
            get;
            private set;
        }
        public string Text
        {
            get;
            private set;
        }
        public string Name
        {
            get;
            private set;
        }

        public Quest(string Name, string Text, string[] ParentChild, string ParentText)
        {
            this.Name = Name;
            this.Text = Text;
            this.ParentChild = ParentChild;
            this.ParentText = ParentText;
        }
    }   
    public class QuestManager
    {
        public List<Quest> Quests
        {
            get;
            private set;
        }
        public string Name
        {
            get;
            private set;
        }

        public Quest AddQuest(string Name, string Text, string[] ParentChild, string ParentText)
        {
            //Console.WriteLine(ContainsQuest(Name));
            if (!ContainsQuest(Name))
            {
                var tmp = new Quest(Name, Text, ParentChild, ParentText);
                Quests.Add(tmp);
                return tmp;
            }
            else
            {
                var EQ = new Quest(Name, Text, ParentChild, ParentText);
                EditQuest(FromGetName(Name), EQ);
                return EQ;
            }
        }
        public Quest AddQuest(Quest quest)
        {
            Quests.Add(quest);
            return quest;
        }
        public Quest RemoveQuest(Quest quest)
        {
            Quests.Remove(quest);
            return quest;
        }
        public Quest EditQuest(Quest quest, Quest newquest)
        {
            Quests[GetIdQuest(quest)] = newquest;
            return newquest;
        }
        public Quest Select(Quest quest)
        {
            foreach (var item in Quests)
                if (quest.Name == item.Name)
                    return item;
            return new Quest();
        }
        public bool ContainsQuest(string Name)
        {
            if (FromGetName(Name).Name != null)
                return true;
            else
                return false;
        }
        public Quest FromGetName(string Name)
        {
            foreach (var item in Quests)
                if (item.Name == Name)
                    return item;
            return new Quest();
        }
        public Quest[] GetParentChild(Quest node)
        {
            List<Quest> tmp = new List<Quest>();
            foreach (var item in node.ParentChild)
            {
                tmp.Add(FromGetName(item));
            }
            return tmp.ToArray();  
        }
        public Quest[] GetPostChild(Quest node)
        {
            List<Quest> tmp = new List<Quest>();
            foreach (var item in Quests)
            {
                foreach (var item1 in item.ParentChild)
                {
                    if (node.Name == item1)
                        tmp.Add(item);
                }
            }
            return tmp.ToArray();
        }
        public int GetIdQuest(Quest node)
        {
            if (!Quests.Contains(node))
                return int.MaxValue;
            for (int i = 0; i < Quests.Count; i++)
                if(Quests[i].Name == node.Name)
                    return i;
            return int.MaxValue; ;
        }

        private QuestManager()
        {
            Quests = new List<Quest>();
        }
        public static QuestManager Create(string Name)
        {
            var tmp = new QuestManager();
            tmp.Name = Name;
            return tmp;
        }
        public static QuestManager FromFile(File file)
        {
            var tmp = new QuestManager();
            tmp.Name = file.Name;

            var QuestBin = file.Quest.Split('\r');

            foreach (var item in QuestBin)
            {
                var BIN = new BINFormat(item);
                Quest tm = new Quest(BIN.Name, BIN.Text, BIN.ParentChild, BIN.ParentText);                
                tmp.Quests.Add(tm);
            }

            return tmp;
        }
        public File ToFile()
        {            
            var tmp = new File(Name, BINFormat.Pack(Quests.ToArray()));
            return tmp;
        }
    }
}
