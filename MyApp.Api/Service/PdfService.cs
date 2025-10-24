//using KapadaModel.DBOs;
//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;

//namespace MyApp.Api.Service
//{
//    public class PdfService
//    {
//        public byte[] GenerateBuyerSalesPdf(string buyer, IEnumerable<SaleResponseDBO> sales)
//        {
//            var doc = Document.Create(container =>
//            {
//                container.Page(page =>
//                {
//                    page.Margin(20);
//                    page.Size(PageSizes.A4);
//                    page.PageColor(Colors.White);
//                    page.DefaultTextStyle(x => x.FontSize(12));

//                    // Header
//                    page.Header()
//                        .Column(column =>
//                        {
//                            column.Item()
//                                  .PaddingBottom(5)
//                                  .Text("OM Textile")
//                                  .FontSize(20)
//                                  .Bold()
//                                  .AlignCenter();

//                            column.Item()
//                                  .PaddingBottom(10)
//                                  .Text($"Sales Report - Buyer: {buyer}")
//                                  .FontSize(14)
//                                  .SemiBold()
//                                  .AlignCenter();
//                        });

//                    // Content: Group by Category -> Room -> Individual Sales
//                    page.Content()
//                        .Column(column =>
//                        {
//                            var categories = sales.GroupBy(s => s.CategoryName);

//                            foreach (var categoryGroup in categories)
//                            {
//                                column.Item()
//                                      .PaddingBottom(5)
//                                      .Text($"Category: {categoryGroup.Key} - Total Quantity: {categoryGroup.Sum(x => x.QuantitySold)} - Total Price: {categoryGroup.Sum(x => x.TotalPrice)}")
//                                      .SemiBold()
//                                      .FontSize(14);

//                                var rooms = categoryGroup.GroupBy(s => s.RoomName);
//                                foreach (var roomGroup in rooms)
//                                {
//                                    column.Item()
//                                          .PaddingBottom(3)
//                                          .Text($"Room: {roomGroup.Key} - Total Quantity: {roomGroup.Sum(x => x.QuantitySold)} - Total Price: {roomGroup.Sum(x => x.TotalPrice)}")
//                                          .FontSize(13)
//                                          .SemiBold();

//                                    column.Item().Table(table =>
//                                    {
//                                        table.ColumnsDefinition(columns =>
//                                        {
//                                            columns.RelativeColumn(3); // Date
//                                            columns.RelativeColumn(2); // Quantity
//                                            columns.RelativeColumn(2); // Total Price
//                                        });

//                                        table.Header(header =>
//                                        {
//                                            header.Cell().Text("Date").Bold();
//                                            header.Cell().Text("Quantity").Bold();
//                                            header.Cell().Text("Total Price").Bold();
//                                        });

//                                        foreach (var sale in roomGroup.OrderBy(s => s.SaleDate))
//                                        {
//                                            table.Cell().Text(sale.SaleDate.ToShortDateString());
//                                            table.Cell().Text(sale.QuantitySold.ToString());
//                                            table.Cell().Text(sale.TotalPrice.ToString("0.##"));
//                                        }
//                                    });

//                                    column.Item().PaddingBottom(8);
//                                }

//                                column.Item().PaddingBottom(12);
//                            }
//                        });

//                    // Footer
//                    page.Footer()
//                       .AlignCenter()
//                       .Text(txt =>
//                       {
//                           txt.CurrentPageNumber();
//                           txt.Span(" / ");
//                           txt.TotalPages();
//                       });
//                });
//            });

//            return doc.GeneratePdf();
//        }
//    }
//}
//using KapadaModel.DBOs;
//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;

//namespace MyApp.Api.Service
//{
//    public class PdfService
//    {
//        public byte[] GenerateBuyerSalesPdf(string buyer, IEnumerable<SaleResponseDBO> sales)
//        {
//            // Set license type for QuestPDF Community (free for learners and small projects)
//            QuestPDF.Settings.License = LicenseType.Community;

//            var doc = Document.Create(container =>
//            {
//                container.Page(page =>
//                {
//                    page.Margin(20);
//                    page.Size(PageSizes.A4);
//                    page.PageColor(Colors.White);
//                    page.DefaultTextStyle(x => x.FontSize(12));

//                    // Header
//                    page.Header()
//                        .Column(column =>
//                        {
//                            column.Item()
//                                  .Text("OM Textile")
//                                  .FontSize(20)
//                                  .Bold()
//                                  .AlignCenter();

//                            column.Item()
//                                  .PaddingBottom(10)
//                                  .Text($"Sales Report - Buyer: {buyer}")
//                                  .FontSize(14)
//                                  .SemiBold()
//                                  .AlignCenter();
//                        });

//                    // Content: Category -> Room -> Sales Table
//                    page.Content()
//                        .Column(column =>
//                        {
//                            var categories = sales.GroupBy(s => s.CategoryName);

//                            foreach (var categoryGroup in categories)
//                            {
//                                // Category Header
//                                column.Item()
//                                      .PaddingVertical(5)
//                                      .Text($"Category: {categoryGroup.Key} - Total Quantity: {categoryGroup.Sum(x => x.QuantitySold)} - Total Price: {categoryGroup.Sum(x => x.TotalPrice)}")
//                                      .FontSize(14)
//                                      .SemiBold();

//                                var rooms = categoryGroup.GroupBy(s => s.RoomName);
//                                foreach (var roomGroup in rooms)
//                                {
//                                    // Room Header
//                                    column.Item()
//                                          .PaddingVertical(3)
//                                          .Text($"Room: {roomGroup.Key} - Total Quantity: {roomGroup.Sum(x => x.QuantitySold)} - Total Price: {roomGroup.Sum(x => x.TotalPrice)}")
//                                          .FontSize(13)
//                                          .SemiBold();

//                                    // Table of sales
//                                    column.Item()
//                                          .PaddingBottom(8)
//                                          .Table(table =>
//                                          {
//                                              table.ColumnsDefinition(columns =>
//                                              {
//                                                  columns.RelativeColumn(3);
//                                                  columns.RelativeColumn(2);
//                                                  columns.RelativeColumn(2);
//                                              });

//                                              // Table header
//                                              table.Header(header =>
//                                              {
//                                                  header.Cell().Element(cell => cell.Background(Colors.Grey.Lighten2).Border(1).Padding(3))
//                                                      .Text("Date").Bold();
//                                                  header.Cell().Element(cell => cell.Background(Colors.Grey.Lighten2).Border(1).Padding(3))
//                                                      .Text("Quantity").Bold();
//                                                  header.Cell().Element(cell => cell.Background(Colors.Grey.Lighten2).Border(1).Padding(3))
//                                                      .Text("Total Price").Bold();
//                                              });

//                                              // Table rows
//                                              foreach (var sale in roomGroup.OrderBy(s => s.SaleDate))
//                                              {
//                                                  table.Cell().Element(cell => cell.Border(1).Padding(3))
//                                                      .Text(sale.SaleDate.ToShortDateString());
//                                                  table.Cell().Element(cell => cell.Border(1).Padding(3))
//                                                      .Text(sale.QuantitySold.ToString());
//                                                  table.Cell().Element(cell => cell.Border(1).Padding(3))
//                                                      .Text(sale.TotalPrice.ToString("0.##"));
//                                              }
//                                          });
//                                }

//                                // Space after each category
//                                column.Item().PaddingBottom(10);
//                            }
//                        });

//                    // Footer
//                    page.Footer()
//                        .AlignCenter()
//                        .Text(txt =>
//                        {
//                            txt.Span("Page ");
//                            txt.CurrentPageNumber();
//                            txt.Span(" / ");
//                            txt.TotalPages();
//                        });
//                });
//            });

//            return doc.GeneratePdf();
//        }
//    }
//}

using KapadaModel.DBOs;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MyApp.Api.Service
{
    public class PdfService
    {
        public byte[] GenerateBuyerSalesPdf(string buyer, IEnumerable<SaleResponseDBO> sales)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Header
                    page.Header()
                        .Column(column =>
                        {
                            column.Item()
                                  .Text("OM Textile")
                                  .FontSize(20)
                                  .Bold()
                                  .AlignCenter();

                            column.Item()
                                  .PaddingTop(5)
                                  .Text($"Sales Report")
                                  .FontSize(16)
                                  .SemiBold()
                                  .AlignCenter();

                            column.Item()
                                  .PaddingTop(2)
                                  .Text($"Buyer: {buyer}")
                                  .FontSize(14)
                                  .AlignCenter();
                        });

                    // Content: Categories -> Rooms -> Sales Table
                    page.Content()
                        .Column(column =>
                        {
                            var categories = sales.GroupBy(s => s.CategoryName);

                            foreach (var categoryGroup in categories)
                            {
                                // Category details: Name, Quantity, Total Price (separate lines)
                                column.Item().PaddingTop(10)
                                      .Column(catCol =>
                                      {
                                          catCol.Item().Text($"Category: {categoryGroup.Key}").FontSize(14).SemiBold().AlignRight();
                                          catCol.Item().Text($"Total Quantity: {categoryGroup.Sum(x => x.QuantitySold)}m").SemiBold().FontSize(13).AlignRight();
                                          catCol.Item().Text($"Total Price: Rs.{categoryGroup.Sum(x => x.TotalPrice):0.##}").SemiBold().FontSize(13).AlignRight();
                                      });

                                var rooms = categoryGroup.GroupBy(s => s.RoomName);
                                foreach (var roomGroup in rooms)
                                {
                                    // Room details: Name, Quantity, Total Price (separate lines)
                                    column.Item().PaddingTop(5)
                                          .Column(roomCol =>
                                          {
                                              roomCol.Item().Text($"Room: {roomGroup.Key}").FontSize(13).SemiBold();
                                              roomCol.Item().Text($"Total Quantity: {roomGroup.Sum(x => x.QuantitySold)}m").SemiBold().FontSize(12);
                                              //roomCol.Item().Text($"Total Price: Rs.{roomGroup.Sum(x => x.TotalPrice):0.##}").SemiBold().FontSize(12);
                                          });

                                    // Table for sales
                                    column.Item().PaddingTop(5)
                                          .Table(table =>
                                          {
                                              table.ColumnsDefinition(columns =>
                                              {
                                                  columns.RelativeColumn(2);
                                                  columns.RelativeColumn(2);
                                                  //columns.RelativeColumn(2);
                                              });

                                              table.Header(header =>
                                              {
                                                  header.Cell().Element(cell => cell.Background(Colors.Grey.Lighten2).Border(1).Padding(3))
                                                      .Text("Date").Bold();
                                                  header.Cell().Element(cell => cell.Background(Colors.Grey.Lighten2).Border(1).Padding(3))
                                                      .Text("Quantity").Bold();
                                                  //header.Cell().Element(cell => cell.Background(Colors.Grey.Lighten2).Border(1).Padding(3))
                                                  //    .Text("Total Price").Bold();
                                              });

                                              foreach (var sale in roomGroup.OrderBy(s => s.SaleDate))
                                              {
                                                  table.Cell().Element(cell => cell.Border(1).Padding(3))
                                                      .Text(sale.SaleDate.ToShortDateString());
                                                  table.Cell().Element(cell => cell.Border(1).Padding(3))
                                                      .Text(sale.QuantitySold.ToString());
                                                  //table.Cell().Element(cell => cell.Border(1).Padding(3))
                                                  //    .Text(sale.TotalPrice.ToString("0.##"));
                                              }
                                          });
                                }
                            }
                        });

                    // Footer: Page number + "Sales Approved" signature
                    page.Footer()
                        .Column(column =>
                        {
                            column.Item()
                                  .PaddingTop(20)
                                  .AlignRight()
                                  .Column(sigCol =>
                                  {
                                      sigCol.Item().LineHorizontal(1, Unit.Point);

                                      sigCol.Item().Text("Sales Approved By:").FontSize(12).SemiBold();
                                      sigCol.Item().Text("Mr.Nishant Shrestha").FontSize(12).SemiBold();
                                  });

                            column.Item()
                                  .AlignCenter()
                                  .Text(txt =>
                                  {
                                      txt.Span("Page ");
                                      txt.CurrentPageNumber();
                                      txt.Span(" / ");
                                      txt.TotalPages();
                                  });
                        });
                });
            });

            return doc.GeneratePdf();
        }
    }
}


