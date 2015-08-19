using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace ViberToPDF_Converter
{
    class Implementation
    {
        private List<List<string>> data;
        public string FilePath { get; set; }

        public Implementation()
        {
            data = new List<List<string>>();
            FilePath = null;
        }

        public void ReadFile()
        {
            if (FilePath == null) return;

            var parser = new TextFieldParser(FilePath)
            {
                TextFieldType = FieldType.Delimited,
                CommentTokens = new string[] {"#"}
            };

            parser.SetDelimiters(new string[] { "," });
            parser.HasFieldsEnclosedInQuotes = false;

            parser.ReadLine();

            while (!parser.EndOfData)
            {
                var row = parser.ReadFields();
                if (row == null) continue;
                var newLine = new List<string>(row.Length);

                foreach (string t in row)
                {
                    newLine.Add(t);
                }

                data.Add(newLine);
            }

        }  


        public void PrintData()
        {
            var printDialog = new PrintDialog();
            var doc = new PrintDocument();

            printDialog.Document = doc;
            doc.PrintPage += new PrintPageEventHandler(doc_PrintPage);

            var result = printDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                doc.Print();
            }
        }

        private void doc_PrintPage(object sender, PrintPageEventArgs e)
        {
            var graphics = e.Graphics;

            var font = new Font("Times New Roman", 12);
            var brush = new SolidBrush(Color.Black);

            var height = font.GetHeight();

            const int startX = 55;
            const int startY = 60;
            var offset = 40;

            int count = 0;
            float linesPerPage = e.MarginBounds.Height / font.GetHeight(e.Graphics);

            
            while (0 != data.Count)
            {
                var toPrint = data[0];
                string message = "";
                foreach (string line in toPrint)
                {
                    message += line + " ";
                }

                if (message.Length < 95)
                {
                    graphics.DrawString(message, font, brush, startX, startY + offset);
                    count += 1;
                }
                else if (message.Length < 190)
                {
                    graphics.DrawString(message.Substring(0, 90), font, brush, startX, startY + offset);
                    offset = offset + (int) height + 5;
                    graphics.DrawString(message.Substring(90), font, brush, startX, startY + offset);
                    count += 2;
                }
                else if (message.Length < 285)
                {
                    graphics.DrawString(message.Substring(0, 90), font, brush, startX, startY + offset);
                    offset = offset + (int) height + 5;
                    graphics.DrawString(message.Substring(90, 90), font, brush, startX, startY + offset);
                    offset = offset + (int) height + 5;
                    graphics.DrawString(message.Substring(190), font, brush, startX, startY + offset);
                    count += 3;
                }
                else
                {
                    graphics.DrawString(message.Substring(0, 90), font, brush, startX, startY + offset);
                    offset = offset + (int)height + 5;
                    graphics.DrawString(message.Substring(90, 90), font, brush, startX, startY + offset);
                    offset = offset + (int)height + 5;
                    graphics.DrawString(message.Substring(190, 90), font, brush, startX, startY + offset);
                    offset = offset + (int)height + 5;
                    graphics.DrawString(message.Substring(285), font, brush, startX, startY + offset);
                    count += 4;                    
                }

                offset = offset + (int) height + 5;
                
                data.RemoveAt(0);

                if (count > 40)
                    break;
            }

            if (data.Count != 0)
                e.HasMorePages = true;
            else
                e.HasMorePages = false;


        }
    }
}