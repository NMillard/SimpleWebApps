using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using WebApi.Models;

namespace WebApi.OutputFormatters {
    public class CsvOutputFormatter : OutputFormatter {

        public CsvOutputFormatter() {
            SupportedMediaTypes.Add("application/csv");
        }

        protected override bool CanWriteType(Type type) {
            bool isListWithCsvSerializableType = type.IsAssignableTo(typeof(IEnumerable<ICsvSerializable>));
            return isListWithCsvSerializableType;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context) {
            var contextType = (IEnumerable<ICsvSerializable>) context.Object;
            if (!contextType.Any()) return;

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Join(",", contextType.First().GetCsvPropertyNames()));
            foreach (ICsvSerializable csvSerializable in contextType) {
                stringBuilder.AppendLine(csvSerializable.ToCsv());
            }

            await context.HttpContext.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
            await context.HttpContext.Response.CompleteAsync();
        }
    }
}