using System.Text;

namespace ReportsService.Application.Utilities;

internal static class PdfBuilder
{
    public static byte[] FromText(string text)
    {
        var contentBuilder = new StringBuilder();
        contentBuilder.AppendLine("BT");
        contentBuilder.AppendLine("/F1 12 Tf");
        contentBuilder.AppendLine("72 720 Td");
        contentBuilder.AppendLine("12 TL");

        var lines = text.Replace("\r", string.Empty).Split('\n');
        foreach (var line in lines)
        {
            var escapedLine = Escape(line);
            contentBuilder.AppendLine($"({escapedLine}) Tj");
            contentBuilder.AppendLine("T*");
        }

        contentBuilder.AppendLine("ET");

        var contentStream = contentBuilder.ToString();
        var contentBytes = Encoding.ASCII.GetBytes(contentStream);

        var objects = new List<string>
        {
            "1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj",
            "2 0 obj << /Type /Pages /Kids [3 0 R] /Count 1 >> endobj",
            "3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >> endobj",
            $"4 0 obj << /Length {contentBytes.Length} >> stream\n{contentStream}\nendstream endobj",
            "5 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> endobj"
        };

        using var memory = new MemoryStream();
        using var writer = new StreamWriter(memory, Encoding.ASCII, leaveOpen: true);

        writer.WriteLine("%PDF-1.4");
        writer.Flush();

        var offsets = new List<long>();
        foreach (var entry in objects)
        {
            offsets.Add(memory.Position);
            writer.WriteLine(entry);
            writer.Flush();
        }

        var xrefPosition = memory.Position;
        writer.WriteLine("xref");
        writer.WriteLine($"0 {objects.Count + 1}");
        writer.WriteLine("0000000000 65535 f ");

        foreach (var offset in offsets)
        {
            writer.WriteLine($"{offset:D10} 00000 n ");
        }

        writer.WriteLine("trailer");
        writer.WriteLine($"<< /Size {objects.Count + 1} /Root 1 0 R >>");
        writer.WriteLine("startxref");
        writer.WriteLine(xrefPosition.ToString());
        writer.WriteLine("%%EOF");
        writer.Flush();

        return memory.ToArray();
    }

    private static string Escape(string text)
    {
        return text
            .Replace("\\", "\\\\")
            .Replace("(", "\\(")
            .Replace(")", "\\)");
    }
}
