using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Microsoft.AspNetCore.Mvc;
using Part_2.Models;

namespace Part_2.Services {
    public class PdfGenerator {
        /// <summary>
        /// Generates an invoice PDF for the given claim.
        /// </summary>
        /// <param name="claim">The claim for which the invoice is generated.</param>
        /// <returns>A File containing the generated PDF.</returns>
        public static FileContentResult GenerateInvoice(Claim claim) {
            Console.WriteLine(Environment.CurrentDirectory);
            using (MemoryStream stream = new MemoryStream()) {
                // Document creation
                iTextSharp.text.Document document = new(PageSize.A4, 50, 50, 25, 25);
                PdfWriter.GetInstance(document, stream);

                document.Open();

                // Fonts for styling
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 26, BaseColor.DARK_GRAY);
                var textFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);
                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                var smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.WHITE);
                var footerFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 10, BaseColor.DARK_GRAY);
                var logoFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);

                // Add logo (assuming the logo file is located at "./wwwroot/Logo.png")
                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Logo.png");
                if (File.Exists(logoPath)) {
                    Image logo = Image.GetInstance(logoPath);
                    logo.ScaleToFit(150f, 150f); // Adjust logo size
                    logo.Alignment = Element.ALIGN_LEFT;
                    document.Add(logo);
                }

                // Add invoice title with background color and gradient
                Paragraph title = new Paragraph("Invoice", titleFont) {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                };
                document.Add(title);

                // Add a soft gradient line below the title (for a smooth, modern look)
                Chunk titleLine = new Chunk(new LineSeparator(0.5f, 100f, new BaseColor(200, 200, 200), Element.ALIGN_CENTER, 1));
                document.Add(titleLine);

                // Create a gradient for background colors on sections (like claim details)
                PdfPTable table = new PdfPTable(2);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2f, 3f }); // Adjust column width ratio

                // Section header with background color and rounded corners
                PdfPCell headerCell = new PdfPCell(new Phrase("Claim Details", headerFont)) {
                    BackgroundColor = new BaseColor(51, 102, 153), // Soft Blue background
                    Colspan = 2,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 10,
                    Border = Rectangle.NO_BORDER
                };
                table.AddCell(headerCell);

                // Styling for the table cells with rounded corners, borders, and shadows
                table.AddCell(CreateTableCell("Claim ID:", boldFont, BaseColor.LIGHT_GRAY, 1));
                table.AddCell(CreateTableCell($"{claim.ID}", textFont, BaseColor.WHITE, 1));

                table.AddCell(CreateTableCell("Title:", boldFont, BaseColor.LIGHT_GRAY, 1));
                table.AddCell(CreateTableCell(claim.Title, textFont, BaseColor.WHITE, 1));

                table.AddCell(CreateTableCell("Hourly Rate:", boldFont, BaseColor.LIGHT_GRAY, 1));
                table.AddCell(CreateTableCell($"${claim.HourlyRate}", textFont, BaseColor.WHITE, 1));

                table.AddCell(CreateTableCell("Hours:", boldFont, BaseColor.LIGHT_GRAY, 1));
                table.AddCell(CreateTableCell($"{claim.Hours}", textFont, BaseColor.WHITE, 1));

                table.AddCell(CreateTableCell("Start Date:", boldFont, BaseColor.LIGHT_GRAY, 1));
                table.AddCell(CreateTableCell($"{claim.StartDate:yyyy-MM-dd}", textFont, BaseColor.WHITE, 1));

                table.AddCell(CreateTableCell("End Date:", boldFont, BaseColor.LIGHT_GRAY, 1));
                table.AddCell(CreateTableCell($"{claim.EndDate:yyyy-MM-dd}", textFont, BaseColor.WHITE, 1));

                table.AddCell(CreateTableCell("Status:", boldFont, BaseColor.LIGHT_GRAY, 1));
                table.AddCell(CreateTableCell(claim.Status, textFont, BaseColor.WHITE, 1));

                table.AddCell(CreateTableCell("Total:", boldFont, BaseColor.LIGHT_GRAY, 2));
                table.AddCell(CreateTableCell($"${claim.Total}", boldFont, BaseColor.WHITE, 2));

                document.Add(table);

                // Add a more prominent line after the table (thicker)
                Chunk linebreak = new Chunk(new LineSeparator(2f, 100f, BaseColor.DARK_GRAY, Element.ALIGN_CENTER, 1));
                document.Add(linebreak);

                // Add footer with a more prominent contact message and visual depth
                Paragraph footer = new Paragraph("Thank you for using our service. If you have any questions, feel free to contact us at support@company.com.", footerFont) {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingBefore = 30
                };
                document.Add(footer);

                // Close document
                document.Close();

                // Return the PDF as a downloadable file
                return new FileContentResult(stream.ToArray(), "application/pdf") {
                    FileDownloadName = $"Invoice_{claim.ID}.pdf"
                };
            }
        }

        // Helper method to create a table cell with rounded corners, borders, and shadow effect
        private static PdfPCell CreateTableCell(string text, Font font, BaseColor bgColor, int borderWidth) {
            var cell = new PdfPCell(new Phrase(text, font)) {
                PaddingBottom = 10,
                PaddingTop = 10,
                PaddingLeft = 15,
                PaddingRight = 15,
                BackgroundColor = bgColor,
                Border = Rectangle.BOX,
                BorderWidth = borderWidth,
                HorizontalAlignment = Element.ALIGN_LEFT,
                BorderColor = new BaseColor(200, 200, 200) // Light gray border color
            };
            cell.BorderWidthTop = 1f;
            cell.BorderWidthBottom = 1f;
            return cell;
        }
    }
}
