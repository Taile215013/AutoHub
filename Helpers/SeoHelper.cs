using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoHub.Helpers
{
    public static class SeoHelper
    {
        public static string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var normalizedString = input.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            // Remove diacritics and convert to lowercase
            var slug = stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();

            // Replace specific Vietnamese characters (đ)
            slug = slug.Replace("đ", "d");

            // Replace spaces and special characters with hyphen
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", " ").Trim();
            slug = slug.Replace(" ", "-");

            // Remove multiple hyphens
            slug = Regex.Replace(slug, @"-+", "-");

            return slug;
        }

        public static string GenerateCategorySlug(string vehicleType)
        {
            if (string.IsNullOrWhiteSpace(vehicleType))
                return "khac";

            var normalizedType = vehicleType.Trim().ToLowerInvariant();
            
            // "Auto" là giá trị DB cho ô tô 4 bánh
            if (normalizedType == "auto" || normalizedType.Contains("ô tô") || normalizedType.Contains("xe hơi") || normalizedType.Contains("car"))
            {
                return "cars";
            }
            // "Motorbike" là giá trị DB cho xe máy
            if (normalizedType == "motorbike" || normalizedType == "motorcycle" || normalizedType.Contains("xe máy") || normalizedType.Contains("mô tô"))
            {
                return "moto";
            }
            if (normalizedType.Contains("electric") || normalizedType.Contains("xe điện"))
            {
                return "electric-vehicles";
            }
            if (normalizedType.Contains("cũ") || normalizedType.Contains("đã qua sử dụng"))
            {
                return "xe-cu";
            }

            return GenerateSlug(vehicleType);
        }
    }
}
