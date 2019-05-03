using System;
using System.IO;

namespace QuestManager.QuestFiles
{
    public enum FormatVersion : byte
    {
        One = (0xAE ^ 0xEF),
    }
    public struct File
    {
        public byte[] Version
        {
            get;
            private set;
        } //2byte
        public byte[] Checksym
        {
            get;
            private set;
        } //32byte
        public string Name
        {
            get;
            private set;
        }//320byte       
        public string Quest
        {
            get;
            private set;
        }

        public File(string Name, string Quest)
        {
            Version = new byte[2] { 0xAE, 0xEF };
            this.Name = Name;
            this.Quest = Quest;
            Checksym = Crc32.CRC32(Name + Quest);
        }
        public File(string Name, string Quest, FormatVersion version, byte[] checksym)
        {
            Version = GetVersion(version);
            this.Name = Name;
            this.Quest = Quest;
            Checksym = checksym;

            if (!Crc32.IsValidCRC(Crc32.CRC32(Name + Quest), checksym))
                throw new Exception("CRC32 не верный");
        }

        public static FormatVersion GetVersion(byte[] Version)
        {
            if (Version.Length != 2)
                throw new Exception("Неверный размер");
            if(!Enum.IsDefined(typeof(FormatVersion), (byte)(Version[0] ^ Version[1])))
                throw new Exception("Некорректная версия");
            return (FormatVersion)(Version[0] ^ Version[1]);
        }
        public static byte[] GetVersion(FormatVersion Version)
        {
            if (Version == FormatVersion.One)
                return new byte[] {0xAE, 0xEF };
            else
                return new byte[] { 0x00, 0x00 };
        }

        public void Save(string path)
        {
            // создаем объект BinaryWriter
            using (BinaryWriter writer = new BinaryWriter(System.IO.File.Open(path, FileMode.OpenOrCreate)))
            {
                writer.Write(Version);
                writer.Write(Name);
                writer.Write(Quest);
                writer.Write(Checksym);
            }
        }
        public static File Open(string path)
        {
            using (BinaryReader reader = new BinaryReader(System.IO.File.Open(path, FileMode.Open)))
            {
                // пока не достигнут конец файла
                // считываем каждое значение из файла
                while (reader.PeekChar() > -1)
                {
                    byte[] version = reader.ReadBytes(2);
                    string name = reader.ReadString();
                    string quest = reader.ReadString();
                    byte[] checksym = reader.ReadBytes((int)Crc32.DefaultSize);

                    return new File(name, quest, GetVersion(version), checksym);
                }
            }
            return new File();
        }
    }
}
