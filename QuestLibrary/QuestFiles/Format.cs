using System;
using System.IO;

namespace QuestManager.QuestFiles
{
    public enum FormatVersion : byte
    {
        /// <summary>
        /// Первая версия
        /// </summary>
        One = (0xAE ^ 0xEF),
        /// <summary>
        /// Вторая версия
        /// </summary>
        Two = (0xAA ^ 0xBF),
    }
    struct FileVersion
    {
        /// <summary>
        /// Первая версия
        /// </summary>
        public static byte[] One
        {
            get
            {
                return new byte[2] { 0xAE, 0xEF };
            }
        }
        /// <summary>
        /// Вторая версия
        /// </summary>
        public static byte[] Two
        {
            get
            {
                return new byte[2] { 0xAA, 0xBF };
            }
        }
    }
    public struct File
    {
        /// <summary>
        /// Версия (2 байта)
        /// </summary>
        public byte[] Version
        {
            get;
            private set;
        } //2byte

        /// <summary>
        /// Контрольная сумма
        /// </summary>
        public byte[] Checksym
        {
            get;
            private set;
        } //32byte

        /// <summary>
        /// Имя
        /// </summary>
        public string Name
        {
            get;
            private set;
        }       

        /// <summary>
        /// Данные квестов
        /// </summary>
        public string Quest
        {
            get;
            private set;
        }

        /// <summary>
        /// Автор
        /// </summary>
        public string Author
        {
            get;
            private set;
        }

        /// <summary>
        /// Только для чтения
        /// </summary>
        public bool ReadOnly
        {
            get;
            private set;
        }

        /// <summary>
        /// Только для чтения (Байт формат)
        /// </summary>
        public byte ReadOnlyByte
        {
            get
            {
                if (ReadOnly)
                    return 0xFF;
                else
                    return 0x00;
            }
            set
            {
                if (value == 0xFF)
                    ReadOnly = true;
                else
                    ReadOnly = false;
            }
        }

        /// <summary>
        /// Цифровая подпись
        /// </summary>
        public byte[] DigitalSign
        {
            get;
            private set;
        }

        //First Version
        /// <summary>
        /// Объявление структуры
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Quest">Квест</param>
        public File(string Name, string Quest)
        {
            Version = FileVersion.One;
            this.Name = Name;
            this.Quest = Quest;
            this.ReadOnly = false;
            this.DigitalSign = null;
            this.Author = Environment.UserName;
            Checksym = Crc32.CRC32(Name + Quest);
        }

        /// <summary>
        /// Объявление структуры
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Quest">Квест</param>
        /// <param name="version">Версия</param>
        /// <param name="checksym">Контрольная сумма</param>
        public File(string Name, string Quest, FormatVersion version, byte[] checksym)
        {
            Version = GetVersion(version);
            this.Name = Name;
            this.Quest = Quest;
            this.ReadOnly = false;
            this.DigitalSign = null;
            this.Author = Environment.UserName;
            Checksym = checksym;

            if (!Crc32.IsValidCRC(Crc32.CRC32(Name + Quest), checksym))
                throw new Exception("CRC32 не верный");
        }

        //Second Version
        /// <summary>
        /// Объявление структуры
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Quest">Квест</param>
        /// <param name="ReadOnly">Только для чтения</param>
        public File(string Name, string Quest, byte ReadOnly)
        {
            Version = FileVersion.Two;
            this.Name = Name;
            this.Quest = Quest;
            this.ReadOnly = ByteToBool(ReadOnly);
            this.DigitalSign = null;
            this.Author = Environment.UserName;
            Checksym = Crc32.CRC32(Name + Quest);
        }

        /// <summary>
        /// Объявление структуры
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="Quest">Квест</param>
        /// <param name="version">Версия</param>
        /// <param name="checksym">Контрольная сумма</param>
        /// <param name="ReadOnly">Только для чтения</param>
        /// <param name="DigitalSign">Цифровая подпись</param>
        public File(string Name, string Quest, FormatVersion version, byte[] checksym, byte ReadOnly, byte[] DigitalSign, string Author)
        {
            Version = GetVersion(version);
            this.Name = Name;
            this.Quest = Quest;            
            this.DigitalSign = DigitalSign;
            Checksym = checksym;
            this.ReadOnly = ByteToBool(ReadOnly);
            this.Author = Author;

            if (!QuestLibrary.QuestFiles.Sign.Sign.CorrectSign(DigitalSign))
                throw new Exception("Цифровая подпись неверная");

            if (!Crc32.IsValidCRC(Crc32.CRC32(Name + Quest), checksym))
                throw new Exception("CRC32 не верный");
        }

        /// <summary>
        /// Версия файла из массива байтов
        /// </summary>
        /// <param name="Version">Массив байтов (2 байта)</param>
        /// <returns></returns>
        public static FormatVersion GetVersion(byte[] Version)
        {
            if (Version.Length != 2)
                throw new Exception("Неверный размер");
            if(!Enum.IsDefined(typeof(FormatVersion), (byte)(Version[0] ^ Version[1])))
                throw new Exception("Некорректная версия");
            return (FormatVersion)(Version[0] ^ Version[1]);
        }

        /// <summary>
        /// Версия файла из перечисления
        /// </summary>
        /// <param name="Version">Перечисление</param>
        /// <returns></returns>
        public static byte[] GetVersion(FormatVersion Version)
        {
            if (Version == FormatVersion.One)
                return FileVersion.One;
            else if (Version == FormatVersion.Two)
                return FileVersion.Two;
            else
                return new byte[] { 0x00, 0x00 };
        }

        /// <summary>
        /// Байт в логическое значение
        /// </summary>
        /// <param name="b">Байт (0xFF или 0x00)</param>
        /// <returns></returns>
        public static bool ByteToBool(byte b)
        {
            return ((b==0xFF)?true:false);
        }

        /// <summary>
        /// Логическое значение в байт
        /// </summary>
        /// <param name="b">Логическое значение в байт (0xFF или 0x00)</param>
        /// <returns></returns>
        public static byte BoolToByte(bool b)
        {
            return ((b) ? (byte)0xFF : (byte)0x00);
        }

        /// <summary>
        /// Сохранить в файл
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        public void Save(string path)
        {
            // создаем объект BinaryWriter
            using (BinaryWriter writer = new BinaryWriter(System.IO.File.Open(path, FileMode.OpenOrCreate)))
            {
                writer.Write(Version);
                writer.Write(Name);
                writer.Write(Quest);
                writer.Write(Checksym);

                if (Version[0] == FileVersion.Two[0] & Version[1] == FileVersion.Two[1])
                {
                    writer.Write(Author);
                    writer.Write(ReadOnlyByte);
                    writer.Write(DigitalSign);
                }
            }
        }
        /// <summary>
        /// Чтение из файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
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

                    if(version[0] == FileVersion.Two[0] & version[1] == FileVersion.Two[1])
                    {
                        string Author = reader.ReadString();
                        byte ReadOnly = reader.ReadByte();
                        var Sign = reader.ReadBytes(4);
                        return new File(name, quest, GetVersion(version), checksym, ReadOnly, Sign, Author);
                    }
                    else
                        return new File(name, quest, GetVersion(version), checksym);
                }
            }
            return new File();
        }
    }
}
