using ClosedXML.Excel;
using Microsoft.JSInterop;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

    public async Task GeneratePdfSharpCore<T>(IEnumerable<T> data, string title)
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

    public async Task GeneratePdf<T>(IEnumerable<T> data, string title)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        if (data == null || !data.Any())
            throw new ArgumentException("No hay datos para generar el PDF.");

        var properties = typeof(T)
            .GetProperties()
            .Where(p => !p.Name.StartsWith("Id_", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4.Landscape());
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text(title)
                    .SemiBold().FontSize(18).AlignCenter();

                page.Content()
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            foreach (var _ in properties)
                                columns.RelativeColumn();
                        });

                        // Encabezados
                        table.Header(header =>
                        {
                            foreach (var prop in properties)
                            {

                                header.Cell()
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(5)
                                    .Text(prop.Name)
                                    .SemiBold();
                            }
                        });

                        // Filas
                        foreach (var item in data)
                        {
                            foreach (var prop in properties)
                            {
                                var rawValue = prop.GetValue(item)?.ToString() ?? "";
                                string value = rawValue?.ToString() ?? string.Empty;

                                if (prop.Name.Equals("Estado", StringComparison.OrdinalIgnoreCase))
                                {
                                    value = value switch
                                    {
                                        "A" => "Activo",
                                        "I" => "Inactivo",
                                        _ => value
                                    };
                                }

                                table.Cell()
                                    .BorderBottom(1)
                                    .Padding(4)
                                    .Text(value);
                            }
                        }
                    });

                page.Footer()
                    .AlignRight()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
            });
        });

        var stream = new MemoryStream();
        document.GeneratePdf(stream);
        stream.Position = 0;

        await _jsInterop.InvokeVoidAsync("saveAsFile", $"{title}.pdf", Convert.ToBase64String(stream.ToArray()));
    }

}
