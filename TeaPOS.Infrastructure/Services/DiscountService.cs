using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeaPOS.Application.Common.DTOs;
using TeaPOS.Infrastructure.Settings;

namespace TeaPOS.Infrastructure.Services
{
    // ĐỔI TÊN interface -> IDiscountService
    public interface IDiscountService
    {
        DiscountDto? GetValidDiscountByCode(string code);
    }

    // Class triển khai interface
    public sealed class DiscountService : IDiscountService
    {
        public DiscountDto? GetValidDiscountByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;

            using var conn = new SqlConnection(DbConfig.ConnectionString);
            conn.Open();

            using var cmd = new SqlCommand(@"
SELECT TOP 1 DiscountID, Code, Percentage, ExpireDate
FROM Discounts
WHERE Code = @code
  AND (ExpireDate IS NULL OR CAST(ExpireDate AS DATE) >= CAST(GETDATE() AS DATE));", conn);

            cmd.Parameters.AddWithValue("@code", code.Trim().ToUpperInvariant());

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new DiscountDto
            {
                DiscountID = rd.GetInt32(0),
                Code = rd.GetString(1),

                // Nếu cột Percentage là FLOAT trong DB
                Percentage = rd.GetFieldType(2) == typeof(double)
                           ? Convert.ToDecimal(rd.GetDouble(2))
                           : rd.GetDecimal(2),

                ExpireDate = rd.IsDBNull(3) ? null : rd.GetDateTime(3)
            };
        }
    }
}
