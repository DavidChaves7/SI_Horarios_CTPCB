using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using ClosedXML.Excel;
using System.Text;
using Microsoft.JSInterop;

public class DataGridUtil
{
    private readonly IJSRuntime _jsInterop;

    public DataGridUtil(IJSRuntime jsInterop)
    {
        _jsInterop = jsInterop;
    }

    public async Task GenerateCsv<T>(IEnumerable<T> data)
    {

        var properties = typeof(T).GetProperties();
        var csvBuilder = new StringBuilder();

        csvBuilder.AppendLine(string.Join(" || ", properties.Select(p => p.Name)));

        foreach (var item in data)
        {
            var values = properties.Select(p => p.GetValue(item, null)?.ToString() ?? string.Empty);
            csvBuilder.AppendLine(string.Join(" || ", values));
        }
        
        await _jsInterop.InvokeVoidAsync("navigator.clipboard.writeText", csvBuilder.ToString());

    }

    public async Task GenerateExcel<T>(IEnumerable<T> data,string title)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(title);
        using var stream = new MemoryStream();

        var properties = typeof(T).GetProperties();
        int row = 1;

        for (int col = 0; col < properties.Length; col++)
        {
            worksheet.Cell(row, col + 1).Value = properties[col].Name;
            worksheet.Cell(row, col + 1).Style.Font.Bold = true;
            worksheet.Cell(row, col + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        foreach (var item in data)
        {
            row++;
            for (int col = 0; col < properties.Length; col++)
            {
                var value = properties[col].GetValue(item, null)?.ToString() ?? string.Empty;
                worksheet.Cell(row, col + 1).Value = value;
            }
        }

        worksheet.Columns().AdjustToContents();
        workbook.SaveAs(stream);
        await _jsInterop.InvokeVoidAsync("saveAsFile", $"{title}.xlsx", Convert.ToBase64String(stream.ToArray()));
    }

    public async Task GeneratePdf<T>(IEnumerable<T> data, string title)
    {
        if (data == null || !data.Any())
            throw new ArgumentException("No data available to generate PDF.");

        using var document = new PdfDocument();
        var page = document.AddPage();
        page.Orientation = PdfSharpCore.PageOrientation.Landscape;
        var gfx = XGraphics.FromPdfPage(page);
        var font = new XFont("Arial", 12, XFontStyle.Regular);
        var titleFont = new XFont("Arial", 16, XFontStyle.Bold);

        // Determinar columnas a partir de las propiedades de T
        var properties = typeof(T).GetProperties();
        var columnNames = properties.Select(p => p.Name).ToArray();

        // Título
        gfx.DrawString(title, titleFont, XBrushes.Black, new XRect(40, 20, page.Width - 40, 40), XStringFormats.TopLeft);

        int yPosition = 60;
        const int xPadding = 40;
        const int yPadding = 20;
        const int colWidth = 100;

        // Encabezados
        for (int col = 0; col < columnNames.Length; col++)
        {
            gfx.DrawString(columnNames[col], font, XBrushes.Black, new XPoint(xPadding + col * colWidth, yPosition));
        }

        yPosition += yPadding;

        // Filas de datos
        foreach (var item in data)
        {
            for (int col = 0; col < properties.Length; col++)
            {
                var value = properties[col].GetValue(item)?.ToString() ?? string.Empty;
                gfx.DrawString(value, font, XBrushes.Black, new XPoint(xPadding + col * colWidth, yPosition));
            }

            yPosition += yPadding;

            // Crear nueva página si se excede el espacio
            if (yPosition > page.Height - 40)
            {
                page = document.AddPage();
                page.Orientation = PdfSharpCore.PageOrientation.Landscape;
                gfx = XGraphics.FromPdfPage(page);
                yPosition = 40;
            }
        }

        using var stream = new MemoryStream();
        document.Save(stream, false);
        await _jsInterop.InvokeVoidAsync("saveAsFile", $"{title}.pdf", Convert.ToBase64String(stream.ToArray()));
    }

}
