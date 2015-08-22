using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace ViberToPDF_Converter
{
    class Implementation
    {
        private readonly List<List<string>> _data;
        public string FilePath { get; set; }

        public Implementation()
        {
            _data = new List<List<string>>();
            FilePath = null;
        }

        public void ReadFile()
        {
            if (FilePath == null) return;

            var parser = new TextFieldParser(FilePath)
            {
                TextFieldType = FieldType.Delimited,
                CommentTokens = new[] {"#"}
            };

            parser.SetDelimiters(",");
            parser.HasFieldsEnclosedInQuotes = false;

            parser.ReadLine();

            while (!parser.EndOfData)
            {
                var row = parser.ReadFields();
                if (row == null) continue;
                var newLine = new List<string>(row.Length);
                newLine.AddRange(row);

                _data.Add(newLine);
            }

        }  

        public void PrintData()
        {
            var printDialog = new PrintDialog();
            var doc = new PrintDocument();

            printDialog.Document = doc;
            doc.PrintPage += doc_PrintPage;

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
            var count = 0;
            var readIndex = 0;
            var newIndex = 0;

            while (0 != _data.Count)
            {
                var toPrint = _data[0];
                var message = toPrint.Aggregate("", (current, line) => current + (line + " "));

                if (message.Length < 90)
                {
                    graphics.DrawString(message, font, brush, startX, startY + offset);
                    offset = offset + (int)height + 5;
                    count += 1;
                }
                else if (message.Length < 180)
                {
                    readIndex = message.IndexOf(" ", 86, StringComparison.Ordinal);
                    graphics.DrawString(message.Substring(0, readIndex), font, brush, startX, startY + offset);
                    offset = offset + (int) height + 5;

                    graphics.DrawString(message.Substring(readIndex), font, brush, startX, startY + offset);
                    offset = offset + (int) height + 5;
                    count += 2;
                }
                else if (message.Length < 270)
                {
                    readIndex = message.IndexOf(" ", 86, StringComparison.Ordinal);

                    if (readIndex > 90)
                    {
                        readIndex = message.LastIndexOf(" ", 90, StringComparison.Ordinal);
                    }

                    graphics.DrawString(message.Substring(0, readIndex), font, brush, startX, startY + offset);
                    offset = offset + (int) height + 5;

                    newIndex = (message.IndexOf(" ", readIndex + 80, StringComparison.Ordinal) - 90);

                    if (newIndex > 180)
                    {
                        newIndex = message.LastIndexOf(" ", readIndex + 90, StringComparison.Ordinal);
                    }

                    graphics.DrawString(message.Substring(readIndex, newIndex), font, brush, startX, startY + offset);
                    offset = offset + (int) height + 5;

                    readIndex = newIndex + 90;
                    graphics.DrawString(message.Substring(readIndex), font, brush, startX, startY + offset);
                    offset = offset + (int)height + 5;
                    count += 3;
                }
                else
                {
                    readIndex = message.IndexOf(" ", 86, StringComparison.Ordinal);

                    if (readIndex > 90)
                    {
                        readIndex = message.LastIndexOf(" ", 90, StringComparison.Ordinal);
                    }

                    graphics.DrawString(message.Substring(0, readIndex), font, brush, startX, startY + offset);
                    offset = offset + (int) height + 5;

                    newIndex = (message.IndexOf(" ", readIndex + 80, StringComparison.Ordinal) - 90);

                    if (newIndex > 180)
                    {
                        newIndex = message.LastIndexOf(" ", readIndex + 90, StringComparison.Ordinal);
                    }

                    graphics.DrawString(message.Substring(readIndex, newIndex), font, brush, startX, startY + offset);
                    offset = offset + (int) height + 5;

                    readIndex = newIndex + 90;
                    newIndex = (message.IndexOf(" ", 266, StringComparison.Ordinal) - 180);

                    if (newIndex > 270)
                    {
                        newIndex = message.LastIndexOf(" ", readIndex + 90, StringComparison.Ordinal);
                    }

                    graphics.DrawString(message.Substring(readIndex, newIndex), font, brush, startX, startY + offset);
                    offset = offset + (int)height + 5;

                    readIndex = newIndex + 180;
                    graphics.DrawString(message.Substring(readIndex), font, brush, startX, startY + offset);
                    offset = offset + (int)height + 5;
                    count += 4;                
                }
                
                _data.RemoveAt(0);

                if (count > 40)
                    break;
            }

            e.HasMorePages = _data.Count != 0; // if the amount of data != zero, more pages will be produced
        }
    }
}