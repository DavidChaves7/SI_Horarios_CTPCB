using SI_Horarios_CTPCB.Infrastructure.ApiClient;
using SI_Horarios_CTPCB.Infrastructure.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Drawing;

namespace SI_Horarios_CTPCB.Infrastructure.Services
{
    public class ExcelService
    {
        public byte[] CreateExcel<T>(IEnumerable<T?> data, ExcelService columnQty, Color headerColor, string? sheetName = "Sheet1")
        {
            var stream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(stream))
            {
                //Crea el libro de trabajo
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                //Implementa la data cargandola desde la coleccion
                worksheet.Cells.LoadFromCollection(data, true);

                //Estilos de columnas de header

                worksheet.Cells[$"A1:{columnQty.Value}1"].Style.Font.Bold = true;
                worksheet.Cells[$"A1:{columnQty.Value}1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[$"A1:{columnQty.Value}1"].Style.Fill.BackgroundColor.SetColor(headerColor);
                worksheet.Cells[$"A1:{columnQty.Value}1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"A1:{columnQty.Value}1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"A1:{columnQty.Value}1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A1:{columnQty.Value}1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A1:{columnQty.Value}1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A1:{columnQty.Value}1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                //Setea el header en mayusculas para que no se vea a como devuelve la info el api
                worksheet.Cells[$"A1:{columnQty.Value}1"].Text.Trim().ToUpper();

                //Estilos de columnas de body

                worksheet.Cells[$"A2:{columnQty.Value}{data.Count() + 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"A2:{columnQty.Value}{data.Count() + 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"A2:{columnQty.Value}{data.Count() + 1}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A2:{columnQty.Value}{data.Count() + 1}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A2:{columnQty.Value}{data.Count() + 1}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A2:{columnQty.Value}{data.Count() + 1}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                worksheet.Cells.AutoFitColumns();
                package.Save();
                return package.GetAsByteArray();
            }
        }

        #region Metodos para reporte especificos

        //public byte[] CreateMonthlyStudiesExcel(IEnumerable<QueryReportMonthlyStudies> data)
        //{
        //    var stream = new MemoryStream();
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //    using (ExcelPackage package = new ExcelPackage(stream))
        //    {
        //        //Crea el libro de trabajo
        //        var worksheet = package.Workbook.Worksheets.Add("Sheet1");

        //        //Inicializa el datatable y obtiene la data por parametro
        //        DataTable table = CreateMonthlyStudiesDataTable(data);

        //        //Exporta la data desde el datatable a la hoja de excel

        //        worksheet.Cells.LoadFromDataTable(table, true);

        //        //Estilos de columnas de header

        //        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        //        worksheet.Cells["A1:C1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //        worksheet.Cells["A1:C1"].Style.Fill.BackgroundColor.SetColor(Color.DodgerBlue);
        //        worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //        worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //        worksheet.Cells["A1:C1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //        worksheet.Cells["A1:C1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //        worksheet.Cells["A1:C1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //        worksheet.Cells["A1:C1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

        //        //Estilos de columnas de body

        //        worksheet.Cells[$"A2:C{data.Count() + 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //        worksheet.Cells[$"A2:C{data.Count() + 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //        worksheet.Cells[$"A2:C{data.Count() + 1}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //        worksheet.Cells[$"A2:C{data.Count() + 1}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //        worksheet.Cells[$"A2:C{data.Count() + 1}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //        worksheet.Cells[$"A2:C{data.Count() + 1}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

        //        worksheet.Cells.AutoFitColumns();
        //        package.Save();
        //        return package.GetAsByteArray();
        //    }
        //}

        //public byte[] CreateDailyStudiesExcel(IEnumerable<QueryReportDailyStudies> data)
        //{
        //    var stream = new MemoryStream();
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //    using (ExcelPackage package = new ExcelPackage(stream))
        //    {
        //        //Crea el libro de trabajo
        //        var worksheet = package.Workbook.Worksheets.Add("Sheet1");

        //        //Inicializa el datatable y obtiene la data por parametro
        //        DataTable table = CreateMonthlyStudiesDataTable(data);

        //        //Exporta la data desde el datatable a la hoja de excel

        //        worksheet.Cells.LoadFromDataTable(table, true);

        //        //Estilos de columnas de header

        //        worksheet.Cells["A1:H1"].Style.Font.Bold = true;
        //        worksheet.Cells["A1:H1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //        worksheet.Cells["A1:H1"].Style.Fill.BackgroundColor.SetColor(Color.DodgerBlue);
        //        worksheet.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //        worksheet.Cells["A1:H1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //        worksheet.Cells["A1:H1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //        worksheet.Cells["A1:H1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //        worksheet.Cells["A1:H1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //        worksheet.Cells["A1:H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

        //        //Estilos de columnas de body

        //        worksheet.Cells[$"A2:H{data.Count() + 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //        worksheet.Cells[$"A2:H{data.Count() + 1}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //        worksheet.Cells[$"A2:H{data.Count() + 1}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //        worksheet.Cells[$"A2:H{data.Count() + 1}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //        worksheet.Cells[$"A2:H{data.Count() + 1}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //        worksheet.Cells[$"A2:H{data.Count() + 1}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

        //        worksheet.Cells.AutoFitColumns();
        //        package.Save();
        //        return package.GetAsByteArray();
        //    }
        //}

        #region DataTables

        //private DataTable CreateDailyStudiesDataTable(IEnumerable<QueryReportDailyStudies> data)
        //{
        //    DataTable reports = new DataTable();

        //    if (data is not null && data.Count() > 0)
        //    {
        //        //Definicion de columnas
        //        reports.Columns.Add("FECHA_CREACION", typeof(DateTime));
        //        reports.Columns.Add("CEDULA");
        //        reports.Columns.Add("USUARIO");
        //        reports.Columns.Add("PRODUCTO");
        //        reports.Columns.Add("ESTADO");
        //        reports.Columns.Add("TIPO_CONSULTA");
        //        reports.Columns.Add("MONTO_SOLICITADO", typeof(decimal));
        //        reports.Columns.Add("MONEDA");

        //        //Definicion de lineas
        //        foreach (var row in data)
        //        {
        //            reports.Rows.Add(row.FechA_CREACION, row.Cedula, row.Usuario, row.Producto, row.Estado, row.TipO_CONSULTA, row.MontO_SOLICITADO, row.Moneda);
        //        }
        //    }

        //    return reports;
        //}

        //private DataTable CreateMonthlyStudiesDataTable(IEnumerable<QueryReportMonthlyStudies> data)
        //{
        //    DataTable reports = new DataTable();

        //    if (data is not null && data.Count() > 0)
        //    {
        //        //Definicion de columnas
        //        reports.Columns.Add("ID_ESTUDIO");
        //        reports.Columns.Add("INGRESADO_POR");
        //        reports.Columns.Add("FEC_REGISTRO", typeof(DateTimeOffset));

        //        //Definicion de lineas
        //        foreach (var row in data)
        //        {
        //            reports.Rows.Add(row.ID_ESTUDIO, row.IngresadO_POR, row.FechA_INGRESADO);
        //        }
        //    }

        //    return reports;
        //}

        #endregion

        #endregion

        #region Properties

        public ExcelService()
        {

        }

        public ExcelService(string value) { Value = value; }
        public string? Value { get; set; }

        public static ExcelService One { get { return new ExcelService("A"); } }
        public static ExcelService Two { get { return new ExcelService("B"); } }
        public static ExcelService Three { get { return new ExcelService("C"); } }
        public static ExcelService Four { get { return new ExcelService("D"); } }
        public static ExcelService Five { get { return new ExcelService("E"); } }
        public static ExcelService Six { get { return new ExcelService("F"); } }
        public static ExcelService Seven { get { return new ExcelService("G"); } }
        public static ExcelService Eight { get { return new ExcelService("H"); } }
        public static ExcelService Nine { get { return new ExcelService("I"); } }
        public static ExcelService Ten { get { return new ExcelService("J"); } }

        public override string ToString()
        {
            return Value ?? "";
        }

        #endregion
    }
}
