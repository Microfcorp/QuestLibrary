using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuestManager.QuestFiles;

namespace QuestManager.QuestFiles.Quest
{
    public struct Quest
    {
        /// <summary>
        /// Родительские элементы
        /// </summary>
        public string[] ParentChild
        {
            get;
            private set;
        }
        /// <summary>
        /// Текст в родительском элементе
        /// </summary>
        public string ParentText
        {
            get;
            private set;
        }
        /// <summary>
        /// Текст результата выбора
        /// </summary>
        public string Text
        {
            get;
            private set;
        }
        /// <summary>
        /// Имя
        /// </summary>
        public string Name
        {
            get;
            private set;
        }
        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Text">Текст результата выбора</param>
        /// <param name="ParentChild">Родительские элементы</param>
        /// <param name="ParentText">Текст в родительском элементе</param>
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
        /// <summary>
        /// Список всех элементов
        /// </summary>
        public List<Quest> Quests
        {
            get;
            private set;
        }
        /// <summary>
        /// Имя файла
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Добавить квест
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Text">Текст результата выбора</param>
        /// <param name="ParentChild">Родительские элементы</param>
        /// <param name="ParentText">Текст в родительском элементе</param>
        /// <returns></returns>
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
        /// <summary>
        /// Добавить квест
        /// </summary>
        /// <param name="quest">Квест</param>
        /// <returns></returns>
        public Quest AddQuest(Quest quest)
        {
            Quests.Add(quest);
            return quest;
        }
        /// <summary>
        /// Удалить квест
        /// </summary>
        /// <param name="quest">Квест</param>
        /// <returns></returns>
        public Quest RemoveQuest(Quest quest)
        {
            Quests.Remove(quest);
            return quest;
        }
        /// <summary>
        /// Изменить квест
        /// </summary>
        /// <param name="quest">Квест</param>
        /// <param name="newquest">Квест на который изменяем</param>
        /// <returns></returns>
        public Quest EditQuest(Quest quest, Quest newquest)
        {
            Quests[GetIdQuest(quest)] = newquest;
            return newquest;
        }
        /// <summary>
        /// Выбор квеста из списка
        /// </summary>
        /// <param name="quest">Квест</param>
        /// <returns></returns>
        public Quest Select(Quest quest)
        {
            foreach (var item in Quests)
                if (quest.Name == item.Name)
                    return item;
            return new Quest();
        }
        /// <summary>
        /// Имеется ли квест с таким именем
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <returns></returns>
        public bool ContainsQuest(string Name)
        {
            if (FromGetName(Name).Name != null)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Возвращает квест по имени
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <returns></returns>
        public Quest FromGetName(string Name)
        {
            foreach (var item in Quests)
                if (item.Name == Name)
                    return item;
            return new Quest();
        }
        /// <summary>
        /// Получить все родительские квесте
        /// </summary>
        /// <param name="node">Квест</param>
        /// <returns></returns>
        public Quest[] GetParentChild(Quest node)
        {
            List<Quest> tmp = new List<Quest>();
            foreach (var item in node.ParentChild)
            {
                tmp.Add(FromGetName(item));
            }
            return tmp.ToArray();  
        }
        /// <summary>
        /// Получить все дочерние квесты
        /// </summary>
        /// <param name="node">Квест</param>
        /// <returns></returns>
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
        /// <summary>
        /// Возвращает ID квеста в списке квестов
        /// </summary>
        /// <param name="node">Квест</param>
        /// <returns></returns>
        public int GetIdQuest(Quest node)
        {
            if (!Quests.Contains(node))
                return int.MaxValue;
            for (int i = 0; i < Quests.Count; i++)
                if(Quests[i].Name == node.Name)
                    return i;
            return int.MaxValue; ;
        }

        /// <summary>
        /// Инициализация
        /// </summary>
        private QuestManager()
        {
            Quests = new List<Quest>();
        }

        /// <summary>
        /// Создать новую систему
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <returns></returns>
        public static QuestManager Create(string Name)
        {
            var tmp = new QuestManager();
            tmp.Name = Name;
            return tmp;
        }

        /// <summary>
        /// Загрузить систему из файла
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns></returns>
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
        /// <summary>
        /// Преобразовать систему к файлу
        /// </summary>
        /// <returns></returns>
        public File ToFile()
        {            
            var tmp = new File(Name, BINFormat.Pack(Quests.ToArray()));
            return tmp;
        }
    }
}
