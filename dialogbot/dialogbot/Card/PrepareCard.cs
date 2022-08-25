using System.IO;

namespace dialogbot.Card
{
   
        public static class PrepareCard
        {
            public static string ReadCard(string fileName)
            {
                string[] BuildPath = { ".", "Card", fileName };
                var filePath = Path.Combine(BuildPath);
                var fileRead = File.ReadAllText(filePath);
                return fileRead;
            }
        }
}
